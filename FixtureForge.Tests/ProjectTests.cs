using FixtureForge.Domain.Entities;

namespace FixtureForge.Tests;

public class ProjectTests
{
    [Fact]
    public void Project_DefaultsToNewGuidAndUtcNow()
    {
        var project = new Project();

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(string.Empty, project.Name);
        Assert.Null(project.Description);
        Assert.True(project.CreatedAt <= DateTime.UtcNow);
        Assert.True(project.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Project_IdCanBeSet()
    {
        var id = Guid.NewGuid();
        var project = new Project { Id = id, Name = "Test" };

        Assert.Equal(id, project.Id);
        Assert.Equal("Test", project.Name);
    }
}