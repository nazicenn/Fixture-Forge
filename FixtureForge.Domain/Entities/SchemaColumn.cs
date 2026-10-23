namespace FixtureForge.Domain.Entities;

public class SchemaColumn
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SchemaTableId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; } = true;
    public bool IsPrimaryKey { get; set; }
    public bool IsUnique { get; set; }
    public string? DefaultValue { get; set; }
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public int OrdinalPosition { get; set; }
    public SchemaTable SchemaTable { get; set; } = null!;
    public ICollection<SchemaRelationship> SourceRelationships { get; set; } = new List<SchemaRelationship>();
    public ICollection<SchemaRelationship> TargetRelationships { get; set; } = new List<SchemaRelationship>();
}