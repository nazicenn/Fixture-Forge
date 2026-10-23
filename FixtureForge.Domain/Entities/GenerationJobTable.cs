namespace FixtureForge.Domain.Entities;

public class GenerationJobTable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GenerationJobId { get; set; }
    public Guid SchemaTableId { get; set; }
    public int TargetRowCount { get; set; }
    public int GeneratedRowCount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ErrorSummary { get; set; }

    public GenerationJob GenerationJob { get; set; } = null!;
    public SchemaTable SchemaTable { get; set; } = null!;
}