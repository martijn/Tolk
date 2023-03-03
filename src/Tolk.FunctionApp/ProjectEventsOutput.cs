using System.Collections.Immutable;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Tolk.Domain;

namespace Tolk.FunctionApp;

/// <summary>
///     An Output Binding for a function that can return both a list of documents
///     to be stored in the ProjectEvents container, and a HTTP response with a
///     JSON body.
/// </summary>
public class ProjectEventsOutput
{
    private ProjectEventsOutput()
    {
    }

    [CosmosDBOutput("TolkDev", "ProjectEvents", Connection = "AzureCosmosDbConnectionString")]
    public IReadOnlyList<object>? ProjectEvents { get; set; }

    public HttpResponseData? HttpResponse { get; set; }

    public static async Task<ProjectEventsOutput> BadRequest(HttpRequestData req, string? message)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new { Error = message });

        return new ProjectEventsOutput
        {
            HttpResponse = response
        };
    }

    public static ProjectEventsOutput NotFound(HttpRequestData req)
    {
        return new ProjectEventsOutput
        {
            HttpResponse = req.CreateResponse(HttpStatusCode.NotFound)
        };
    }

    public static async Task<ProjectEventsOutput> Ok(HttpRequestData req, IList<Event> projectEvents,
        object? body = null)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        if (body is not null) await response.WriteAsJsonAsync(body);

        return new ProjectEventsOutput
        {
            ProjectEvents = projectEvents.ToImmutableList<object>(),
            HttpResponse = response
        };
    }
}
