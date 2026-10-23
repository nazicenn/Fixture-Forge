namespace FixtureForge.Domain.Entities;

public class SchemaRelationship
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Guid SourceTableId { get; set; }
    public Guid SourceColumnId { get; set; }
    public Guid TargetTableId { get; set; }
    public Guid TargetColumnId { get; set; }
    public string RelationshipType { get; set; } = "FK";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
    public SchemaTable SourceTable { get; set; } = null!;
    public SchemaColumn SourceColumn { get; set; } = null!;
    public SchemaTable TargetTable { get; set; } = null!;
    public SchemaColumn TargetColumn { get; set; } = null!;
}