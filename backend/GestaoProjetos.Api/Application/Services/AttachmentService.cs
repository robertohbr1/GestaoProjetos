using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Application.Services;

public interface IAttachmentService
{
    Task<AttachmentResponse> UploadAsync(int issueId, string fileName, Stream stream, string uploadedBy);
    Task<Attachment?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}

public class AttachmentService : IAttachmentService
{
    private readonly AppDbContext _context;
    private readonly string _uploadFolder;

    public AttachmentService(AppDbContext context)
    {
        _context = context;
        _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(_uploadFolder))
        {
            Directory.CreateDirectory(_uploadFolder);
        }
    }

    public async Task<AttachmentResponse> UploadAsync(int issueId, string fileName, Stream stream, string uploadedBy)
    {
        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_uploadFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(fileStream);
        }

        var attachment = new Attachment
        {
            IssueId = issueId,
            FileName = fileName,
            FilePath = $"/uploads/{uniqueFileName}", // relative URL path
            UploadedBy = uploadedBy,
            UploadedAt = DateTime.UtcNow
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();

        return new AttachmentResponse(
            attachment.Id,
            attachment.IssueId,
            attachment.FileName,
            attachment.FilePath,
            attachment.UploadedBy,
            attachment.UploadedAt
        );
    }

    public async Task<Attachment?> GetByIdAsync(int id)
    {
        return await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
        if (attachment == null) return false;

        var relativePath = attachment.FilePath.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();
        return true;
    }
}
