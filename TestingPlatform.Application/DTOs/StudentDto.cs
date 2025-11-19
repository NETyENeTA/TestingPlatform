using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Application.Dtos;

public class StudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string VkProfileLink { get; set; }
    public int UserId { get; set; }
    public UserDto User { get; set; }
    public List<GroupDto> Groups { get; set; } = new();
    public List<TestDto> Tests { get; set; } = new();

}