namespace GestaoProjetos.Api.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
