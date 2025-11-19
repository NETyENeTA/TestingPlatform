
using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserDto user);
    int? ValidateToken(string token);
}