using System.Net;
using System.Net.Http;
using Xunit;
using Moq;
using Moq.Protected;

namespace Common.Tests.BDD.TokenEndpointDiscovery;

public class TokenEndpointDiscoverySteps
{
    private AuthenticationService? _service;
    private Mock<HttpMessageHandler>? _handler;
    private string? _requestUri;

    [Given("the info endpoint returns {token_url}")]
    public void GivenInfoEndpointReturns(string tokenUrl)
    {
        _handler = new Mock<HttpMessageHandler>();
        var seq = new MockSequence();
        _handler.Protected().InSequence(seq)
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.AbsolutePath.EndsWith("/info")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"{{\"token_endpoint\":\"{tokenUrl}\"}}")
            });
        _handler.Protected().InSequence(seq)
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => { _requestUri = r.RequestUri!.ToString(); return true; }), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"a\",\"refresh_token\":\"r\",\"expires_in\":3600\"}")
            });
        _service = new AuthenticationService(new HttpClient(_handler.Object), "http://api.example.com");
    }

    [When("I authenticate with {username} and {password}")]
    public async Task WhenAuthenticate(string user, string pass)
    {
        if (_service != null)
            _ = await _service.GetBearerTokenAsync(user, pass);
    }

    [Then("the token request is sent to {expected}")]
    public void ThenRequestSent(string expected)
    {
        Assert.Equal(expected, _requestUri);
    }
}

public class GivenAttribute : Attribute { public GivenAttribute(string t) { } }
public class WhenAttribute : Attribute { public WhenAttribute(string t) { } }
public class ThenAttribute : Attribute { public ThenAttribute(string t) { } }
