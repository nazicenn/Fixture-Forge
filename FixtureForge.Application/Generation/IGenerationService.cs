using FixtureForge.Contracts.Generation;

namespace FixtureForge.Application.Generation;

public interface IGenerationService
{
    Task<IReadOnlyList<GenerationJobDto>> GetJobsAsync(Guid projectId, CancellationToken ct = default);
    Task<GenerationJobDto?> GetJobAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GenerationJobTableDto>> GetJobTablesAsync(Guid jobId, CancellationToken ct = default);
    Task<GenerationJobDto> CreateAsync(Guid projectId, CreateGenerationJobRequest request, CancellationToken ct = default);
    Task<bool> CancelAsync(Guid id, CancellationToken ct = default);
}