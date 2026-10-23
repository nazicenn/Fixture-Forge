using FixtureForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixtureForge.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<SchemaTable> SchemaTables => Set<SchemaTable>();
    public DbSet<SchemaColumn> SchemaColumns => Set<SchemaColumn>();
    public DbSet<SchemaRelationship> SchemaRelationships => Set<SchemaRelationship>();
    public DbSet<GenerationJob> GenerationJobs => Set<GenerationJob>();
    public DbSet<GenerationJobTable> GenerationJobTables => Set<GenerationJobTable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}