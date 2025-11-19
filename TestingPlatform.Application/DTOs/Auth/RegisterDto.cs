using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Application.Dtos;

public class RegisterDto
{
    [Required]
    public string Login { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public UserRole Role { get; set; }
}