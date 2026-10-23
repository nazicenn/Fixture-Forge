using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixtureForge.Persistence.Configurations;

public class SchemaColumnConfiguration : IEntityTypeConfiguration<SchemaColumn>
{
    public void Configure(EntityTypeBuilder<SchemaColumn> builder)
    {
        builder.ToTable("SchemaColumns");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.DataType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DefaultValue).HasMaxLength(500);

        builder.HasIndex(x => new { x.SchemaTableId, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.SchemaTableId, x.OrdinalPosition }).IsUnique();

        builder.HasOne(x => x.SchemaTable)
            .WithMany(t => t.Columns)
            .HasForeignKey(x => x.SchemaTableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}