using FixtureForge.Application.Generation;
using FixtureForge.Contracts.Generation;
using FixtureForge.Domain.Entities;
using FixtureForge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Infrastructure.Generation;

public class GenerationService : IGenerationService
{
    private readonly AppDbContext _db;
    public GenerationService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<GenerationJobDto>> GetJobsAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _db.GenerationJobs
            .AsNoTracking()
            .Where(j => j.ProjectId == projectId)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => MapJob(j))
            .ToListAsync(ct);
    }

    public async Task<GenerationJobDto?> GetJobAsync(Guid id, CancellationToken ct = default)
    {
        var job = await _db.GenerationJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id, ct);
        return job is null ? null : MapJob(job);
    }

    public async Task<IReadOnlyList<GenerationJobTableDto>> GetJobTablesAsync(Guid jobId, CancellationToken ct = default)
    {
        return await _db.GenerationJobTables
            .AsNoTracking()
            .Where(t => t.GenerationJobId == jobId)
            .OrderBy(t => t.SchemaTable.Name)
            .Select(t => new GenerationJobTableDto(
                t.Id, t.GenerationJobId, t.SchemaTableId, t.SchemaTable.Name,
                t.TargetRowCount, t.GeneratedRowCount, t.Status, t.ErrorSummary))
            .ToListAsync(ct);
    }

    public async Task<GenerationJobDto> CreateAsync(Guid projectId, CreateGenerationJobRequest request, CancellationToken ct = default)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId, ct);
        if (project is null) throw new InvalidOperationException("Proje bulunamadı.");

        // Tablolar gerçekten projeye mi ait?
        var tableIds = request.Tables.Select(t => t.SchemaTableId).ToList();
        var validTables = await _db.SchemaTables
            .Where(t => t.ProjectId == projectId && tableIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(ct);

        if (validTables.Count != tableIds.Distinct().Count())
            throw new InvalidOperationException("Bazı tablolar projeye ait değil veya mevcut değil.");

        var job = new GenerationJob
        {
            ProjectId = projectId,
            Status = "Pending",
            RandomSeed = request.RandomSeed,
            SchemaVersionTag = $"v{project.SchemaVersion}",
            RuleVersionTag = "v1",
            GeneratorVersionTag = "v1",
            CreatedAt = DateTime.UtcNow,
            GeneratedRowCount = 0
        };

        _db.GenerationJobs.Add(job);

        foreach (var target in request.Tables)
        {
            _db.GenerationJobTables.Add(new GenerationJobTable
            {
                GenerationJobId = job.Id,
                SchemaTableId = target.SchemaTableId,
                TargetRowCount = target.TargetRowCount,
                GeneratedRowCount = 0,
                Status = "Pending"
            });
        }

        await _db.SaveChangesAsync(ct);
        return MapJob(job);
    }

    public async Task<bool> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var job = await _db.GenerationJobs.FirstOrDefaultAsync(j => j.Id == id, ct);
        if (job is null) return false;
        if (job.Status is not ("Pending" or "Running")) return false;

        job.Status = "Cancelled";
        job.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static GenerationJobDto MapJob(GenerationJob j) => new(
        j.Id, j.ProjectId, j.Status, j.StartedAt, j.CompletedAt,
        j.GeneratedRowCount, j.ErrorSummary, j.RandomSeed,
        j.SchemaVersionTag, j.RuleVersionTag, j.GeneratorVersionTag, j.CreatedAt);
}