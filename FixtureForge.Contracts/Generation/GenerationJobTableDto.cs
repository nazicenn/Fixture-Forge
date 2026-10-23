namespace FixtureForge.Contracts.Generation;

public record GenerationJobTableDto(
    Guid Id,
    Guid GenerationJobId,
    Guid SchemaTableId,
    string SchemaTableName,
    int TargetRowCount,
    int GeneratedRowCount,
    string Status,
    string? ErrorSummary);