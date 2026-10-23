using FixtureForge.Domain.Entities;

namespace FixtureForge.Tests;

public class GenerationJobTests
{
    [Fact]
    public void GenerationJob_Defaults_AreCorrect()
    {
        var job = new GenerationJob();

        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.Equal("Pending", job.Status);
        Assert.Null(job.StartedAt);
        Assert.Null(job.CompletedAt);
        Assert.Equal(0, job.GeneratedRowCount);
        Assert.Null(job.ErrorSummary);
        Assert.Equal("v1", job.SchemaVersionTag);
        Assert.Equal("v1", job.RuleVersionTag);
        Assert.Equal("v1", job.GeneratorVersionTag);
        Assert.Null(job.SnapshotJson);
        Assert.True(job.CreatedAt <= DateTime.UtcNow);
        Assert.Empty(job.Tables);
    }

    [Fact]
    public void GenerationJob_CanSet_StatusAndTimestamps()
    {
        var job = new GenerationJob
        {
            Status = "Succeeded",
            StartedAt = DateTime.UtcNow.AddSeconds(-5),
            CompletedAt = DateTime.UtcNow,
            GeneratedRowCount = 1000,
            RandomSeed = 42
        };

        Assert.Equal("Succeeded", job.Status);
        Assert.NotNull(job.StartedAt);
        Assert.NotNull(job.CompletedAt);
        Assert.Equal(1000, job.GeneratedRowCount);
        Assert.Equal(42, job.RandomSeed);
    }
}