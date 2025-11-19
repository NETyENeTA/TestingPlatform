// AnswerRepository.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Data;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AnswerRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> CreateAsync(AnswerDto answerDto)
    {
        // Проверяем существование вопроса
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == answerDto.QuestionId);

        if (question == null)
            throw new EntityNotFoundException($"Question with id {answerDto.QuestionId} not found");

        // Валидация для текстовых вопросов
        if (question.AnswerType == AnswerType.Text.ToString())
            throw new InvalidOperationException("Cannot add answers to text-based questions");

        // Валидация для вопросов с одним ответом
        if (question.AnswerType == AnswerType.Single.ToString() &&
            answerDto.IsCorrect &&
            question.Answers.Any(a => a.IsCorrect))
        {
            throw new InvalidOperationException("Single-choice question can have only one correct answer");
        }

        var answer = _mapper.Map<Answer>(answerDto);

        await _context.Answers.AddAsync(answer);
        await _context.SaveChangesAsync();

        return answer.Id;
    }

    public async Task UpdateAsync(AnswerDto answerDto)
    {
        var existingAnswer = await _context.Answers
            .Include(a => a.Question)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(a => a.Id == answerDto.Id);

        if (existingAnswer == null)
            throw new EntityNotFoundException($"Answer with id {answerDto.Id} not found");

        // Проверяем, не меняется ли правильность ответа для single-choice вопросов
        if (existingAnswer.Question.AnswerType == AnswerType.Single.ToString() &&
            answerDto.IsCorrect &&
            !existingAnswer.IsCorrect)
        {
            var hasOtherCorrectAnswer = existingAnswer.Question.Answers
                .Any(a => a.Id != answerDto.Id && a.IsCorrect);

            if (hasOtherCorrectAnswer)
                throw new InvalidOperationException("Single-choice question can have only one correct answer");
        }

        // Обновляем поля
        existingAnswer.Text = answerDto.Text;
        existingAnswer.IsCorrect = answerDto.IsCorrect;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var answer = await _context.Answers
            .Include(a => a.UserSelectedOptions)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (answer == null)
            throw new EntityNotFoundException($"Answer with id {id} not found");

        // Проверяем, используется ли ответ в попытках
        if (answer.UserSelectedOptions.Any())
            throw new InvalidOperationException("Cannot delete answer that is used in attempts");

        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();
    }

    public async Task<AnswerDto> GetByIdAsync(int id)
    {
        var answer = await _context.Answers
            .Include(a => a.Question)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (answer == null)
            throw new EntityNotFoundException($"Answer with id {id} not found");

        return _mapper.Map<AnswerDto>(answer);
    }

    public async Task<IEnumerable<AnswerDto>> GetByQuestionIdAsync(int questionId)
    {
        var answers = await _context.Answers
            .Where(a => a.QuestionId == questionId)
            .OrderBy(a => a.Id)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<AnswerDto>>(answers);
    }

    public async Task<IEnumerable<AnswerDto>> GetCorrectAnswersByQuestionIdAsync(int questionId)
    {
        var answers = await _context.Answers
            .Where(a => a.QuestionId == questionId && a.IsCorrect)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<AnswerDto>>(answers);
    }

    /// <summary>
    /// Проверяет, принадлежит ли ответ указанному вопросу
    /// </summary>
    public async Task<bool> IsAnswerBelongsToQuestionAsync(int answerId, int questionId)
    {
        return await _context.Answers
            .AnyAsync(a => a.Id == answerId && a.QuestionId == questionId);
    }
}