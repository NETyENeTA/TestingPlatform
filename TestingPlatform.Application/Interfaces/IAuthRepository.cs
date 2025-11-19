
using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IAuthRepository
{
    Task<UserDto> GetUserByLoginAsync(string login);
    Task<UserDto> CreateUserAsync(RegisterDto registerDto);
    Task<bool> UserExistsAsync(string login, string email);
}