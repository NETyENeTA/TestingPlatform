using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Application.Dtos;

public class TestDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsRepeatable { get; set; }
    public TestType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime Deadline { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsPublic { get; set; }
    public int? PassingScore { get; set; }
    public int? MaxAttempts { get; set; }
    public List<StudentDto> Students { get; set; } = new();
    public List<ProjectDto> Projects { get; set; } = new();
    public List<CourseDto> Courses { get; set; } = new();
    public List<GroupDto> Groups { get; set; } = new();
    public List<DirectionDto> Directions { get; set; } = new();
}