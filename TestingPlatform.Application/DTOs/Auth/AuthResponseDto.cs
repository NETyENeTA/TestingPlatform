using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Application.Dtos;

public class AuthResponseDto
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public UserDto User { get; set; }
}