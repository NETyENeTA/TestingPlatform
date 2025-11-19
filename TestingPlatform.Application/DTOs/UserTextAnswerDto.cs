namespace TestingPlatform.Application.Dtos;

public class UserTextAnswerDto
{
    public int Id { get; set; }
    public string TextAnswer { get; set; }
    public int UserAttemptAnswerId { get; set; }
}