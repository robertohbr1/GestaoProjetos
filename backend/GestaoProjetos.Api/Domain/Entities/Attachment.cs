namespace GestaoProjetos.Api.Domain.Entities;

public class Attachment
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}
