
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public int Number { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string AnswerType { get; set; } // Store as string

    public bool IsScoring { get; set; } = true;

    public int? MaxScore { get; set; }

    [Required]
    public int TestId { get; set; }

    // Navigation properties
    public virtual Test Test { get; set; }
    public virtual ICollection<UserAttemptAnswer> UserAttemptAnswers { get; set; } = new List<UserAttemptAnswer>();
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}