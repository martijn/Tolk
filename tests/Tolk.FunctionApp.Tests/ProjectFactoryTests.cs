using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp.Tests;

public class ProjectFactoryTests : IClassFixture<ProjectFactory>
{
    private readonly ProjectFactory _projectFactory;

    public ProjectFactoryTests(ProjectFactory projectFactory)
    {
        _projectFactory = projectFactory;
    }

    [Fact]
    public void CreatesAProject()
    {
        var project = _projectFactory.Create(Guid.Empty, new List<IEvent>
        {
            new ProjectCreatedEvent(Guid.Empty, "My project")
        });

        Assert.IsType<Project>(project);
        Assert.Equal("My project", project.Name);
    }
}
