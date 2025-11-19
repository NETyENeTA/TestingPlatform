
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Domain.Models;

public class Project
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    // Navigation properties
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}