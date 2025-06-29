using System.Net;
using System.Net.Http;
using Xunit;

namespace Common.Tests.BDD.AppComponentInfo;

public class AppComponentInfoSteps
{
    private AppApi? _api;
    private string? _json;

    [Given("the app endpoint returns {json}")]
    public void GivenEndpoint(string json)
    {
        _api = new AppApi(new HttpClient(new JsonHandler(json)), "http://localhost");
    }

    [When("I request apps for space {spaceId}")]
    public async Task WhenIRequest(string spaceId)
    {
        if (_api != null)
            _json = await _api.GetAppsAsync(spaceId);
    }

    [Then("the app API returns {json}")]
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
