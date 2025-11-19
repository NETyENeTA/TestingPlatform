
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class UserTextAnswer
{
    public int Id { get; set; }

    [Required]
    public string TextAnswer { get; set; }

    [Required]
    public int UserAttemptAnswerId { get; set; }

    // Navigation properties
    public virtual UserAttemptAnswer UserAttemptAnswer { get; set; }
}