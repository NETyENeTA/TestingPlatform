using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Domain.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Login { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    public UserRole Role { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual Student? Student { get; set; }
}