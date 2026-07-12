using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        return users.Select(u => new UserResponse(u.Id, u.Username, u.Email, u.Role.ToString()));
    }
}
