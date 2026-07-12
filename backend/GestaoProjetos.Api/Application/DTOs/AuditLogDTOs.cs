namespace GestaoProjetos.Api.Application.DTOs;

public record AuditLogResponse(
    int Id,
    int IssueId,
    int UserId,
    string Username,
    string FieldChanged,
    string? OldValue,
    string? NewValue,
    DateTime ChangedAt
);
