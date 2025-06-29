using System;
using Xunit;

namespace Common.Tests.BDD.FoundationInfo;

public class FoundationInfoSteps
{
    private IFoundationApi? _api;
    private string? _json;

    [Given("the foundation endpoint returns {json}")]
    public void GivenEndpointReturns(string json)
    {
        _api = new FoundationApi(new HttpClient(new JsonHandler(json)), "http://localhost/info");
    }

    [When("I request the foundation info")]
    public async Task WhenIRequestInfo()
    {
        if (_api != null)
            _json = await _api.GetFoundationAsync();
    }

    [Then("the API returns {json}")]
    public void ThenApiReturns(string json)
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
