namespace FixtureForge.Contracts.Queries;

public record QueryExecutionResult(
    Guid Id,
    string QueryType,
    string QueryFingerprint,
    string FingerprintPolicyVersion,
    string MaskedQueryText,
    IReadOnlyList<string> ColumnNames,
    IReadOnlyList<IReadOnlyList<string?>> Rows,
    int RowCount,
    int RowsAffected,
    long DurationMs,
    string Status,
    string? ErrorMessage);