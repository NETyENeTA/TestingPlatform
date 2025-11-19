
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class Student
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Phone { get; set; }

    [Required]
    public string VkProfileLink { get; set; }

    [Required]
    public int UserId { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    public virtual ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>(); // Добавили это
}