using FixtureForge.Contracts.Projects;

namespace FixtureForge.Application.Projects;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);
    Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}