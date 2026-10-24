namespace FixtureForge.Contracts.Connections;

public record CreateConnectionRequest(
    string Name,
    string ServerReference,
    string DatabaseName,
    string? Username,
    string? Password,
    string Provider = "SqlServer",
    string AuthenticationType = "SqlAuth");