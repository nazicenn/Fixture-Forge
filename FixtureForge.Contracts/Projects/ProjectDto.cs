namespace FixtureForge.Contracts.Projects;

public record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    int SchemaVersion,
    DateTime CreatedAt,
    DateTime UpdatedAt);