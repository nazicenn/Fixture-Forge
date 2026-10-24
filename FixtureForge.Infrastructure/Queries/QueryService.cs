using System.Data;
using System.Diagnostics;
using FixtureForge.Application.Queries;
using FixtureForge.Contracts.Queries;
using FixtureForge.Domain.Entities;
using FixtureForge.Infrastructure.Security;
using FixtureForge.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Infrastructure.Queries;

public class QueryService : IQueryService
{
    private readonly AppDbContext _db;
    private readonly ConnectionStringProtector _protector;

    public QueryService(AppDbContext db, ConnectionStringProtector protector)
    {
        _db = db;
        _protector = protector;
    }

    public async Task<QueryExecutionResult> ExecuteAsync(ExecuteQueryRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.QueryText))
            throw new InvalidOperationException("Sorgu metni boş olamaz.");

        var connection = await _db.DatabaseConnections.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ConnectionId, ct)
            ?? throw new InvalidOperationException("Bağlantı bulunamadı.");

        var plainConnectionString = _protector.Unprotect(connection.EncryptedConnectionString);

        var masked = QueryMasker.Mask(request.QueryText);
        var queryType = QueryClassifier.Classify(request.QueryText);
        var fingerprint = QueryFingerprint.Compute(masked);
        var timeout = request.TimeoutSeconds ?? 30;

        var execution = new QueryExecution
        {
            ConnectionId = connection.Id,
            QueryFingerprint = fingerprint,
            FingerprintPolicyVersion = QueryFingerprint.PolicyVersion,
            QueryText = masked,
            QueryType = queryType,
            StartedAt = DateTime.UtcNow,
            Status = "Running",
            CreatedAt = DateTime.UtcNow
        };

        _db.QueryExecutions.Add(execution);
        await _db.SaveChangesAsync(ct);

        var sw = Stopwatch.StartNew();
        try
        {
            await using var sqlConn = new SqlConnection(plainConnectionString);
            await sqlConn.OpenAsync(ct);

            await using var cmd = new SqlCommand(request.QueryText, sqlConn)
            {
                CommandTimeout = timeout
            };

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columnNames.Add(reader.GetName(i));

            var rows = new List<IReadOnlyList<string?>>();
            int rowCount = 0;
            const int MaxRows = 1000;   // güvenlik sınırı

            while (await reader.ReadAsync(ct) && rowCount < MaxRows)
            {
                var row = new string?[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader.IsDBNull(i) ? null : reader.GetValue(i)?.ToString();
                }
                rows.Add(row);
                rowCount++;
            }

            sw.Stop();
            execution.CompletedAt = DateTime.UtcNow;
            execution.DurationMs = sw.ElapsedMilliseconds;
            execution.RowCount = rowCount;
            execution.Status = "Succeeded";

            await _db.SaveChangesAsync(ct);

            return new QueryExecutionResult(
                execution.Id, queryType, fingerprint, QueryFingerprint.PolicyVersion,
                masked, columnNames, rows, rowCount, 0, execution.DurationMs,
                "Succeeded", null);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            execution.CompletedAt = DateTime.UtcNow;
            execution.DurationMs = sw.ElapsedMilliseconds;
            execution.Status = "Cancelled";
            execution.ErrorMessage = "Sorgu iptal edildi.";
            await _db.SaveChangesAsync(CancellationToken.None);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            execution.CompletedAt = DateTime.UtcNow;
            execution.DurationMs = sw.ElapsedMilliseconds;
            execution.Status = "Failed";
            execution.ErrorMessage = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message;

            await _db.SaveChangesAsync(CancellationToken.None);

            return new QueryExecutionResult(
                execution.Id, queryType, fingerprint, QueryFingerprint.PolicyVersion,
                masked, Array.Empty<string>(), Array.Empty<IReadOnlyList<string?>>(),
                0, 0, execution.DurationMs, "Failed", execution.ErrorMessage);
        }
    }

    public async Task<IReadOnlyList<QueryHistoryDto>> GetHistoryAsync(Guid connectionId, CancellationToken ct = default)
    {
        return await _db.QueryExecutions
            .AsNoTracking()
            .Where(q => q.ConnectionId == connectionId)
            .OrderByDescending(q => q.CreatedAt)
            .Take(100)
            .Select(q => new QueryHistoryDto(
                q.Id, q.ConnectionId, q.QueryType, q.QueryFingerprint, q.QueryText,
                q.DurationMs, q.RowCount, q.RowsAffected, q.Status, q.StartedAt, q.CompletedAt))
            .ToListAsync(ct);
    }

    public async Task<QueryHistoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var q = await _db.QueryExecutions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (q is null) return null;
        return new QueryHistoryDto(
            q.Id, q.ConnectionId, q.QueryType, q.QueryFingerprint, q.QueryText,
            q.DurationMs, q.RowCount, q.RowsAffected, q.Status, q.StartedAt, q.CompletedAt);
    }
}