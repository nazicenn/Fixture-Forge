using FixtureForge.Application.Schema;
using FixtureForge.Contracts.Schema;
using FixtureForge.Domain.Entities;
using FixtureForge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Infrastructure.Schema;

public class TableService : ITableService
{
    private readonly AppDbContext _db;
    public TableService(AppDbContext db) => _db = db;

    // ===== TABLES =====
    public async Task<IReadOnlyList<TableDto>> GetTablesAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _db.SchemaTables
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Name)
            .Select(t => new TableDto(t.Id, t.ProjectId, t.Name, t.Description, t.CreatedAt, t.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<TableDto?> GetTableAsync(Guid id, CancellationToken ct = default)
    {
        var t = await _db.SchemaTables.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return t is null ? null : new TableDto(t.Id, t.ProjectId, t.Name, t.Description, t.CreatedAt, t.UpdatedAt);
    }

    public async Task<TableDto> CreateTableAsync(Guid projectId, CreateTableRequest request, CancellationToken ct = default)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId, ct);
        if (!projectExists) throw new InvalidOperationException("Proje bulunamadı.");

        var table = new SchemaTable
        {
            ProjectId = projectId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.SchemaTables.Add(table);
        await IncrementSchemaVersionAsync(projectId, ct);
        await _db.SaveChangesAsync(ct);

        return new TableDto(table.Id, table.ProjectId, table.Name, table.Description, table.CreatedAt, table.UpdatedAt);
    }

    public async Task<TableDto?> UpdateTableAsync(Guid id, UpdateTableRequest request, CancellationToken ct = default)
    {
        var table = await _db.SchemaTables.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (table is null) return null;

        table.Name = request.Name.Trim();
        table.Description = request.Description?.Trim();
        table.UpdatedAt = DateTime.UtcNow;

        await IncrementSchemaVersionAsync(table.ProjectId, ct);
        await _db.SaveChangesAsync(ct);

        return new TableDto(table.Id, table.ProjectId, table.Name, table.Description, table.CreatedAt, table.UpdatedAt);
    }

    public async Task<bool> DeleteTableAsync(Guid id, CancellationToken ct = default)
    {
        var table = await _db.SchemaTables.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (table is null) return false;

        _db.SchemaTables.Remove(table);
        await IncrementSchemaVersionAsync(table.ProjectId, ct);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ===== COLUMNS =====
    public async Task<IReadOnlyList<ColumnDto>> GetColumnsAsync(Guid tableId, CancellationToken ct = default)
    {
        return await _db.SchemaColumns
            .AsNoTracking()
            .Where(c => c.SchemaTableId == tableId)
            .OrderBy(c => c.OrdinalPosition)
            .Select(c => new ColumnDto(c.Id, c.SchemaTableId, c.Name, c.DataType,
                c.IsNullable, c.IsPrimaryKey, c.IsUnique, c.DefaultValue,
                c.MaxLength, c.Precision, c.Scale, c.OrdinalPosition))
            .ToListAsync(ct);
    }

    public async Task<ColumnDto?> GetColumnAsync(Guid id, CancellationToken ct = default)
    {
        var c = await _db.SchemaColumns.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return c is null ? null : new ColumnDto(c.Id, c.SchemaTableId, c.Name, c.DataType,
            c.IsNullable, c.IsPrimaryKey, c.IsUnique, c.DefaultValue,
            c.MaxLength, c.Precision, c.Scale, c.OrdinalPosition);
    }

    public async Task<ColumnDto> CreateColumnAsync(Guid tableId, CreateColumnRequest request, CancellationToken ct = default)
    {
        var table = await _db.SchemaTables.FirstOrDefaultAsync(t => t.Id == tableId, ct);
        if (table is null) throw new InvalidOperationException("Tablo bulunamadı.");

        // Aynı tabloda tek PK kontrolü
        if (request.IsPrimaryKey)
        {
            var hasPk = await _db.SchemaColumns.AnyAsync(c => c.SchemaTableId == tableId && c.IsPrimaryKey, ct);
            if (hasPk) throw new InvalidOperationException("Bu tabloda zaten bir birincil anahtar var.");
        }

        var column = new SchemaColumn
        {
            SchemaTableId = tableId,
            Name = request.Name.Trim(),
            DataType = request.DataType.ToLowerInvariant(),
            IsNullable = request.IsNullable,
            IsPrimaryKey = request.IsPrimaryKey,
            IsUnique = request.IsUnique,
            DefaultValue = request.DefaultValue,
            MaxLength = request.MaxLength,
            Precision = request.Precision,
            Scale = request.Scale,
            OrdinalPosition = request.OrdinalPosition
        };

        _db.SchemaColumns.Add(column);
        await IncrementSchemaVersionAsync(table.ProjectId, ct);
        await _db.SaveChangesAsync(ct);

        return new ColumnDto(column.Id, column.SchemaTableId, column.Name, column.DataType,
            column.IsNullable, column.IsPrimaryKey, column.IsUnique, column.DefaultValue,
            column.MaxLength, column.Precision, column.Scale, column.OrdinalPosition);
    }

    public async Task<ColumnDto?> UpdateColumnAsync(Guid id, UpdateColumnRequest request, CancellationToken ct = default)
    {
        var column = await _db.SchemaColumns.Include(c => c.SchemaTable).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (column is null) return null;

        if (request.IsPrimaryKey && !column.IsPrimaryKey)
        {
            var hasPk = await _db.SchemaColumns.AnyAsync(c => c.SchemaTableId == column.SchemaTableId && c.IsPrimaryKey && c.Id != id, ct);
            if (hasPk) throw new InvalidOperationException("Bu tabloda zaten bir birincil anahtar var.");
        }

        column.Name = request.Name.Trim();
        column.DataType = request.DataType.ToLowerInvariant();
        column.IsNullable = request.IsNullable;
        column.IsPrimaryKey = request.IsPrimaryKey;
        column.IsUnique = request.IsUnique;
        column.DefaultValue = request.DefaultValue;
        column.MaxLength = request.MaxLength;
        column.Precision = request.Precision;
        column.Scale = request.Scale;
        column.OrdinalPosition = request.OrdinalPosition;

        await IncrementSchemaVersionAsync(column.SchemaTable.ProjectId, ct);
        await _db.SaveChangesAsync(ct);

        return new ColumnDto(column.Id, column.SchemaTableId, column.Name, column.DataType,
            column.IsNullable, column.IsPrimaryKey, column.IsUnique, column.DefaultValue,
            column.MaxLength, column.Precision, column.Scale, column.OrdinalPosition);
    }

    public async Task<bool> DeleteColumnAsync(Guid id, CancellationToken ct = default)
    {
    var column = await _db.SchemaColumns.Include(c => c.SchemaTable).FirstOrDefaultAsync(x => x.Id == id, ct);
    if (column is null) return false;

    _db.SchemaColumns.Remove(column);
    await IncrementSchemaVersionAsync(column.SchemaTable.ProjectId, ct);   // ← bu satır olmalı
    await _db.SaveChangesAsync(ct);
    return true;
    }

    private async Task IncrementSchemaVersionAsync(Guid projectId, CancellationToken ct)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId, ct);
        if (project is not null)
        {
            project.SchemaVersion++;
            project.UpdatedAt = DateTime.UtcNow;
        }
    }
}