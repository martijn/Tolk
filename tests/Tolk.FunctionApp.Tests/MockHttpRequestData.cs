using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Tolk.FunctionApp.Tests;

public sealed class MockHttpRequestData : HttpRequestData
{
    public MockHttpRequestData(FunctionContext functionContext) : base(functionContext)
    {
        Context = functionContext;
    }

    public FunctionContext Context { get; }

    public override Stream Body { get; }
    public override HttpHeadersCollection Headers { get; }
    public override IReadOnlyCollection<IHttpCookie> Cookies { get; }
    public override Uri Url { get; }
    public override IEnumerable<ClaimsIdentity> Identities { get; }
    public override string Method { get; }

    public override HttpResponseData CreateResponse()
    {
        return new MockHttpResponseData(Context);
    }
}
