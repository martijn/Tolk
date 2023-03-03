using System.Net;
using System.Text.Json;
using Tolk.Domain;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp.Tests;

public class CreatePhraseTest
{
    private readonly Mock<IProjectFactory> _mockProjectFactory;
    private readonly CreatePhrase _function;

    public CreatePhraseTest()
    {
        _mockProjectFactory = new Mock<IProjectFactory>();
        _function = new CreatePhrase(new ProjectBuilder(_mockProjectFactory.Object));

        _mockProjectFactory
            .Setup(pf => pf.Create(Guid.Empty, It.IsAny<IEnumerable<IEvent>>()))
            .Returns((Guid id, IEnumerable<IEvent> events) => new ProjectFactory().Create(id, events));
    }

    [Fact]
    public async Task ReturnsNotFound()
    {
        var context = new MockFunctionContext();

        var response = await _function.Run(
            new MockHttpRequestData(context),
            Guid.Empty,
            "myProject",
            new List<JsonElement>(),
            context);

        Assert.Equal(HttpStatusCode.NotFound, response.HttpResponse.StatusCode);
    }

    [Fact]
    public async Task CallsProjectBuilder()
    {
        var context = new MockFunctionContext();
        var jsonEvents = new List<JsonElement> { JsonDocument.Parse(MockEvents.ProjectCreatedEvent).RootElement };

        await _function.Run(
            new MockHttpRequestData(context),
            Guid.Empty,
            "myProject",
            jsonEvents,
            context);

        // TODO Verification doesn't match test name…
        _mockProjectFactory.Verify(pf => pf.Create(Guid.Empty, It.IsAny<IEnumerable<IEvent>>()), Times.Once);
    }

    [Fact]
    public async Task OutputsPhraseCreatedEventAndStatusOk()
    {
        var context = new MockFunctionContext();
        var jsonEvents = new List<JsonElement> { JsonDocument.Parse(MockEvents.ProjectCreatedEvent).RootElement };

        var response = await _function.Run(
            new MockHttpRequestData(context),
            Guid.Empty,
            "A new phrase",
            jsonEvents,
            context);

        var projectEvent = response.ProjectEvents![0];
        Assert.IsType<PhraseCreatedEvent>(projectEvent);
        Assert.Equal("A new phrase", (projectEvent as PhraseCreatedEvent)!.Name);

        Assert.Equal(HttpStatusCode.OK, response.HttpResponse!.StatusCode);
    }
}
