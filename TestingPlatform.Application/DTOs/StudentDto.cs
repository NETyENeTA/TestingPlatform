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
}