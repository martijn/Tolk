using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Tolk.FunctionApp;

public class ArchiveProject
{
    private const string Query =
        "SELECT * FROM ProjectEvents e WHERE e.Aggregate = CONCAT('Project-', {projectId}) ORDER BY e.Version ASC";
    
    private readonly IProjectBuilder _projectBuilder;

    public ArchiveProject(IProjectBuilder projectBuilder)
    {
        _projectBuilder = projectBuilder;
    }

    [Function("ArchiveProject")]
    public async Task<ProjectEventsOutput> Run(
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
        if (!jsonEvents.Any()) return ProjectEventsOutput.NotFound(req);

        var project = _projectBuilder.FromJsonEvents(projectId, jsonEvents);
        project.Archive();

        return await ProjectEventsOutput.Ok(req, project.UnsavedEvents());
    }
}
