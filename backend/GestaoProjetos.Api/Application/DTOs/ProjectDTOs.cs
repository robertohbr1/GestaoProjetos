using System.ComponentModel.DataAnnotations;

namespace GestaoProjetos.Api.Application.DTOs;

public record ProjectRequest(
    [Required, MaxLength(150)] string Name,
    [Required, MaxLength(1000)] string Description,
    bool IsActive
);

public record ProjectResponse(
    int Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt
);
