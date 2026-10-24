using System.Text;
using FixtureForge.Application.Generation;
using FixtureForge.Domain.Entities;
using FixtureForge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Infrastructure.Generation;

public class FixtureGenerator : IFixtureGenerator
{
    private readonly AppDbContext _db;
    public FixtureGenerator(AppDbContext db) => _db = db;

    public async Task RunAsync(Guid jobId, CancellationToken ct = default)
    {
        var job = await _db.GenerationJobs
            .Include(j => j.Tables)
            .FirstOrDefaultAsync(j => j.Id == jobId, ct)
            ?? throw new InvalidOperationException("İş bulunamadı.");

        if (job.Status != "Pending")
            throw new InvalidOperationException("Sadece 'Pending' durumdaki işler çalıştırılabilir.");

        job.Status = "Running";
        job.StartedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        try
        {
            var project = await _db.Projects.AsNoTracking().FirstAsync(p => p.Id == job.ProjectId, ct);
            var tables = await _db.SchemaTables
                .Include(t => t.Columns)
                .Where(t => t.ProjectId == job.ProjectId)
                .ToListAsync(ct);

            var relationships = await _db.SchemaRelationships
                .Where(r => r.ProjectId == job.ProjectId)
                .ToListAsync(ct);

            var targets = job.Tables.ToDictionary(t => t.SchemaTableId, t => t.TargetRowCount);
            var rng = new Random(job.RandomSeed);

            job.SnapshotJson = BuildSnapshot(job, project, tables, relationships);

            var orderedTables = TopologicalSort(tables, relationships, targets.Keys);
            if (orderedTables is null)
            {
                job.Status = "Failed";
                job.ErrorSummary = "Döngüsel yabancı anahtar ilişkisi tespit edildi.";
                job.CompletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
                return;
            }

            var generatedKeys = new Dictionary<Guid, List<object>>();

            foreach (var table in orderedTables)
            {
                var target = targets[table.Id];
                if (target <= 0)
                {
                    MarkTable(job, table.Id, "Skipped", 0, "Hedef kayıt sayısı 0.");
                    continue;
                }

                // ✅ YENİ: Sütunsuz tablo → atla
                if (table.Columns.Count == 0)
                {
                    MarkTable(job, table.Id, "Skipped", 0, "Tabloda sütun tanımı yok.");
                    continue;
                }

                var pkColumn = table.Columns.FirstOrDefault(c => c.IsPrimaryKey);
                var fkColumns = relationships.Where(r => r.SourceTableId == table.Id).ToList();

                // ✅ YENİ: PK'sız + FK varsa → hata
                if (pkColumn is null && fkColumns.Any())
                {
                    throw new InvalidOperationException(
                        $"{table.Name} tablosunda birincil anahtar yok; FK üretimi için PK gerekli.");
                }

                var keys = new List<object>();

                for (int i = 0; i < target; i++)
                {
                    var values = new Dictionary<Guid, object?>();

                    foreach (var col in table.Columns)
                    {
                        var fk = fkColumns.FirstOrDefault(f => f.SourceColumnId == col.Id);
                        if (fk is not null)
                        {
                            // ✅ YENİ: FK hedefinde PK değeri yoksa → net hata
                            if (!generatedKeys.TryGetValue(fk.TargetTableId, out var targetKeys) || targetKeys.Count == 0)
                            {
                                throw new InvalidOperationException(
                                    $"{table.Name}.{col.Name} için hedef tabloda PK değeri bulunamadı. " +
                                    $"Hedef tabloya PK sütunu ekleyin veya hedef tabloya veri üretin.");
                            }
                            values[col.Id] = targetKeys[rng.Next(targetKeys.Count)];
                        }
                        else
                        {
                            values[col.Id] = GenerateValue(col, rng);
                        }
                    }

                    if (pkColumn is not null && values.TryGetValue(pkColumn.Id, out var pkVal) && pkVal is not null)
                        keys.Add(pkVal);
                }

                if (pkColumn is not null)
                    generatedKeys[table.Id] = keys;

                MarkTable(job, table.Id, "Succeeded", target, null);
                job.GeneratedRowCount += target;
            }

            job.Status = "Succeeded";
            job.CompletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            job.Status = "Failed";
            job.ErrorSummary = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            throw;
        }
    }

