namespace FixtureForge.Contracts.Schema;

public record RelationshipDto(
    Guid Id,
    Guid ProjectId,
    Guid SourceTableId,
    string SourceTableName,
    Guid SourceColumnId,
    string SourceColumnName,
    Guid TargetTableId,
    string TargetTableName,
    Guid TargetColumnId,
    string TargetColumnName,
    string RelationshipType,
    DateTime CreatedAt);