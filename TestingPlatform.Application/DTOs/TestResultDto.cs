namespace TestingPlatform.Application.Dtos;

public class TestResultDto
{
    public int Id { get; set; }
    public bool Passed { get; set; }
    public int TestId { get; set; }
    public int AttemptId { get; set; }
    public int StudentId { get; set; }
}