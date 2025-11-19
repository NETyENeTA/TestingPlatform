using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> GetByLoginAsync(string login);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto userDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}