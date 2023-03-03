using System.Text.Json;
using Tolk.Domain;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp.Tests;

public class ProjectBuilderTests
{
    private readonly Mock<IProjectFactory> _mockProjectFactory;
    private readonly ProjectBuilder _projectBuilder;

    public ProjectBuilderTests()
    {
        _mockProjectFactory = new Mock<IProjectFactory>();
        _projectBuilder = new ProjectBuilder(_mockProjectFactory.Object);
    }

    [Theory]
    [InlineData(MockEvents.PhraseCreatedEvent, typeof(PhraseCreatedEvent))]
    [InlineData(MockEvents.ProjectCreatedEvent, typeof(ProjectCreatedEvent))]
    [InlineData(MockEvents.TranslationUpdatedEvent, typeof(TranslationUpdatedEvent))]
    public void DeserializesEvents(string json, Type eventType)
    {
        var jsonEvents = new List<JsonElement> { JsonDocument.Parse(json).RootElement };

        _mockProjectFactory
            .Setup(pf => pf.Create(Guid.Empty, It.IsAny<IEnumerable<IEvent>>()))
            .Callback((Guid _, IEnumerable<IEvent> events) =>
            {
                Assert.Collection(events, e => Assert.IsType(eventType, e));
            });

        _projectBuilder.FromJsonEvents(Guid.Empty, jsonEvents);

        _mockProjectFactory.VerifyAll();
    }

    [Fact]
    public void ThrowsIfEventTypeDoesNotExist()
    {
        var jsonEvents = new List<JsonElement>
        {
            JsonDocument.Parse("""{ "Type": "DoesNotExist" }""").RootElement
        };

        Assert.Throws<ProjectBuilder.InvalidEventException>(() =>
            new ProjectBuilder(new ProjectFactory()).FromJsonEvents(Guid.Empty, jsonEvents)
        );
    }

    [Fact]
    public void ThrowsIfTypeIsMissing()
    {
        var jsonEvents = new List<JsonElement>
        {
            JsonDocument.Parse("""{}""").RootElement
        };

        Assert.Throws<ProjectBuilder.InvalidEventException>(() =>
            new ProjectBuilder(new ProjectFactory()).FromJsonEvents(Guid.Empty, jsonEvents)
        );
    }
}
