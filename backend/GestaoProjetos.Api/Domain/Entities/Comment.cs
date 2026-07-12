namespace GestaoProjetos.Api.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}
