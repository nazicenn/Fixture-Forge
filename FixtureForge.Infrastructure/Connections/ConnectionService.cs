using System.Diagnostics;
using FixtureForge.Application.Connections;
using FixtureForge.Contracts.Connections;
using FixtureForge.Domain.Entities;
using FixtureForge.Infrastructure.Security;
using FixtureForge.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Infrastructure.Connections;

public class ConnectionService : IConnectionService
{
    private readonly AppDbContext _db;
    private readonly ConnectionStringProtector _protector;

    public ConnectionService(AppDbContext db, ConnectionStringProtector protector)
    {
        _db = db;
        _protector = protector;
    }

    public async Task<IReadOnlyList<ConnectionDto>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _db.DatabaseConnections
            .AsNoTracking()
            .Where(c => c.ProjectId == projectId)
            .OrderBy(c => c.Name)
            .Select(c => Map(c))
            .ToListAsync(ct);
    }

    public async Task<ConnectionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var c = await _db.DatabaseConnections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return c is null ? null : Map(c);
    }

    public async Task<ConnectionDto> CreateAsync(Guid projectId, CreateConnectionRequest request, CancellationToken ct = default)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId, ct);
        if (!projectExists) throw new InvalidOperationException("Proje bulunamadı.");

        var plainConnectionString = BuildConnectionString(
            request.ServerReference,
            request.DatabaseName,
            request.Username,
            request.Password);

        var connection = new DatabaseConnection
        {
            ProjectId = projectId,
            Name = request.Name.Trim(),
            Provider = request.Provider,
            ServerReference = request.ServerReference.Trim(),
            DatabaseName = request.DatabaseName.Trim(),
            AuthenticationType = request.AuthenticationType,
            EncryptedConnectionString = _protector.Protect(plainConnectionString),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.DatabaseConnections.Add(connection);
        await _db.SaveChangesAsync(ct);
        return Map(connection);
    }

    public async Task<ConnectionDto?> UpdateAsync(Guid id, UpdateConnectionRequest request, CancellationToken ct = default)
    {
        var connection = await _db.DatabaseConnections.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (connection is null) return null;

        var plainConnectionString = BuildConnectionString(
            request.ServerReference,
            request.DatabaseName,
            request.Username,
            request.Password);

        connection.Name = request.Name.Trim();
        connection.ServerReference = request.ServerReference.Trim();
        connection.DatabaseName = request.DatabaseName.Trim();
        connection.EncryptedConnectionString = _protector.Protect(plainConnectionString);
        connection.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Map(connection);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var connection = await _db.DatabaseConnections.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (connection is null) return false;

        _db.DatabaseConnections.Remove(connection);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ConnectionTestResult> TestAsync(Guid id, CancellationToken ct = default)
    {
        var connection = await _db.DatabaseConnections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (connection is null) throw new InvalidOperationException("Bağlantı bulunamadı.");

        string plainConnectionString;
        try
        {
            plainConnectionString = _protector.Unprotect(connection.EncryptedConnectionString);
        }
        catch
        {
            return new ConnectionTestResult(false, 0, "Bağlantı dizesi çözülemedi.");
        }

        var sw = Stopwatch.StartNew();
        try
        {
            await using var sqlConnection = new SqlConnection(plainConnectionString);
            await sqlConnection.OpenAsync(ct);

            await using var cmd = new SqlCommand("SELECT 1", sqlConnection)
            {
                CommandTimeout = 5
            };
            await cmd.ExecuteScalarAsync(ct);
            sw.Stop();

            return new ConnectionTestResult(true, sw.ElapsedMilliseconds, null);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new ConnectionTestResult(false, sw.ElapsedMilliseconds, SanitizeError(ex.Message));
        }
    }

    private static string BuildConnectionString(string server, string database, string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return $"Server={server};Database={database};Integrated Security=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=5";
        }
        return $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate=True;Encrypt=False;Connect Timeout=5";
    }

    private static ConnectionDto Map(DatabaseConnection c) => new(
        c.Id, c.ProjectId, c.Name, c.Provider, c.ServerReference,
        c.DatabaseName, c.AuthenticationType, c.CreatedAt, c.UpdatedAt);

    private static string SanitizeError(string error)
    {
        if (error.Length > 500) return error[..500];
        return error;
    }
}