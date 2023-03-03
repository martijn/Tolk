using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Tolk.Domain.ProjectAggregate;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp;

public static class CreateProject
{
    [Function("CreateProject")]
    public static async Task<SingleDocumentOutput> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequestData req,
        string? name,
        FunctionContext executionContext)
    {
        if (name is null) return await SingleDocumentOutput.BadRequest(req, "Please specify name");

        var initialEvent = new ProjectCreatedEvent(
            Guid.NewGuid(),
            name);

        var project = Project.Create(initialEvent);

        // TODO naming
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(project);

        return new SingleDocumentOutput
        {
            OutputEvent = project.UnsavedEvents().First(),
            HttpResponse = response
        };
    }
}
