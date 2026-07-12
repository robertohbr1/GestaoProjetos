using GestaoProjetos.Api.Application.DTOs;
using GestaoProjetos.Api.Domain.Entities;
using GestaoProjetos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Application.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponse>> GetAllAsync(bool onlyActive = false);
    Task<ProjectResponse?> GetByIdAsync(int id);
    Task<ProjectResponse> CreateAsync(ProjectRequest request);
    Task<ProjectResponse?> UpdateAsync(int id, ProjectRequest request);
    Task<bool> DeleteAsync(int id);
}

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllAsync(bool onlyActive = false)
    {
        var query = _context.Projects.AsNoTracking();
        
        if (onlyActive)
        {
            query = query.Where(p => p.IsActive);
        }

        var projects = await query.ToListAsync();
        return projects.Select(p => MapToResponse(p));
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        var project = await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return project != null ? MapToResponse(project) : null;
    }

    public async Task<ProjectResponse> CreateAsync(ProjectRequest request)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return MapToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateAsync(int id, ProjectRequest request)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return null;

        project.Name = request.Name;
        project.Description = request.Description;
        project.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return MapToResponse(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ProjectResponse MapToResponse(Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.IsActive,
            project.CreatedAt
        );
    }
}
