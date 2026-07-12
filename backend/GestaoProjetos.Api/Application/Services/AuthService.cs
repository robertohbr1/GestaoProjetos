using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Domain.Enums;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GestaoProjetos.Api.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserResponse?> RegisterAsync(RegisterRequest request);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var token = GenerateJwtToken(user);
        return new LoginResponse(token, user.Username, user.Role.ToString());
    }

    public async Task<UserResponse?> RegisterAsync(RegisterRequest request)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.Username == request.Username || u.Email == request.Email);

        if (exists)
        {
            return null;
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            userRole = UserRole.Collaborator;
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = userRole
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponse(user.Id, user.Username, user.Email, user.Role.ToString());
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["Jwt:Secret"] ?? "SuperSecretKeyForProjectManagementApp2026!!!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "GestaoProjetosIssuer",
            audience: _configuration["Jwt:Audience"] ?? "GestaoProjetosAudience",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