    public async Task<string?> ExportSqlAsync(Guid jobId, CancellationToken ct = default)
    {
        var job = await _db.GenerationJobs
            .Include(j => j.Tables).ThenInclude(t => t.SchemaTable).ThenInclude(t => t.Columns)
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);
        if (job is null) return null;
        if (job.Status != "Succeeded") return null;

        return await RegenerateSqlAsync(job, ct);
    }

    private async Task<string?> RegenerateSqlAsync(GenerationJob job, CancellationToken ct)
    {
        var project = await _db.Projects.AsNoTracking().FirstAsync(p => p.Id == job.ProjectId, ct);
        var tables = await _db.SchemaTables.Include(t => t.Columns)
            .Where(t => t.ProjectId == job.ProjectId).ToListAsync(ct);
        var relationships = await _db.SchemaRelationships
            .Where(r => r.ProjectId == job.ProjectId).ToListAsync(ct);
        var targets = job.Tables.ToDictionary(t => t.SchemaTableId, t => t.TargetRowCount);
        var rng = new Random(job.RandomSeed);

        var sb = new StringBuilder();
        sb.AppendLine($"-- Generated by FixtureForge");
        sb.AppendLine($"-- Job: {job.Id}");
        sb.AppendLine($"-- Seed: {job.RandomSeed}");
        sb.AppendLine($"-- SchemaVersion: {job.SchemaVersionTag}");
        sb.AppendLine($"-- GeneratorVersion: {job.GeneratorVersionTag}");
        sb.AppendLine();

        var ordered = TopologicalSort(tables, relationships, targets.Keys);
        if (ordered is null) return null;

        var generatedKeys = new Dictionary<Guid, List<object>>();

        foreach (var table in ordered)
        {
            var target = targets[table.Id];
            if (target <= 0) continue;
            if (table.Columns.Count == 0) continue;   // ✅ YENİ: sütunsuz tabloyu atla

            sb.AppendLine($"-- Table: {table.Name} ({target} rows)");

            var pkColumn = table.Columns.FirstOrDefault(c => c.IsPrimaryKey);
            var fkColumns = relationships.Where(r => r.SourceTableId == table.Id).ToList();
            var keys = new List<object>();

            for (int i = 0; i < target; i++)
            {
                var values = new Dictionary<Guid, object?>();
                foreach (var col in table.Columns)
                {
                    var fk = fkColumns.FirstOrDefault(f => f.SourceColumnId == col.Id);
                    if (fk is not null)
                    {
                        if (!generatedKeys.TryGetValue(fk.TargetTableId, out var tk) || tk.Count == 0)
                        {
                            throw new InvalidOperationException(
                                $"{table.Name}.{col.Name} için hedef tabloda PK değeri bulunamadı.");
                        }
                        values[col.Id] = tk[rng.Next(tk.Count)];
                    }
                    else
                    {
                        values[col.Id] = GenerateValue(col, rng);
                    }
                }
                if (pkColumn is not null && values[pkColumn.Id] is not null)
                    keys.Add(values[pkColumn.Id]!);
                sb.AppendLine(BuildInsertStatement(table, table.Columns.ToList(), values));
            }

            if (pkColumn is not null) generatedKeys[table.Id] = keys;
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static List<SchemaTable>? TopologicalSort(
        List<SchemaTable> tables,
        List<SchemaRelationship> relationships,
        IEnumerable<Guid> targetTableIds)
    {
        var targetSet = targetTableIds.ToHashSet();
        var relevant = tables.Where(t => targetSet.Contains(t.Id)).ToList();
        var relevantIds = relevant.Select(t => t.Id).ToHashSet();

        var deps = relevant.ToDictionary(
            t => t.Id,
            t => relationships
                .Where(r => r.SourceTableId == t.Id && relevantIds.Contains(r.TargetTableId))
                .Select(r => r.TargetTableId)
                .Distinct()
                .ToList());

        var result = new List<SchemaTable>();
        var visited = new HashSet<Guid>();
        var visiting = new HashSet<Guid>();

        bool Visit(Guid id)
        {
            if (visited.Contains(id)) return true;
            if (!visiting.Add(id)) return false;
            foreach (var dep in deps[id])
                if (!Visit(dep)) return false;
            visiting.Remove(id);
            visited.Add(id);
            result.Add(relevant.First(t => t.Id == id));
            return true;
        }

        foreach (var t in relevant)
            if (!Visit(t.Id)) return null;

        return result;
    }

    private static object? GenerateValue(SchemaColumn col, Random rng)
    {
        var type = col.DataType.ToLowerInvariant();
        return type switch
        {
            "int" => rng.Next(1, 100000),
            "bigint" => (long)rng.Next(1, 1000000),
            "smallint" => (short)rng.Next(1, 32000),
            "tinyint" => (byte)rng.Next(0, 256),
            "nvarchar" or "varchar" or "char" or "nchar" => RandomString(rng, col.MaxLength ?? 20),
            "datetime2" or "datetime" => DateTime.UtcNow.AddDays(-rng.Next(0, 730)),
            "date" => DateTime.UtcNow.AddDays(-rng.Next(0, 730)).Date,
            "time" => TimeSpan.FromSeconds(rng.Next(0, 86400)),
            "bit" => rng.Next(0, 2) == 1,
            "decimal" or "numeric" => Math.Round((decimal)(rng.NextDouble() * 10000), 2),
            "float" => rng.NextDouble() * 10000,
            "real" => (float)(rng.NextDouble() * 10000),
            "uniqueidentifier" => Guid.NewGuid(),
            "varbinary" or "binary" => new byte[] { (byte)rng.Next(0, 256) },
            _ => null
        };
    }

    private static string RandomString(Random rng, int maxLength)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var len = rng.Next(3, Math.Min(maxLength, 20) + 1);
        var sb = new StringBuilder(len);
        for (int i = 0; i < len; i++) sb.Append(chars[rng.Next(chars.Length)]);
        return sb.ToString();
    }

    private static string BuildInsertStatement(SchemaTable table, List<SchemaColumn> columns, Dictionary<Guid, object?> values)
    {
        var cols = string.Join(", ", columns.Select(c => $"[{c.Name}]"));
        var vals = string.Join(", ", columns.Select(c => FormatValue(values.GetValueOrDefault(c.Id))));
        return $"INSERT INTO [{table.Name}] ({cols}) VALUES ({vals});";
    }

    private static string FormatValue(object? value)
    {
        if (value is null) return "NULL";
        return value switch
        {
            string s => $"N'{s.Replace("'", "''")}'",
            DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
            DateTimeOffset dto => $"'{dto:yyyy-MM-dd HH:mm:ss}'",
            TimeSpan ts => $"'{ts}'",
            bool b => b ? "1" : "0",
            Guid g => $"'{g}'",
            byte[] bytes => $"0x{BitConverter.ToString(bytes).Replace("-", "")}",
            decimal d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            float f => f.ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString() ?? "NULL"
        };
    }

    private static void MarkTable(GenerationJob job, Guid tableId, string status, int generated, string? error)
    {
        var t = job.Tables.FirstOrDefault(x => x.SchemaTableId == tableId);
        if (t is null) return;
        t.Status = status;
        t.GeneratedRowCount = generated;
        t.ErrorSummary = error;
    }

    private static string BuildSnapshot(GenerationJob job, Project project, List<SchemaTable> tables, List<SchemaRelationship> relationships)
    {
        var snapshot = new
        {
            snapshotVersion = "v1",
            generationJobId = job.Id,
            schemaVersionTag = job.SchemaVersionTag,
            ruleVersionTag = job.RuleVersionTag,
            generatorVersionTag = job.GeneratorVersionTag,
            randomSeed = job.RandomSeed,
            tables = tables.Select(t => new
            {
                tableId = t.Id,
                tableName = t.Name,
                targetRowCount = job.Tables.FirstOrDefault(x => x.SchemaTableId == t.Id)?.TargetRowCount ?? 0
            }),
            relationships = relationships.Select(r => new
            {
                sourceTableId = r.SourceTableId,
                sourceColumnId = r.SourceColumnId,
                targetTableId = r.TargetTableId,
                targetColumnId = r.TargetColumnId
            })
        };
        return System.Text.Json.JsonSerializer.Serialize(snapshot, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }
}