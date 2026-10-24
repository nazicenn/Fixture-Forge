namespace FixtureForge.Contracts.Queries;

public record ExecuteQueryRequest(
    Guid ConnectionId,
    string QueryText,
    int? TimeoutSeconds = null);