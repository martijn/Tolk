using System.Net;
using System.Text.Json;
using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;
using Tolk.Domain.ProjectAggregate.Events;
using Tolk.FunctionApp.Tests.Support;

namespace Tolk.FunctionApp.Tests;

public class CreatePhraseTest
{
    private readonly Mock<IProjectFactory> _mockProjectFactory;
    private readonly Project _project;
    private readonly ProjectBuilder _projectBuilder;
    private readonly CreatePhrase _sut;

    public CreatePhraseTest()
    {
        _mockProjectFactory = new Mock<IProjectFactory>();
        _projectBuilder = new ProjectBuilder(_mockProjectFactory.Object);
        _sut = new CreatePhrase(_projectBuilder);
        _project = Project.Create(new ProjectCreatedEvent(Guid.Empty, "My Project"));
    }

    [Fact]
    public async Task ReturnsNotFound()
    {
        var context = new MockFunctionContext();

        var response = await _sut.Run(
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

        _mockProjectFactory.Setup(pf => pf.Create(Guid.Empty, It.IsAny<List<IEvent>>())).Returns(_project);

        var response = await _sut.Run(
            new MockHttpRequestData(context),
            Guid.Empty,
            "myProject",
            jsonEvents,
            context);

        _mockProjectFactory.VerifyAll();
    }

    [Fact]
    public async Task AddsPhraseCreatedEvent()
    {
        var context = new MockFunctionContext();
        var jsonEvents = new List<JsonElement> { JsonDocument.Parse(MockEvents.ProjectCreatedEvent).RootElement };

        _mockProjectFactory.Setup(pf => pf.Create(Guid.Empty, It.IsAny<List<IEvent>>())).Returns(_project);

        var response = await _sut.Run(
            new MockHttpRequestData(context),
            Guid.Empty,
            "A new phrase",
            jsonEvents,
            context);

        Assert.IsType<PhraseCreatedEvent>(_project.UnsavedEvents().Last());
        Assert.Equal("A new phrase", (_project.UnsavedEvents().Last() as PhraseCreatedEvent).Name);
    }

    [Fact]
    public async Task ReturnsEventDocumentAndOk()
    {
    }
}
