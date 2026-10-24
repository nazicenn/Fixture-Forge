using FixtureForge.Contracts.Connections;

namespace FixtureForge.Application.Connections;

public interface IConnectionService
{
    Task<IReadOnlyList<ConnectionDto>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<ConnectionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ConnectionDto> CreateAsync(Guid projectId, CreateConnectionRequest request, CancellationToken ct = default);
    Task<ConnectionDto?> UpdateAsync(Guid id, UpdateConnectionRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<ConnectionTestResult> TestAsync(Guid id, CancellationToken ct = default);
}