using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixtureForge.Persistence.Configurations;

public class SchemaRelationshipConfiguration : IEntityTypeConfiguration<SchemaRelationship>
{
    public void Configure(EntityTypeBuilder<SchemaRelationship> builder)
    {
        builder.ToTable("SchemaRelationships");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RelationshipType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => new { x.SourceColumnId, x.TargetColumnId }).IsUnique();

        builder.HasOne(x => x.Project)
            .WithMany(p => p.Relationships)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SourceTable)
            .WithMany(t => t.SourceRelationships)
            .HasForeignKey(x => x.SourceTableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SourceColumn)
            .WithMany(c => c.SourceRelationships)
            .HasForeignKey(x => x.SourceColumnId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TargetTable)
            .WithMany(t => t.TargetRelationships)
            .HasForeignKey(x => x.TargetTableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TargetColumn)
            .WithMany(c => c.TargetRelationships)
            .HasForeignKey(x => x.TargetColumnId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}