using GestaoProjetos.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoProjetos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;

    public AttachmentsController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    [HttpPost("issue/{issueId:int}")]
    public async Task<IActionResult> Upload(int issueId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Nenhum arquivo enviado." });
        }

        var uploadedBy = User.Identity?.Name ?? "Sistema";
        using (var stream = file.OpenReadStream())
        {
            var response = await _attachmentService.UploadAsync(issueId, file.FileName, stream, uploadedBy);
            return Created(string.Empty, response);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        var attachment = await _attachmentService.GetByIdAsync(id);
        if (attachment == null)
        {
            return NotFound(new { message = $"Anexo com ID {id} não encontrado." });
        }

        var relativePath = attachment.FilePath.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound(new { message = "Arquivo físico não encontrado no servidor." });
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(fullPath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;

        var contentType = GetContentType(fullPath);
        return File(memory, contentType, attachment.FileName);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _attachmentService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Anexo com ID {id} não encontrado." });
        }
        return NoContent();
    }

    private static string GetContentType(string path)
    {
        var types = new Dictionary<string, string>
        {
            {".pdf", "application/pdf"},
            {".doc", "application/msword"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".txt", "text/plain"}
        };

        var ext = Path.GetExtension(path).ToLowerInvariant();
        return types.TryGetValue(ext, out var type) ? type : "application/octet-stream";
    }
}
