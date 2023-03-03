namespace Tolk.Domain.Tests;

public class AggregateTest
{
    [Fact]
    public void BuildAggregateFromEventsTest()
    {
        var events = new List<IEvent>
        {
            new ProjectCreatedEvent(Guid.NewGuid(), "My test project"),
            new PhraseCreatedEvent(Guid.NewGuid(), "A phrase")
        };

        var project = new Project(Guid.NewGuid(), events);

        Assert.Equal("My test project", project.Name);
        Assert.Equal(1, project.Version);
    }

    [Fact]
    public void CreatePhraseTest()
    {
        var initialEvent = new ProjectCreatedEvent(Guid.NewGuid(), "My test project");

        var project = Project.Create(initialEvent);

        project.CreatePhrase("A phrase");

        Assert.Equal("My test project", project.Name);
        Assert.Equal("A phrase", project.Phrases.First().Key);
    }

    [Fact]
    public void CreateSetsIdAndPartitionKeyTest()
    {
        var initialEvent = new ProjectCreatedEvent(Guid.NewGuid(), "My test project");
        var project = Project.Create(initialEvent);

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal($"Project-{project.Id}", project.PartitionKey);
    }

    [Fact]
    public void VersionIncrementTest()
    {
        var initialEvent = new ProjectCreatedEvent(Guid.NewGuid(), "My test project");
        var project = Project.Create(initialEvent);

        Assert.Equal(0, project.Version);

        project.CreatePhrase("A phrase");

        Assert.Equal(1, project.Version);
    }
}
