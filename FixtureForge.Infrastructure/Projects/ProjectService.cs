using FixtureForge.Application.Projects;
using FixtureForge.Contracts.Projects;
using FixtureForge.Domain.Entities;
using FixtureForge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Infrastructure.Projects;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt, p.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return p is null ? null : new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt, p.UpdatedAt);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync(ct);

        return new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt, project.UpdatedAt);
    }

    public async Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken ct = default)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (project is null) return null;

        project.Name = request.Name.Trim();
        project.Description = request.Description?.Trim();
        project.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt, project.UpdatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (project is null) return false;

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}