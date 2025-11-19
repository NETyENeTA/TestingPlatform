using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Application.Dtos;

public class CreateStudentDto
{
    public string Phone { get; set; }
    public string VkProfileLink { get; set; }
    public int UserId { get; set; }
}