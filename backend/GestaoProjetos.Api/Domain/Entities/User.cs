using GestaoProjetos.Api.Domain.Enums;

namespace GestaoProjetos.Api.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }

    // Navigation properties
    public ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
    public ICollection<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
