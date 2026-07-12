using System.Security.Claims;
using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoProjetos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost("issue/{issueId:int}")]
    public async Task<IActionResult> Create(int issueId, [FromBody] CommentRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var comment = await _commentService.CreateAsync(issueId, request, currentUserId);
        return Created(string.Empty, comment);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _commentService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Comentário com ID {id} não encontrado." });
        }
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }
}
