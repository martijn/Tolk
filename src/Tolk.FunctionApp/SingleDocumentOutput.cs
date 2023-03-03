using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Tolk.FunctionApp;

public class SingleDocumentOutput
{
    [CosmosDBOutput(
        "TolkDev",
        "ProjectEvents",
        Connection = "AzureCosmosDbConnectionString")]

    public object? OutputEvent { get; set; }

    public HttpResponseData? HttpResponse { get; set; }

    public static SingleDocumentOutput NotFound(HttpRequestData req)
    {
        return new SingleDocumentOutput
        {
            HttpResponse = req.CreateResponse(HttpStatusCode.NotFound)
        };
    }

    public static async Task<SingleDocumentOutput> BadRequest(HttpRequestData req, string? message)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new { Error = message });

        return new SingleDocumentOutput
        {
            HttpResponse = response
        };
    }
}
