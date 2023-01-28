using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;
using Tolk.Domain.ProjectAggregate.Events;

namespace Tolk.FunctionApp;

public static class GetProject
{
    private const string Query = "SELECT * FROM ProjectEvents e WHERE e.Aggregate = CONCAT('Project-', {id})";
    
    [Function("GetProject")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProject/{id:guid}")] HttpRequestData req,
        Guid id,
        [CosmosDBInput(
            "TolkDev",
            "ProjectEvents",
            Connection = "AzureCosmosDbConnectionString",
            SqlQuery = Query)] IEnumerable<JsonElement> jsonEvents,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetProject");
        logger.LogInformation("Getting project {id}", id);
        
        var events = new List<IEvent>();

        foreach (var jsonEvent in jsonEvents)
        {
            var eventTypeString = jsonEvent.GetProperty("Type").GetString();
            var eventType = Type.GetType($"Tolk.Domain.ProjectAggregate.Events.{eventTypeString}, Tolk.Domain");

            if (eventType is null) throw new Exception($"Can't locate eventType {eventTypeString}");
            events.Add((IEvent)jsonEvent.Deserialize(eventType)!);
        }

        var project = new Project(id, events);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(project);

        return response;
    }
}
