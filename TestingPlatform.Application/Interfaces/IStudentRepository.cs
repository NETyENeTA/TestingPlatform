
using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<StudentDto>> GetAllAsync();
    Task<StudentDto> GetByIdAsync(int id);
    Task<int> CreateAsync(StudentDto studentDto);
    Task UpdateAsync(StudentDto studentDto);
    Task DeleteAsync(int id);
}