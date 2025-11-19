
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Data;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public QuestionRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<QuestionDto>> GetByTestIdAsync(int testId)
    {
        var questions = await _context.Questions
            .Where(q => q.TestId == testId)
            .Include(q => q.Answers)
            .OrderBy(q => q.Number)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<QuestionDto>>(questions);
    }

    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        var question = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            throw new EntityNotFoundException($"Question with id {id} not found");

        return _mapper.Map<QuestionDto>(question);
    }

    public async Task<QuestionDto> GetByIdWithAnswersAsync(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            throw new EntityNotFoundException($"Question with id {id} not found");

        return _mapper.Map<QuestionDto>(question);
    }

    public async Task<int> CreateAsync(QuestionDto questionDto)
    {
        // Проверяем существование теста
        var test = await _context.Tests.FindAsync(questionDto.TestId);
        if (test == null)
            throw new EntityNotFoundException($"Test with id {questionDto.TestId} not found");

        // Если номер не указан, генерируем следующий
        if (questionDto.Number <= 0)
        {
            questionDto.Number = await GetNextQuestionNumberAsync(questionDto.TestId);
        }
        else
        {
            // Проверяем уникальность номера
            var existingQuestion = await _context.Questions
                .FirstOrDefaultAsync(q => q.TestId == questionDto.TestId && q.Number == questionDto.Number);

            if (existingQuestion != null)
                throw new InvalidOperationException($"Question with number {questionDto.Number} already exists in test {questionDto.TestId}");
        }

        var question = _mapper.Map<Question>(questionDto);

        // Валидация в зависимости от типа ответа
        ValidateQuestion(question);

        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();

        return question.Id;
    }

    public async Task UpdateAsync(QuestionDto questionDto)
    {
        var existingQuestion = await _context.Questions
            .Include(q => q.Answers)
            .Include(q => q.UserAttemptAnswers)
            .FirstOrDefaultAsync(q => q.Id == questionDto.Id);

        if (existingQuestion == null)
            throw new EntityNotFoundException($"Question with id {questionDto.Id} not found");

        // Проверяем, можно ли менять тип ответа (если уже есть попытки - нельзя)
        if (existingQuestion.AnswerType != questionDto.AnswerType.ToString() &&
            existingQuestion.UserAttemptAnswers.Any())
        {
            throw new InvalidOperationException("Cannot change answer type for question that has attempts");
        }

        // Проверяем уникальность номера, если он изменился
        if (existingQuestion.Number != questionDto.Number)
        {
            var duplicateQuestion = await _context.Questions
                .FirstOrDefaultAsync(q => q.TestId == existingQuestion.TestId &&
                                         q.Number == questionDto.Number &&
                                         q.Id != questionDto.Id);

            if (duplicateQuestion != null)
                throw new InvalidOperationException($"Question with number {questionDto.Number} already exists in test {existingQuestion.TestId}");
        }

        // Обновляем поля
        existingQuestion.Text = questionDto.Text;
        existingQuestion.Number = questionDto.Number;
        existingQuestion.Description = questionDto.Description;
        existingQuestion.AnswerType = questionDto.AnswerType.ToString();
        existingQuestion.IsScoring = questionDto.IsScoring;
        existingQuestion.MaxScore = questionDto.MaxScore;

        // Валидация
        ValidateQuestion(existingQuestion);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await _context.Questions
            .Include(q => q.UserAttemptAnswers)
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            throw new EntityNotFoundException($"Question with id {id} not found");

        // Проверяем, есть ли попытки ответов на этот вопрос
        if (question.UserAttemptAnswers.Any())
            throw new InvalidOperationException("Cannot delete question that has attempts");

        // Удаляем связанные ответы
        _context.Answers.RemoveRange(question.Answers);
        _context.Questions.Remove(question);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Questions.AnyAsync(q => q.Id == id);
    }

    public async Task<int> GetNextQuestionNumberAsync(int testId)
    {
        var lastQuestionNumber = await _context.Questions
            .Where(q => q.TestId == testId)
            .OrderByDescending(q => q.Number)
            .Select(q => q.Number)
            .FirstOrDefaultAsync();

        return lastQuestionNumber + 1;
    }

    public async Task<int> GetQuestionCountAsync(int testId)
    {
        return await _context.Questions
            .CountAsync(q => q.TestId == testId);
    }

    public async Task<IEnumerable<QuestionDto>> GetByTestIdWithPaginationAsync(int testId, int pageNumber, int pageSize)
    {
        var questions = await _context.Questions
            .Where(q => q.TestId == testId)
            .Include(q => q.Answers)
            .OrderBy(q => q.Number)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<QuestionDto>>(questions);
    }

    /// <summary>
    /// Валидация вопроса в зависимости от типа ответа
    /// </summary>
    private void ValidateQuestion(Question question)
    {
        // Для текстовых вопросов не должно быть заранее созданных ответов
        if (question.AnswerType == AnswerType.Text.ToString() && question.Answers.Any())
        {
            throw new InvalidOperationException("Text questions cannot have predefined answers");
        }

        // Для вопросов с выбором должен быть хотя бы один ответ
        if ((question.AnswerType == AnswerType.Single.ToString() ||
             question.AnswerType == AnswerType.Multiple.ToString()) &&
            !question.Answers.Any())
        {
            throw new InvalidOperationException("Choice questions must have at least one answer");
        }

        // Для вопросов с одним правильным ответом должен быть ровно один правильный ответ
        if (question.AnswerType == AnswerType.Single.ToString())
        {
            var correctAnswersCount = question.Answers.Count(a => a.IsCorrect);
            if (correctAnswersCount != 1)
            {
                throw new InvalidOperationException("Single-choice questions must have exactly one correct answer");
            }
        }

        // Для вопросов с множественным выбором должен быть хотя бы один правильный ответ
        if (question.AnswerType == AnswerType.Multiple.ToString())
        {
            var correctAnswersCount = question.Answers.Count(a => a.IsCorrect);
            if (correctAnswersCount == 0)
            {
                throw new InvalidOperationException("Multiple-choice questions must have at least one correct answer");
            }
        }

        // Проверка максимального балла
        if (question.IsScoring && (!question.MaxScore.HasValue || question.MaxScore <= 0))
        {
            throw new InvalidOperationException("Scoring questions must have a positive MaxScore");
        }
    }
}