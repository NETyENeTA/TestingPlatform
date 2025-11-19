using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Application.Dtos;

public class AnswerDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public int QuestionId { get; set; }
}