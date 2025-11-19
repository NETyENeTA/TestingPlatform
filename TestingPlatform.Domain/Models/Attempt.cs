
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class Attempt
{
    public int Id { get; set; }

    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SubmittedAt { get; set; }
    public int? Score { get; set; }

    [Required]
    public int TestId { get; set; }

    [Required]
    public int StudentId { get; set; }

    // Navigation properties
    public virtual Test Test { get; set; }
    public virtual Student Student { get; set; }
    public virtual ICollection<UserAttemptAnswer> UserAttemptAnswers { get; set; } = new List<UserAttemptAnswer>();
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}