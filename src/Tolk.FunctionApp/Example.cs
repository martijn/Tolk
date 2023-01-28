using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Tolk.Domain.ProjectAggregate;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp;

public static class Example
{
    [Function("Example")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("Project");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var key = "myPhrase";
        var enTranslation = "A new phrase";
        var nlTranslation = "Een nieuwe tekst";
        
        var initialEvent = new ProjectCreatedEvent(
            Guid.NewGuid(),
            "My test project");
        
        var project = Project.Create(initialEvent);
       
        project.CreatePhrase(key);
        project.UpdateTranslation(key, "en", enTranslation);
        project.UpdateTranslation(key, "nl", nlTranslation);
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(project);

        return response;
        
    }
}
