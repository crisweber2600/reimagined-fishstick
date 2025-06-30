using System.Net;
using System.Net.Http;
using Xunit;

namespace Common.Tests.BDD.OrgSpaceInfo;

public class OrgSpaceInfoSteps
{
    private OrgSpaceApi? _api;
    private string? _json;

    [Given("the endpoint for {kind} returns {json}")]
    public void GivenEndpoint(string kind, string json)
    {
        _api = new OrgSpaceApi(new HttpClient(new JsonHandler(json)), "http://localhost");
        _kind = kind;
    }

    private string? _kind;

    [When("I request {kind}")]
    public async Task WhenIRequest(string kind)
    {
        if (_api != null)
            _json = kind == "organizations" ? await _api.GetAllOrgsAsync() : await _api.GetSpacesForOrgAsync("org1");
    }

    [Then("I receive {json}")]
    public void ThenReceive(string json)
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
