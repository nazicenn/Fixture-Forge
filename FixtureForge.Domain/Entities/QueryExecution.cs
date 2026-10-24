namespace FixtureForge.Domain.Entities;

public class QueryExecution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConnectionId { get; set; }
    public Guid? ScenarioRunId { get; set; }
    public string QueryFingerprint { get; set; } = string.Empty;
    public string FingerprintPolicyVersion { get; set; } = "v1";
    public string QueryText { get; set; } = string.Empty;       // maskelenmiş
    public string QueryType { get; set; } = "SELECT";
    public string? ParameterSummary { get; set; }               // JSON
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long DurationMs { get; set; }
    public long? ServerDurationMs { get; set; }
    public int RowCount { get; set; }
    public int RowsAffected { get; set; }
    public string Status { get; set; } = "Succeeded";           // Succeeded, Failed, Cancelled, TimedOut
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DatabaseConnection Connection { get; set; } = null!;
}