using FixtureForge.Contracts.Schema;

namespace FixtureForge.Application.Schema;

public interface ITableService
{
    Task<IReadOnlyList<TableDto>> GetTablesAsync(Guid projectId, CancellationToken ct = default);
    Task<TableDto?> GetTableAsync(Guid id, CancellationToken ct = default);
    Task<TableDto> CreateTableAsync(Guid projectId, CreateTableRequest request, CancellationToken ct = default);
    Task<TableDto?> UpdateTableAsync(Guid id, UpdateTableRequest request, CancellationToken ct = default);
    Task<bool> DeleteTableAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<ColumnDto>> GetColumnsAsync(Guid tableId, CancellationToken ct = default);
    Task<ColumnDto?> GetColumnAsync(Guid id, CancellationToken ct = default);
    Task<ColumnDto> CreateColumnAsync(Guid tableId, CreateColumnRequest request, CancellationToken ct = default);
    Task<ColumnDto?> UpdateColumnAsync(Guid id, UpdateColumnRequest request, CancellationToken ct = default);
    Task<bool> DeleteColumnAsync(Guid id, CancellationToken ct = default);
}