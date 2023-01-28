using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp;

public static class CreateProject
{
    [Function("CreateProject")]
    public static async Task<SingleDocumentOutput> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        string? name,
        FunctionContext executionContext)
    {
        if (name is null)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteAsJsonAsync(new { Error = "Please specify name" });
            return new SingleDocumentOutput() { HttpResponse = badRequestResponse };
        }
        
        var initialEvent = new ProjectCreatedEvent(
            Guid.NewGuid(),
            name);
        
        var project = Project.Create(initialEvent);
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(project);

        return new SingleDocumentOutput()
        {
            OutputEvent = project.UnsavedEvents().First() as Event,
            HttpResponse = response
        };
    }

    public class SingleDocumentOutput
    {
        [CosmosDBOutput(
            "TolkDev",
            "ProjectEvents",
            Connection = "AzureCosmosDbConnectionString")]
        public Event? OutputEvent { get; set; }
        public HttpResponseData? HttpResponse { get; set; }
    }
}
