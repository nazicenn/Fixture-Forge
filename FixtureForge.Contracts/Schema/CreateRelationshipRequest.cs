namespace FixtureForge.Contracts.Schema;

public record CreateRelationshipRequest(
    Guid SourceTableId,
    Guid SourceColumnId,
    Guid TargetTableId,
    Guid TargetColumnId,
    string RelationshipType = "FK");