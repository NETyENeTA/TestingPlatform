
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class Answer
{
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public bool IsCorrect { get; set; }

    [Required]
    public int QuestionId { get; set; }

    // Navigation properties
    public virtual Question Question { get; set; }
    public virtual ICollection<UserSelectedOption> UserSelectedOptions { get; set; } = new List<UserSelectedOption>();
}