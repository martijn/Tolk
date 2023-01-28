using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Tolk.FunctionApp;

public static class GetProject
{
    // TODO id -> projectId, DRY
    private const string Query = "SELECT * FROM ProjectEvents e WHERE e.Aggregate = CONCAT('Project-', {id}) ORDER BY e.Version ASC";

    [Function("GetProject")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProject/{id:guid}")]
        HttpRequestData req,
        Guid id,
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
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        
        var project = ProjectBuilder.FromJsonEvents(id, jsonEvents);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(project);

        return response;
    }
}
