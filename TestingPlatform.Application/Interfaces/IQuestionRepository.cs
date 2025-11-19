
using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IQuestionRepository
{
    /// <summary>
    /// Получить все вопросы для теста
    /// </summary>
    Task<IEnumerable<QuestionDto>> GetByTestIdAsync(int testId);

    /// <summary>
    /// Получить вопрос по ID
    /// </summary>
    Task<QuestionDto> GetByIdAsync(int id);

    /// <summary>
    /// Получить вопрос по ID с ответами
    /// </summary>
    Task<QuestionDto> GetByIdWithAnswersAsync(int id);

    /// <summary>
    /// Создать новый вопрос
    /// </summary>
    Task<int> CreateAsync(QuestionDto questionDto);

    /// <summary>
    /// Обновить вопрос
    /// </summary>
    Task UpdateAsync(QuestionDto questionDto);

    /// <summary>
    /// Удалить вопрос
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Проверить существование вопроса
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Получить следующий номер вопроса для теста
    /// </summary>
    Task<int> GetNextQuestionNumberAsync(int testId);

    /// <summary>
    /// Получить количество вопросов в тесте
    /// </summary>
    Task<int> GetQuestionCountAsync(int testId);

    /// <summary>
    /// Получить вопросы с пагинацией
    /// </summary>
    Task<IEnumerable<QuestionDto>> GetByTestIdWithPaginationAsync(int testId, int pageNumber, int pageSize);
}