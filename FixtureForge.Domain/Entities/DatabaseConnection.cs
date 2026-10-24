namespace FixtureForge.Domain.Entities;

public class DatabaseConnection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = "SqlServer";
    public string ServerReference { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string AuthenticationType { get; set; } = "SqlAuth";
    public string EncryptedConnectionString { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<QueryExecution> Queries { get; set; } = new List<QueryExecution>();
    public Project Project { get; set; } = null!;
}