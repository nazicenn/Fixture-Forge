namespace FixtureForge.Contracts.Connections;

public record UpdateConnectionRequest(
    string Name,
    string ServerReference,
    string DatabaseName,
    string? Username,
    string? Password);