using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Application.Services;

public interface ICommentService
{
    Task<CommentResponse> CreateAsync(int issueId, CommentRequest request, int userId);
    Task<bool> DeleteAsync(int id);
}

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CommentResponse> CreateAsync(int issueId, CommentRequest request, int userId)
    {
        var comment = new Comment
        {
            IssueId = issueId,
            UserId = userId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Load navigations
        await _context.Entry(comment).Reference(c => c.User).LoadAsync();

        return new CommentResponse(
            comment.Id,
            comment.IssueId,
            comment.UserId,
            comment.User.Username,
            comment.Content,
            comment.CreatedAt
        );
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null) return false;

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }
}
