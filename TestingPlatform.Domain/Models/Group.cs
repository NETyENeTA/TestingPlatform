using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class Group
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public int DirectionId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    // Navigation properties
    public virtual Direction Direction { get; set; }
    public virtual Course Course { get; set; }
    public virtual Project Project { get; set; }
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}