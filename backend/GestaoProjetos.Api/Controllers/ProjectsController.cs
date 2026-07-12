using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoProjetos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
    {
        var projects = await _projectService.GetAllAsync(onlyActive);
        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = $"Projeto com ID {id} não encontrado." });
        }
        return Ok(project);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectRequest request)
    {
        var project = await _projectService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProjectRequest request)
    {
        var project = await _projectService.UpdateAsync(id, request);
        if (project == null)
        {
            return NotFound(new { message = $"Projeto com ID {id} não encontrado." });
        }
        return Ok(project);
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _projectService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Projeto com ID {id} não encontrado." });
        }
        return NoContent();
    }
}
