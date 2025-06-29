using System.Net;
using System.Net.Http;
using Xunit;

namespace Common.Tests.BDD.ProcessInfo;

public class ProcessInfoSteps
{
    private ProcessApi? _api;
    private string? _json;

    [Given("the process endpoint returns {json}")]
    public void GivenEndpoint(string json)
    {
        _api = new ProcessApi(new HttpClient(new JsonHandler(json)), "http://localhost");
    }

    [When("I request processes for app {appId}")]
    public async Task WhenRequest(string appId)
    {
        if (_api != null)
            _json = await _api.GetProcessesAsync(appId);
    }

    [Then("the process API returns {json}")]
    public void ThenReturns(string json)
    {
        Assert.Equal(json, _json);
    }
}

public class JsonHandler : HttpMessageHandler
{
    private readonly string _json;
    public JsonHandler(string json) => _json = json;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(_json) });
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class GivenAttribute : Attribute
{
    public GivenAttribute(string text) { }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class WhenAttribute : Attribute
{
    public WhenAttribute(string text) { }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ThenAttribute : Attribute
{
    public ThenAttribute(string text) { }
}
