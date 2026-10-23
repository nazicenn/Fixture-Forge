using FixtureForge.Domain.Entities;

namespace FixtureForge.Tests;

public class GenerationJobTableTests
{
    [Fact]
    public void GenerationJobTable_Defaults_AreCorrect()
    {
        var t = new GenerationJobTable();

        Assert.NotEqual(Guid.Empty, t.Id);
        Assert.Equal("Pending", t.Status);
        Assert.Equal(0, t.TargetRowCount);
        Assert.Equal(0, t.GeneratedRowCount);
        Assert.Null(t.ErrorSummary);
    }

    [Fact]
    public void GenerationJobTable_CanSet_Values()
    {
        var t = new GenerationJobTable
        {
            TargetRowCount = 1000,
            GeneratedRowCount = 1000,
            Status = "Succeeded"
        };

        Assert.Equal(1000, t.TargetRowCount);
        Assert.Equal(1000, t.GeneratedRowCount);
        Assert.Equal("Succeeded", t.Status);
    }
}