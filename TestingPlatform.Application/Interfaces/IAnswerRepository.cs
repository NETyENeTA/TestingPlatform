
using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IAnswerRepository
{
    /// <summary>
    /// Создать новый ответ
    /// </summary>
    Task<int> CreateAsync(AnswerDto answerDto);

    /// <summary>
    /// Обновить информацию об ответе
    /// </summary>
    Task UpdateAsync(AnswerDto answerDto);

    /// <summary>
    /// Удалить ответ
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Получить ответ по ID
    /// </summary>
    Task<AnswerDto> GetByIdAsync(int id);

    /// <summary>
    /// Получить все ответы для вопроса
    /// </summary>
    Task<IEnumerable<AnswerDto>> GetByQuestionIdAsync(int questionId);

    /// <summary>
    /// Получить правильные ответы для вопроса
    /// </summary>
    Task<IEnumerable<AnswerDto>> GetCorrectAnswersByQuestionIdAsync(int questionId);
}