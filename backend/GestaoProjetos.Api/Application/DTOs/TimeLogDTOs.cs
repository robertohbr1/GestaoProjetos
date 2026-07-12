using System.ComponentModel.DataAnnotations;

namespace GestaoProjetos.Api.Application.DTOs;

public record TimeLogRequest(
    [Required] int IssueId,
    [Required] DateTime LoggedDate,
    [Required, Range(0.1, 24.0)] decimal HoursSpent,
    [Required, MaxLength(1000)] string WorkDescription
);

public record TimeLogResponse(
    int Id,
    int IssueId,
    string IssueTitle,
    int UserId,
    string Username,
    DateTime LoggedDate,
    decimal HoursSpent,
    string WorkDescription
);
