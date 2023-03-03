using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Tolk.FunctionApp;

public class MultipleDocumentOutput
{
    [CosmosDBOutput(
        "TolkDev",
        "ProjectEvents",
        Connection = "AzureCosmosDbConnectionString")]

    public IReadOnlyList<object>? OutputEvents { get; set; }

    public HttpResponseData? HttpResponse { get; set; }

    public static MultipleDocumentOutput NotFound(HttpRequestData req)
    {
        return new MultipleDocumentOutput
        {
            HttpResponse = req.CreateResponse(HttpStatusCode.NotFound)
        };
    }
}
