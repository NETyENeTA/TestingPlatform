
using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface ITestRepository
{
    Task<IEnumerable<TestDto>> GetAllAsync(bool? isPublic = null, List<int>? groupIds = null, List<int>? studentIds = null);
    Task<TestDto> GetByIdAsync(int id);
    Task<IEnumerable<TestDto>> GetAllForStudentAsync(int studentId);
    Task<int> CreateAsync(TestDto testDto);
    Task UpdateAsync(TestDto testDto);
    Task DeleteAsync(int id);
}