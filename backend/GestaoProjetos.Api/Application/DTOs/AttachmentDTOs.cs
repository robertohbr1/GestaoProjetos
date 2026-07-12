namespace GestaoProjetos.Api.Application.DTOs;

public record AttachmentResponse(
    int Id,
    int IssueId,
    string FileName,
    string FilePath,
    string UploadedBy,
    DateTime UploadedAt
);
