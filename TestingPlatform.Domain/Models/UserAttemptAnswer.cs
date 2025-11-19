
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class UserAttemptAnswer
{
    public int Id { get; set; }

    public bool? IsCorrect { get; set; }
    public int ScoreAwarded { get; set; }

    [Required]
    public int AttemptId { get; set; }

    [Required]
    public int QuestionId { get; set; }

    // Navigation properties
    public virtual Attempt Attempt { get; set; }
    public virtual Question Question { get; set; }
    public virtual ICollection<UserSelectedOption> UserSelectedOptions { get; set; } = new List<UserSelectedOption>();
    public virtual UserTextAnswer UserTextAnswer { get; set; }
}