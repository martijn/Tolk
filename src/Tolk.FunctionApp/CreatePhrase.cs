using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Tolk.Domain;

namespace Tolk.FunctionApp;

public static class CreatePhrase
{
    private const string Query = "SELECT * FROM ProjectEvents e WHERE e.Aggregate = CONCAT('Project-', {projectId}) ORDER BY e.Version DESC";
    
    [Function("CreatePhrase")]
    public static async Task<SingleDocumentOutput> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        Guid projectId,
        string name,
        [CosmosDBInput(
            "TolkDev",
            "ProjectEvents",
            Connection = "AzureCosmosDbConnectionString",
            SqlQuery = Query)]
        List<JsonElement> jsonEvents,
        FunctionContext executionContext)
    {
        if (!jsonEvents.Any())
        {
            return new SingleDocumentOutput()
            {
                HttpResponse = req.CreateResponse(HttpStatusCode.NotFound)
            };
        }
        
        var project = ProjectBuilder.FromJsonEvents(projectId, jsonEvents);
        project.CreatePhrase(name);
        
        return new SingleDocumentOutput()
        {
            OutputEvent = project.UnsavedEvents().First() as Event,
            HttpResponse = req.CreateResponse(HttpStatusCode.OK)
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
