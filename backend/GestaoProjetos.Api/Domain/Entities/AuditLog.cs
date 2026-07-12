namespace GestaoProjetos.Api.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string FieldChanged { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
}
