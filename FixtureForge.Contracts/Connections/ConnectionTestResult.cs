namespace FixtureForge.Contracts.Connections;

public record ConnectionTestResult(
    bool Success,
    long DurationMs,
    string? Error);