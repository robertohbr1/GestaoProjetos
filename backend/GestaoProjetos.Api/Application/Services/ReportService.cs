using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Domain.Enums;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Application.Services;

public interface IReportService
{
    Task<DashboardSummary> GetDashboardSummaryAsync();
    Task<IEnumerable<IssueResponse>> GetCompletedInPeriodAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<DeveloperWorkload>> GetInDevelopmentWorkloadAsync();
    Task<IEnumerable<IssueResponse>> GetPendingIssuesAsync();
}

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync()
    {
        var totals = await _context.Issues
            .GroupBy(i => 1) // group all
            .Select(g => new
            {
                Completed = g.Count(i => i.Status == Status.Done),
                InDev = g.Count(i => i.Status == Status.InDevelopment),
                Pending = g.Count(i => i.Status == Status.Pending || i.Status == Status.Backlog || i.Status == Status.InAnalysis || i.Status == Status.InTesting),
                Critical = g.Count(i => i.Priority == Priority.Critical && i.Status != Status.Done && i.Status != Status.Cancelled)
            })
            .FirstOrDefaultAsync();

        if (totals == null)
        {
            return new DashboardSummary(0, 0, 0, 0);
        }

        return new DashboardSummary(totals.Completed, totals.InDev, totals.Pending, totals.Critical);
    }

    public async Task<IEnumerable<IssueResponse>> GetCompletedInPeriodAsync(DateTime startDate, DateTime endDate)
    {
        var issues = await _context.Issues
            .Include(i => i.Project)
            .Include(i => i.AssignedToUser)
            .Where(i => i.Status == Status.Done && i.EndDate.HasValue && i.EndDate.Value.Date >= startDate.Date && i.EndDate.Value.Date <= endDate.Date)
            .OrderByDescending(i => i.EndDate)
            .AsNoTracking()
            .ToListAsync();

        return issues.Select(MapToResponse);
    }

    public async Task<IEnumerable<DeveloperWorkload>> GetInDevelopmentWorkloadAsync()
    {
        // Get all developers with their in-development tasks
        var devs = await _context.Users
            .Where(u => u.Role == UserRole.Developer || u.Role == UserRole.Administrator)
            .AsNoTracking()
            .ToListAsync();

        var workload = new List<DeveloperWorkload>();

        foreach (var dev in devs)
        {
            var devIssues = await _context.Issues
                .Include(i => i.Project)
                .Include(i => i.AssignedToUser)
                .Where(i => i.Status == Status.InDevelopment && i.AssignedToUserId == dev.Id)
                .AsNoTracking()
                .ToListAsync();

            workload.Add(new DeveloperWorkload(
                dev.Id,
                dev.Username,
                devIssues.Select(MapToResponse).ToList()
            ));
        }

        // Also add a "No Developer Assigned" card if there are tasks In Development without a developer
        var unassignedIssues = await _context.Issues
            .Include(i => i.Project)
            .Where(i => i.Status == Status.InDevelopment && i.AssignedToUserId == null)
            .AsNoTracking()
            .ToListAsync();

        if (unassignedIssues.Any())
        {
            workload.Add(new DeveloperWorkload(
                0,
                "Não Alocado",
                unassignedIssues.Select(MapToResponse).ToList()
            ));
        }

        return workload;
    }

    public async Task<IEnumerable<IssueResponse>> GetPendingIssuesAsync()
    {
        // Pending issues: backlog, pending, analysis, testing
        var pendingStatuses = new[] { Status.Backlog, Status.Pending, Status.InAnalysis, Status.InTesting };

        var issues = await _context.Issues
            .Include(i => i.Project)
            .Include(i => i.AssignedToUser)
            .Where(i => pendingStatuses.Contains(i.Status))
            .OrderBy(i => i.Deadline.HasValue) // Items with deadline first
            .ThenBy(i => i.Deadline)
            .ThenByDescending(i => i.Priority)
            .AsNoTracking()
            .ToListAsync();

        return issues.Select(MapToResponse);
    }

    private static IssueResponse MapToResponse(Issue issue)
    {
        return new IssueResponse(
            issue.Id,
            issue.ProjectId,
            issue.Project?.Name ?? string.Empty,
            issue.Title,
            issue.Description,
            issue.IssueType,
            issue.ImplementationType,
            issue.RequestedBy,
            issue.AssignedToUserId,
            issue.AssignedToUser?.Username,
            issue.Priority,
            issue.Status,
            issue.StartDate,
            issue.EndDate,
            issue.Deadline,
            issue.CreatedAt,
            issue.UpdatedAt
        );
    }
}
