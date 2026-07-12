namespace GestaoProjetos.Api.Application.DTOs;

public record UserResponse(
    int Id,
    string Username,
    string Email,
    string Role
);
