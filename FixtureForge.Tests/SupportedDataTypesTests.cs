using FixtureForge.Application.Schema;

namespace FixtureForge.Tests;

public class SupportedDataTypesTests
{
    [Theory]
    [InlineData("int")]
    [InlineData("nvarchar")]
    [InlineData("datetime2")]
    [InlineData("bit")]
    [InlineData("decimal")]
    [InlineData("uniqueidentifier")]
    public void SupportedDataTypes_Contains_CommonTypes(string type)
    {
        Assert.Contains(type, SupportedDataTypes.All);
    }

    [Theory]
    [InlineData("gecersiz")]
    [InlineData("json")]
    [InlineData("xml")]
    public void SupportedDataTypes_DoesNotContain_UnsupportedTypes(string type)
    {
        Assert.DoesNotContain(type, SupportedDataTypes.All);
    }

    [Theory]
    [InlineData("nvarchar", true)]
    [InlineData("varchar", true)]
    [InlineData("int", false)]
    public void RequireMaxLength_IsCorrect(string type, bool expected)
    {
        Assert.Equal(expected, SupportedDataTypes.RequireMaxLength.Contains(type));
    }
}