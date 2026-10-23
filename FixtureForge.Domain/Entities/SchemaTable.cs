namespace FixtureForge.Domain.Entities;

public class SchemaTable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Project Project { get; set; } = null!;
    public ICollection<SchemaColumn> Columns { get; set; } = new List<SchemaColumn>();
    public ICollection<SchemaRelationship> SourceRelationships { get; set; } = new List<SchemaRelationship>();
    public ICollection<SchemaRelationship> TargetRelationships { get; set; } = new List<SchemaRelationship>();
}