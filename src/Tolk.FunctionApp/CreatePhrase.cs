using System.Collections.Immutable;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Tolk.FunctionApp;

public class CreatePhrase
{
    private const string Query =
        "SELECT * FROM ProjectEvents e WHERE e.Aggregate = CONCAT('Project-', {projectId}) ORDER BY e.Version DESC";

    private readonly IProjectBuilder _projectBuilder;

    public CreatePhrase(IProjectBuilder projectBuilder)
    {
        _projectBuilder = projectBuilder;
    }

    [Function("CreatePhrase")]
    public async Task<MultipleDocumentOutput> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequestData req,
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
        if (!jsonEvents.Any()) return MultipleDocumentOutput.NotFound(req);

        var project = _projectBuilder.FromJsonEvents(projectId, jsonEvents);
        project.CreatePhrase(name);

        return new MultipleDocumentOutput
        {
            // Cast to object to allow polymorphic serialization of derived Events
            OutputEvents = project.UnsavedEvents().ToImmutableList<object>(),
            HttpResponse = req.CreateResponse(HttpStatusCode.OK)
        };
    }
}
