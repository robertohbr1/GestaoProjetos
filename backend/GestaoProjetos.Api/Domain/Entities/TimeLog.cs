namespace GestaoProjetos.Api.Domain.Entities;

public class TimeLog
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime LoggedDate { get; set; }
    public decimal HoursSpent { get; set; }
    public required string WorkDescription { get; set; }
}
