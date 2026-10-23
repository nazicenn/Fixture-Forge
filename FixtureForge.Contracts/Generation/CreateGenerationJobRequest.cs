namespace FixtureForge.Contracts.Generation;

public record CreateGenerationJobRequest(
    int RandomSeed,
    IReadOnlyList<GenerationJobTableTarget> Tables);

public record GenerationJobTableTarget(
    Guid SchemaTableId,
    int TargetRowCount);