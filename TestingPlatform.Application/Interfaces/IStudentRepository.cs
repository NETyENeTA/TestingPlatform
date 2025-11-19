
using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<StudentDto>> GetAllAsync();
    Task<StudentDto> GetByIdAsync(int id);
    Task<StudentDto> GetByUserIdAsync(int userId);
    Task<StudentDto> CreateAsync(CreateStudentDto studentDto);
    Task<StudentDto> UpdateAsync(int id, UpdateStudentDto studentDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}