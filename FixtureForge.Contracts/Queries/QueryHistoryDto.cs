namespace FixtureForge.Contracts.Queries;

public record QueryHistoryDto(
    Guid Id,
    Guid ConnectionId,
    string QueryType,
    string QueryFingerprint,
    string QueryText,
    long DurationMs,
    int RowCount,
    int RowsAffected,
    string Status,
    DateTime StartedAt,
    DateTime? CompletedAt);