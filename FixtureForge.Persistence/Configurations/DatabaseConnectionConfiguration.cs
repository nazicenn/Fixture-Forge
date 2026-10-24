using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixtureForge.Persistence.Configurations;

public class DatabaseConnectionConfiguration : IEntityTypeConfiguration<DatabaseConnection>
{
    public void Configure(EntityTypeBuilder<DatabaseConnection> builder)
    {
        builder.ToTable("DatabaseConnections");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Provider).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ServerReference).IsRequired().HasMaxLength(300);
        builder.Property(x => x.DatabaseName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.AuthenticationType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.EncryptedConnectionString).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.ProjectId, x.Name }).IsUnique();

        builder.HasOne(x => x.Project)
            .WithMany(p => p.Connections)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}