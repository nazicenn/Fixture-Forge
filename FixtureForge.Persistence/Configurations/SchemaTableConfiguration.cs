using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixtureForge.Persistence.Configurations;

public class SchemaTableConfiguration : IEntityTypeConfiguration<SchemaTable>
{
    public void Configure(EntityTypeBuilder<SchemaTable> builder)
    {
        builder.ToTable("SchemaTables");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.ProjectId, x.Name }).IsUnique();

        builder.HasOne(x => x.Project)
            .WithMany(p => p.Tables)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}