using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Application.Dtos;

public class UpdateStudentDto
{
    public string Phone { get; set; }
    public string VkProfileLink { get; set; }
}