using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Application.Services;

public interface ITimeLogService
{
    Task<IEnumerable<TimeLogResponse>> GetByIssueIdAsync(int issueId);
    Task<TimeLogResponse> CreateAsync(TimeLogRequest request, int userId);
    Task<bool> DeleteAsync(int id);
}

public class TimeLogService : ITimeLogService
{
    private readonly AppDbContext _context;

    public TimeLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TimeLogResponse>> GetByIssueIdAsync(int issueId)
    {
        var logs = await _context.TimeLogs
            .Include(tl => tl.Issue)
            .Include(tl => tl.User)
            .Where(tl => tl.IssueId == issueId)
            .AsNoTracking()
            .ToListAsync();

        return logs.Select(MapToResponse);
    }

    public async Task<TimeLogResponse> CreateAsync(TimeLogRequest request, int userId)
    {
        var timeLog = new TimeLog
        {
            IssueId = request.IssueId,
            UserId = userId,
            LoggedDate = request.LoggedDate,
            HoursSpent = request.HoursSpent,
            WorkDescription = request.WorkDescription
        };

        _context.TimeLogs.Add(timeLog);
        await _context.SaveChangesAsync();

        // Load navigations
        await _context.Entry(timeLog).Reference(tl => tl.Issue).LoadAsync();
        await _context.Entry(timeLog).Reference(tl => tl.User).LoadAsync();

        return MapToResponse(timeLog);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var log = await _context.TimeLogs.FirstOrDefaultAsync(tl => tl.Id == id);
        if (log == null) return false;

        _context.TimeLogs.Remove(log);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TimeLogResponse MapToResponse(TimeLog tl)
    {
        return new TimeLogResponse(
            tl.Id,
            tl.IssueId,
            tl.Issue?.Title ?? string.Empty,
            tl.UserId,
            tl.User?.Username ?? string.Empty,
            tl.LoggedDate,
            tl.HoursSpent,
            tl.WorkDescription
        );
    }
}
