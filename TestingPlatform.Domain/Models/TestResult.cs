
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class TestResult
{
    public int Id { get; set; }

    [Required]
    public bool Passed { get; set; }

    [Required]
    public int TestId { get; set; }

    [Required]
    public int AttemptId { get; set; }

    [Required]
    public int StudentId { get; set; }

    // Navigation properties
    public virtual Test Test { get; set; }
    public virtual Attempt Attempt { get; set; }
    public virtual Student Student { get; set; }
}