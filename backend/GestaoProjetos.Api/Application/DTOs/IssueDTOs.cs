using System.ComponentModel.DataAnnotations;
using GestaoProjetos.Api.Domain.Enums;

namespace GestaoProjetos.Api.Application.DTOs;

public record IssueRequest(
    [Required] int ProjectId,
    [Required, MaxLength(250)] string Title,
    [Required, MaxLength(4000)] string Description,
    [Required] IssueType IssueType,
    [Required] ImplementationType ImplementationType,
    [Required, MaxLength(150)] string RequestedBy,
    int? AssignedToUserId,
    [Required] Priority Priority,
    [Required] Status Status,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime? Deadline
);

public record IssueResponse(
    int Id,
    int ProjectId,
    string ProjectName,
    string Title,
    string Description,
    IssueType IssueType,
    ImplementationType ImplementationType,
    string RequestedBy,
    int? AssignedToUserId,
    string? AssignedToUsername,
    Priority Priority,
    Status Status,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime? Deadline,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record IssueDetailResponse(
    int Id,
    int ProjectId,
    string ProjectName,
    string Title,
    string Description,
    IssueType IssueType,
    ImplementationType ImplementationType,
    string RequestedBy,
    int? AssignedToUserId,
    string? AssignedToUsername,
    Priority Priority,
    Status Status,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime? Deadline,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    decimal TotalHoursLogged,
    List<TimeLogResponse> TimeLogs,
    List<CommentResponse> Comments,
    List<AttachmentResponse> Attachments,
    List<AuditLogResponse> AuditLogs
);

public record UpdatePriorityRequest(
    [Required] Priority Priority
);

public record UpdateStatusRequest(
    [Required] Status Status,
    int? AssignedToUserId
);
