namespace FixtureForge.Contracts.Generation;

public record GenerationJobDto(
    Guid Id,
    Guid ProjectId,
    string Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    int GeneratedRowCount,
    string? ErrorSummary,
    int RandomSeed,
    string SchemaVersionTag,
    string RuleVersionTag,
    string GeneratorVersionTag,
    DateTime CreatedAt);