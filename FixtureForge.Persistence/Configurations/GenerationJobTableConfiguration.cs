using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixtureForge.Persistence.Configurations;

public class GenerationJobTableConfiguration : IEntityTypeConfiguration<GenerationJobTable>
{
    public void Configure(EntityTypeBuilder<GenerationJobTable> builder)
    {
        builder.ToTable("GenerationJobTables");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status).IsRequired().HasMaxLength(20);
        builder.Property(x => x.ErrorSummary).HasMaxLength(4000);

        builder.HasIndex(x => new { x.GenerationJobId, x.SchemaTableId }).IsUnique();

        builder.HasOne(x => x.GenerationJob)
            .WithMany(j => j.Tables)
            .HasForeignKey(x => x.GenerationJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SchemaTable)
            .WithMany(t => t.GenerationJobTables)
            .HasForeignKey(x => x.SchemaTableId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}