using FixtureForge.Application.Schema;
using FixtureForge.Contracts.Schema;
using FixtureForge.Domain.Entities;
using FixtureForge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Infrastructure.Schema;

public class RelationshipService : IRelationshipService
{
    private readonly AppDbContext _db;
    public RelationshipService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<RelationshipDto>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _db.SchemaRelationships
            .AsNoTracking()
            .Where(r => r.ProjectId == projectId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RelationshipDto(
                r.Id, r.ProjectId,
                r.SourceTableId, r.SourceTable.Name,
                r.SourceColumnId, r.SourceColumn.Name,
                r.TargetTableId, r.TargetTable.Name,
                r.TargetColumnId, r.TargetColumn.Name,
                r.RelationshipType, r.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<RelationshipDto> CreateAsync(Guid projectId, CreateRelationshipRequest request, CancellationToken ct = default)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId, ct);
        if (!projectExists) throw new InvalidOperationException("Proje bulunamadı.");

        var sourceColumn = await _db.SchemaColumns
            .Include(c => c.SchemaTable)
            .FirstOrDefaultAsync(c => c.Id == request.SourceColumnId && c.SchemaTableId == request.SourceTableId, ct);
        if (sourceColumn is null) throw new InvalidOperationException("Kaynak sütun bulunamadı veya tabloya ait değil.");

        var targetColumn = await _db.SchemaColumns
            .Include(c => c.SchemaTable)
            .FirstOrDefaultAsync(c => c.Id == request.TargetColumnId && c.SchemaTableId == request.TargetTableId, ct);
        if (targetColumn is null) throw new InvalidOperationException("Hedef sütun bulunamadı veya tabloya ait değil.");

        if (!string.Equals(sourceColumn.DataType, targetColumn.DataType, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Veri tipi uyumsuz: {sourceColumn.DataType} → {targetColumn.DataType}");

        if (!targetColumn.IsPrimaryKey)
            throw new InvalidOperationException("Hedef sütun birincil anahtar olmalıdır.");

        var exists = await _db.SchemaRelationships.AnyAsync(
            r => r.SourceColumnId == request.SourceColumnId && r.TargetColumnId == request.TargetColumnId, ct);
        if (exists) throw new InvalidOperationException("Bu ilişki zaten mevcut.");

        var rel = new SchemaRelationship
        {
            ProjectId = projectId,
            SourceTableId = request.SourceTableId,
            SourceColumnId = request.SourceColumnId,
            TargetTableId = request.TargetTableId,
            TargetColumnId = request.TargetColumnId,
            RelationshipType = request.RelationshipType,
            CreatedAt = DateTime.UtcNow
        };

        _db.SchemaRelationships.Add(rel);

        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId, ct);
        if (project is not null)
        {
            project.SchemaVersion++;
            project.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);

        return new RelationshipDto(
            rel.Id, rel.ProjectId,
            sourceColumn.SchemaTableId, sourceColumn.SchemaTable.Name,
            sourceColumn.Id, sourceColumn.Name,
            targetColumn.SchemaTableId, targetColumn.SchemaTable.Name,
            targetColumn.Id, targetColumn.Name,
            rel.RelationshipType, rel.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var rel = await _db.SchemaRelationships.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (rel is null) return false;

        _db.SchemaRelationships.Remove(rel);

        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == rel.ProjectId, ct);
        if (project is not null)
        {
            project.SchemaVersion++;
            project.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }
}