using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Tolk.Domain.ProjectAggregate;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp;

public static class CreateProject
{
    [Function("CreateProject")]
    public static async Task<ProjectEventsOutput> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequestData req,
        string? name,
        FunctionContext executionContext)
    {
        if (name is null) return await ProjectEventsOutput.BadRequest(req, "Please specify name");

        var initialEvent = new ProjectCreatedEvent(Guid.NewGuid(), name);
        var project = Project.Create(initialEvent);

        return await ProjectEventsOutput.Ok(req, project.UnsavedEvents(), project);
    }
}
