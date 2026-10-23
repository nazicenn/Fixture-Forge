using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixtureForge.Persistence.Configurations;

public class GenerationJobConfiguration : IEntityTypeConfiguration<GenerationJob>
{
    public void Configure(EntityTypeBuilder<GenerationJob> builder)
    {
        builder.ToTable("GenerationJobs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status).IsRequired().HasMaxLength(20);
        builder.Property(x => x.ErrorSummary).HasMaxLength(4000);
        builder.Property(x => x.SchemaVersionTag).IsRequired().HasMaxLength(20);
        builder.Property(x => x.RuleVersionTag).IsRequired().HasMaxLength(20);
        builder.Property(x => x.GeneratorVersionTag).IsRequired().HasMaxLength(20);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.SnapshotJson).HasColumnType("nvarchar(max)");
        builder.HasIndex(x => new { x.ProjectId, x.CreatedAt });

        builder.HasOne(x => x.Project)
            .WithMany(p => p.GenerationJobs)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}