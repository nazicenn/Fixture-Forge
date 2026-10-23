using FixtureForge.Domain.Entities;

namespace FixtureForge.Tests;

public class SchemaColumnTests
{
    [Fact]
    public void SchemaColumn_Defaults_AreCorrect()
    {
        var col = new SchemaColumn();

        Assert.NotEqual(Guid.Empty, col.Id);
        Assert.Equal(string.Empty, col.Name);
        Assert.Equal(string.Empty, col.DataType);
        Assert.True(col.IsNullable);
        Assert.False(col.IsPrimaryKey);
        Assert.False(col.IsUnique);
        Assert.Null(col.MaxLength);
        Assert.Null(col.Precision);
        Assert.Null(col.Scale);
    }

    [Fact]
    public void SchemaColumn_PrimaryKey_CanBeSet()
    {
        var col = new SchemaColumn { Name = "Id", DataType = "int", IsPrimaryKey = true };
        Assert.True(col.IsPrimaryKey);
    }
}