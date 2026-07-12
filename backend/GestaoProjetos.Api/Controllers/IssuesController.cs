using System.Security.Claims;
using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Application.Services;
using GestaoProjetos.Api.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoProjetos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly IIssueService _issueService;

    public IssuesController(IIssueService issueService)
    {
        _issueService = issueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFiltered(
        [FromQuery] ImplementationType? implementationType,
        [FromQuery] int? projectId,
        [FromQuery] Status? status,
        [FromQuery] int? assignedToUserId,
        [FromQuery] Priority? priority,
        [FromQuery] string? searchTerm
    )
    {
        // Require implementationType filter as requested by prompt
        if (!implementationType.HasValue)
        {
            return BadRequest(new { message = "O filtro por 'ImplementationType' é obrigatório." });
        }

        var issues = await _issueService.GetFilteredAsync(
            implementationType, projectId, status, assignedToUserId, priority, searchTerm
        );
        return Ok(issues);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var issue = await _issueService.GetByIdAsync(id);
        if (issue == null)
        {
            return NotFound(new { message = $"Demanda com ID {id} não encontrada." });
        }
        return Ok(issue);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IssueRequest request)
    {
        var issue = await _issueService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = issue.Id }, issue);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] IssueRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var issue = await _issueService.UpdateAsync(id, request, currentUserId);
        if (issue == null)
        {
            return NotFound(new { message = $"Demanda com ID {id} não encontrada." });
        }
        return Ok(issue);
    }

    [HttpPatch("{id:int}/priority")]
    public async Task<IActionResult> UpdatePriority(int id, [FromBody] UpdatePriorityRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var issue = await _issueService.UpdatePriorityAsync(id, request.Priority, currentUserId);
        if (issue == null)
        {
            return NotFound(new { message = $"Demanda com ID {id} não encontrada." });
        }
        return Ok(issue);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var issue = await _issueService.UpdateStatusAsync(id, request.Status, request.AssignedToUserId, currentUserId);
        if (issue == null)
        {
            return NotFound(new { message = $"Demanda com ID {id} não encontrada." });
        }
        return Ok(issue);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _issueService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Demanda com ID {id} não encontrada." });
        }
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }
}
