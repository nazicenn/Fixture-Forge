using FixtureForge.Contracts.Schema;

namespace FixtureForge.Application.Schema;

public interface IRelationshipService
{
    Task<IReadOnlyList<RelationshipDto>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<RelationshipDto> CreateAsync(Guid projectId, CreateRelationshipRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}