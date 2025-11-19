
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class UserSelectedOption
{
    public int Id { get; set; }

    [Required]
    public int UserAttemptAnswerId { get; set; }

    [Required]
    public int AnswerId { get; set; }

    // Navigation properties
    public virtual UserAttemptAnswer UserAttemptAnswer { get; set; }
    public virtual Answer Answer { get; set; }
}