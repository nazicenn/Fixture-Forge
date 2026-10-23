namespace FixtureForge.Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SchemaVersion { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<SchemaTable> Tables { get; set; } = new List<SchemaTable>();
    public ICollection<SchemaRelationship> Relationships { get; set; } = new List<SchemaRelationship>();
    public ICollection<GenerationJob> GenerationJobs { get; set; } = new List<GenerationJob>();
}