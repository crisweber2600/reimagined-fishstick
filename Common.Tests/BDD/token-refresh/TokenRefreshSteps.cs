using System.Net;
using System.Net.Http;
using Xunit;

namespace Common.Tests.BDD.TokenRefresh;

public class TokenRefreshSteps
{
    private TokenRefresher? _refresher;
    private TokenModel? _token;

    [Given("the refresh endpoint returns {access_token} and {new_refresh_token}")]
    public void GivenRefreshEndpoint(string access, string newRefresh)
    {
        _refresher = new TokenRefresher(
            new HttpClient(new JsonHandler($"{{\"access_token\":\"{access}\",\"refresh_token\":\"{newRefresh}\"}}")),
            "http://localhost/refresh");
    }

    [When("I refresh using {current_refresh_token}")]
    public async Task WhenIRefresh(string refresh)
    {
        if (_refresher != null)
            _token = await _refresher.RefreshAsync(refresh);
    }

    [Then("the TokenRefresher returns {access_token} and {new_refresh_token}")]
    public void ThenReturns(string access, string newRefresh)
    {
        Assert.Equal(access, _token?.AccessToken);
        Assert.Equal(newRefresh, _token?.RefreshToken);
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
