using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Application.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; }
}