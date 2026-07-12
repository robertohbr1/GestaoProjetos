using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Domain.Enums;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Application.Services;

public interface IIssueService
{
    Task<IEnumerable<IssueResponse>> GetFilteredAsync(
        ImplementationType? implementationType,
        int? projectId,
        Status? status,
        int? assignedToUserId,
        Priority? priority,
        string? searchTerm
    );
    Task<IssueDetailResponse?> GetByIdAsync(int id);
    Task<IssueResponse> CreateAsync(IssueRequest request);
    Task<IssueResponse?> UpdateAsync(int id, IssueRequest request, int currentUserId);
    Task<IssueResponse?> UpdatePriorityAsync(int id, Priority priority, int currentUserId);
    Task<IssueResponse?> UpdateStatusAsync(int id, Status status, int? assignedToUserId, int currentUserId);
    Task<bool> DeleteAsync(int id);
}

public class IssueService : IIssueService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public IssueService(AppDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<IEnumerable<IssueResponse>> GetFilteredAsync(
        ImplementationType? implementationType,
        int? projectId,
        Status? status,
        int? assignedToUserId,
        Priority? priority,
        string? searchTerm
    )
    {
        var query = _context.Issues
            .Include(i => i.Project)
            .Include(i => i.AssignedToUser)
            .AsNoTracking();

        if (implementationType.HasValue)
            query = query.Where(i => i.ImplementationType == implementationType.Value);
        
        if (projectId.HasValue)
            query = query.Where(i => i.ProjectId == projectId.Value);
        
        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);
        
        if (assignedToUserId.HasValue)
            query = query.Where(i => i.AssignedToUserId == assignedToUserId.Value);
        
        if (priority.HasValue)
            query = query.Where(i => i.Priority == priority.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(i => i.Title.ToLower().Contains(term) || i.Description.ToLower().Contains(term));
        }

        var issues = await query.ToListAsync();
        return issues.Select(MapToResponse);
    }

    public async Task<IssueDetailResponse?> GetByIdAsync(int id)
    {
        var issue = await _context.Issues
            .Include(i => i.Project)
            .Include(i => i.AssignedToUser)
            .Include(i => i.TimeLogs).ThenInclude(tl => tl.User)
            .Include(i => i.Comments).ThenInclude(c => c.User)
            .Include(i => i.Attachments)
            .Include(i => i.AuditLogs).ThenInclude(al => al.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null) return null;

        var totalHours = issue.TimeLogs.Sum(tl => tl.HoursSpent);

        return new IssueDetailResponse(
            issue.Id,
            issue.ProjectId,
            issue.Project.Name,
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
            issue.UpdatedAt,
            totalHours,
            issue.TimeLogs.Select(tl => new TimeLogResponse(
                tl.Id, tl.IssueId, issue.Title, tl.UserId, tl.User.Username, tl.LoggedDate, tl.HoursSpent, tl.WorkDescription
            )).ToList(),
            issue.Comments.Select(c => new CommentResponse(
                c.Id, c.IssueId, c.UserId, c.User.Username, c.Content, c.CreatedAt
            )).OrderByDescending(c => c.CreatedAt).ToList(),
            issue.Attachments.Select(a => new AttachmentResponse(
                a.Id, a.IssueId, a.FileName, a.FilePath, a.UploadedBy, a.UploadedAt
            )).ToList(),
            issue.AuditLogs.Select(al => new AuditLogResponse(
                al.Id, al.IssueId, al.UserId, al.User.Username, al.FieldChanged, al.OldValue, al.NewValue, al.ChangedAt
            )).OrderByDescending(al => al.ChangedAt).ToList()
        );
    }

    public async Task<IssueResponse> CreateAsync(IssueRequest request)
    {
        var issue = new Issue
        {
            ProjectId = request.ProjectId,
            Title = request.Title,
            Description = request.Description,
            IssueType = request.IssueType,
            ImplementationType = request.ImplementationType,
            RequestedBy = request.RequestedBy,
            AssignedToUserId = request.AssignedToUserId,
            Priority = request.Priority,
            Status = request.Status,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Deadline = request.Deadline,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();

        // Load project and user details for response
        await _context.Entry(issue).Reference(i => i.Project).LoadAsync();
        if (issue.AssignedToUserId.HasValue)
        {
            await _context.Entry(issue).Reference(i => i.AssignedToUser).LoadAsync();
        }

        return MapToResponse(issue);
    }

    public async Task<IssueResponse?> UpdateAsync(int id, IssueRequest request, int currentUserId)
    {
        var issue = await _context.Issues
            .Include(i => i.Project)
            .Include(i => i.AssignedToUser)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null) return null;

        // Log Audits
        await _auditService.LogChangeAsync(id, currentUserId, "Status", issue.Status.ToString(), request.Status.ToString());
        await _auditService.LogChangeAsync(id, currentUserId, "AssignedUser", issue.AssignedToUserId?.ToString(), request.AssignedToUserId?.ToString());
        await _auditService.LogChangeAsync(id, currentUserId, "Priority", issue.Priority.ToString(), request.Priority.ToString());
        await _auditService.LogChangeAsync(id, currentUserId, "Deadline", issue.Deadline?.ToString("yyyy-MM-dd"), request.Deadline?.ToString("yyyy-MM-dd"));

        issue.ProjectId = request.ProjectId;
        issue.Title = request.Title;
        issue.Description = request.Description;
        issue.IssueType = request.IssueType;
        issue.ImplementationType = request.ImplementationType;
        issue.RequestedBy = request.RequestedBy;
        issue.AssignedToUserId = request.AssignedToUserId;
        issue.Priority = request.Priority;
        issue.Status = request.Status;
        issue.StartDate = request.StartDate;
        issue.EndDate = request.EndDate;
        issue.Deadline = request.Deadline;
        issue.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Load navigation properties again just in case they changed
        await _context.Entry(issue).Reference(i => i.Project).LoadAsync();
        if (issue.AssignedToUserId.HasValue)
        {
            await _context.Entry(issue).Reference(i => i.AssignedToUser).LoadAsync();
        }
        else
        {
            issue.AssignedToUser = null;
        }

        return MapToResponse(issue);
    }

    public async Task<IssueResponse?> UpdatePriorityAsync(int id, Priority priority, int currentUserId)
    {
        var issue = await _context.Issues
            .Include(i => i.Project)
            .Include(i => i.AssignedToUser)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null) return null;

        await _auditService.LogChangeAsync(id, currentUserId, "Priority", issue.Priority.ToString(), priority.ToString());

        issue.Priority = priority;
        issue.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToResponse(issue);
    }

    public async Task<IssueResponse?> UpdateStatusAsync(int id, Status status, int? assignedToUserId, int currentUserId)
    {
        var issue = await _context.Issues
            .Include(i => i.Project)
            .Include(i => i.AssignedToUser)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null) return null;

        await _auditService.LogChangeAsync(id, currentUserId, "Status", issue.Status.ToString(), status.ToString());
        if (assignedToUserId.HasValue || issue.AssignedToUserId.HasValue)
        {
            await _auditService.LogChangeAsync(id, currentUserId, "AssignedUser", issue.AssignedToUserId?.ToString(), assignedToUserId?.ToString());
        }

        issue.Status = status;
        if (assignedToUserId.HasValue)
        {
            issue.AssignedToUserId = assignedToUserId;
        }
        issue.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToResponse(issue);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var issue = await _context.Issues.FirstOrDefaultAsync(i => i.Id == id);
        if (issue == null) return false;

        _context.Issues.Remove(issue);
        await _context.SaveChangesAsync();
        return true;
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
