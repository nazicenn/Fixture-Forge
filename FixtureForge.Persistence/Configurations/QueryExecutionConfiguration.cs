using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixtureForge.Persistence.Configurations;

public class QueryExecutionConfiguration : IEntityTypeConfiguration<QueryExecution>
{
    public void Configure(EntityTypeBuilder<QueryExecution> builder)
    {
        builder.ToTable("QueryExecutions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QueryFingerprint).IsRequired().HasMaxLength(64);
        builder.Property(x => x.FingerprintPolicyVersion).IsRequired().HasMaxLength(20);
        builder.Property(x => x.QueryText).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.QueryType).IsRequired().HasMaxLength(20);
        builder.Property(x => x.ParameterSummary).HasColumnType("nvarchar(max)");
        builder.Property(x => x.Status).IsRequired().HasMaxLength(20);
        builder.Property(x => x.ErrorCode).HasMaxLength(100);
        builder.Property(x => x.ErrorMessage).HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new { x.ConnectionId, x.CreatedAt });
        builder.HasIndex(x => x.QueryFingerprint);

        builder.HasOne(x => x.Connection)
            .WithMany(c => c.Queries)
            .HasForeignKey(x => x.ConnectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}