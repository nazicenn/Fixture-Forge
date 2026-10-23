namespace FixtureForge.Contracts.Schema;

public record TableDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);