namespace TestingPlatform.Application.Dtos;

public class UserAttemptAnswerDto
{
    public int Id { get; set; }
    public bool? IsCorrect { get; set; }
    public int ScoreAwarded { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public List<UserSelectedOptionDto> UserSelectedOptions { get; set; } = new();
    public UserTextAnswerDto UserTextAnswer { get; set; }
}