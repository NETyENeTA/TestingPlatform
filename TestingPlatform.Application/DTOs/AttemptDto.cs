namespace TestingPlatform.Application.Dtos;

public class AttemptDto
{
    public int Id { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public int? Score { get; set; }
    public int TestId { get; set; }
    public int StudentId { get; set; }
    public List<UserAttemptAnswerDto> UserAttemptAnswers { get; set; } = new();
}