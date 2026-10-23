namespace FixtureForge.Domain.Entities;

public class GenerationJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int GeneratedRowCount { get; set; }
    public string? ErrorSummary { get; set; }
    public int RandomSeed { get; set; }
    public string SchemaVersionTag { get; set; } = "v1";
    public string RuleVersionTag { get; set; } = "v1";
    public string GeneratorVersionTag { get; set; } = "v1";
    public string? SnapshotJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
    public ICollection<GenerationJobTable> Tables { get; set; } = new List<GenerationJobTable>();
}