using System.Net;
using System.Net.Http;
using Xunit;

namespace Common.Tests.BDD.AuthTokenAcquisition;

public class AuthTokenAcquisitionSteps
{
    private AuthenticationService? _service;
    private TokenModel? _token;

    [Given("the auth endpoint returns {access_token} and {refresh_token}")]
    public void GivenAuthEndpointReturns(string access, string refresh)
    {
        _service = new AuthenticationService(
            new HttpClient(new JsonHandler($"{{\"access_token\":\"{access}\",\"refresh_token\":\"{refresh}\"}}")),
            "http://localhost/token");
    }

    [When("I request a token using {username} and {password}")]
    public async Task WhenIRequestToken(string user, string pass)
    {
        if (_service != null)
            _token = await _service.GetBearerTokenAsync(user, pass);
    }

    [Then("the AuthenticationService returns {access_token} and {refresh_token}")]
    public void ThenServiceReturns(string access, string refresh)
    {
        Assert.Equal(access, _token?.AccessToken);
        Assert.Equal(refresh, _token?.RefreshToken);
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
