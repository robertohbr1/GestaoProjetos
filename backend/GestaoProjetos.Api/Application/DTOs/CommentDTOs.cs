using System.ComponentModel.DataAnnotations;

namespace GestaoProjetos.Api.Application.DTOs;

public record CommentRequest(
    [Required, MaxLength(2000)] string Content
);

public record CommentResponse(
    int Id,
    int IssueId,
    int UserId,
    string Username,
    string Content,
    DateTime CreatedAt
);
