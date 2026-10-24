using FixtureForge.Contracts.Queries;

namespace FixtureForge.Application.Queries;

public interface IQueryService
{
    Task<QueryExecutionResult> ExecuteAsync(ExecuteQueryRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<QueryHistoryDto>> GetHistoryAsync(Guid connectionId, CancellationToken ct = default);
    Task<QueryHistoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}