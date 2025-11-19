
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class Test
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public bool IsRepeatable { get; set; } = false;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } // Store as string

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    public DateTimeOffset PublishedAt { get; set; }

    [Required]
    public DateTimeOffset Deadline { get; set; }

    public int? DurationMinutes { get; set; }

    public bool IsPublic { get; set; } = false;

    public int? PassingScore { get; set; }

    public int? MaxAttempts { get; set; }

    // Navigation properties
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    public virtual ICollection<Direction> Directions { get; set; } = new List<Direction>();
    public virtual ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}