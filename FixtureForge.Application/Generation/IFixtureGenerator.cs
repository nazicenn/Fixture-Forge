namespace FixtureForge.Application.Generation;

public interface IFixtureGenerator
{
    Task RunAsync(Guid jobId, CancellationToken ct = default);
    Task<string?> ExportSqlAsync(Guid jobId, CancellationToken ct = default);
}