namespace Tolk.Domain.Tests;

public class AggregateTest
{
    [Fact]
    public void BuildAggregateFromEventsTest()
    {
        var events = new List<IEvent>()
        {
            new ProjectCreatedEvent(Guid.NewGuid(), "My test project"),
            new SomePropertyChangedEvent(Guid.NewGuid(), "A value")
        };
        
        var project = new Project(Guid.NewGuid(), events);
        
        Assert.Equal("My test project", project.Name);
        Assert.Equal("A value", project.SomeProperty);
    }
    
    [Fact]
    public void ChangeSomePropertyTest()
    {
        var initialEvent = new ProjectCreatedEvent(Guid.NewGuid(), "My test project");

        var project = Project.Create(initialEvent);
        
        project.ChangeSomeProperty("A value");
        
        Assert.Equal("My test project", project.Name);
        Assert.Equal("A value", project.SomeProperty);
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
        
        Assert.Equal(1, project.Version);
        
        project.ChangeSomeProperty("A value");
        
        Assert.Equal(2, project.Version);
    }
}
