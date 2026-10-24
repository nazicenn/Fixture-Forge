namespace FixtureForge.Contracts.Connections;

public record ConnectionDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Provider,
    string ServerReference,
    string DatabaseName,
    string AuthenticationType,
    DateTime CreatedAt,
    DateTime UpdatedAt);