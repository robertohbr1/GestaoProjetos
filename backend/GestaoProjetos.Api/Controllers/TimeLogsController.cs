using System.Security.Claims;
using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoProjetos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TimeLogsController : ControllerBase
{
    private readonly ITimeLogService _timeLogService;

    public TimeLogsController(ITimeLogService timeLogService)
    {
        _timeLogService = timeLogService;
    }

    [HttpGet("issue/{issueId:int}")]
    public async Task<IActionResult> GetByIssue(int issueId)
    {
        var logs = await _timeLogService.GetByIssueIdAsync(issueId);
        return Ok(logs);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TimeLogRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var log = await _timeLogService.CreateAsync(request, currentUserId);
        return Created(string.Empty, log);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _timeLogService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Apontamento com ID {id} não encontrado." });
        }
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }
}
