using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Infrastructure.Data;

namespace GestaoProjetos.Api.Application.Services;

public interface IAuditService
{
    Task LogChangeAsync(int issueId, int userId, string field, string? oldValue, string? newValue);
}

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogChangeAsync(int issueId, int userId, string field, string? oldValue, string? newValue)
    {
        // Don't log if values are identical
        if (oldValue == newValue) return;

        var auditLog = new AuditLog
        {
            IssueId = issueId,
            UserId = userId,
            FieldChanged = field,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}
