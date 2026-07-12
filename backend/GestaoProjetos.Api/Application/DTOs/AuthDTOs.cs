using System.ComponentModel.DataAnnotations;

namespace GestaoProjetos.Api.Application.DTOs;

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

public record LoginResponse(
    string Token,
    string Username,
    string Role
);

public record RegisterRequest(
    [Required] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string Role
);
