using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class BearerTokenHandlerTests
{
    [Fact]
    public async Task AddsAuthorizationHeader()
    {
        var cache = new TokenCache();
        cache.Current = new TokenModel { AccessToken = "abc", ExpiresAt = DateTime.UtcNow.AddMinutes(5) };
        var auth = new Mock<IAuthenticationService>();
        HttpRequestMessage? captured = null;
        var inner = new Mock<HttpMessageHandler>();
        inner.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((m, _) => captured = m)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var handler = new BearerTokenHandler(cache, auth.Object, inner.Object);
        var client = new HttpClient(handler);
        await client.GetAsync("http://example.com");

        Assert.Equal("Bearer", captured?.Headers.Authorization?.Scheme);
        Assert.Equal("abc", captured?.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task RefreshesExpiredToken()
    {
        var cache = new TokenCache();
        cache.Current = new TokenModel { AccessToken = "old", RefreshToken = "r1", ExpiresAt = DateTime.UtcNow }; // expired
        var auth = new Mock<IAuthenticationService>();
        auth.Setup(a => a.RefreshAsync("r1")).ReturnsAsync(new TokenModel { AccessToken = "new", RefreshToken = "r2", ExpiresAt = DateTime.UtcNow.AddMinutes(5) });
        HttpRequestMessage? captured = null;
        var inner = new Mock<HttpMessageHandler>();
        inner.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((m, _) => captured = m)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var handler = new BearerTokenHandler(cache, auth.Object, inner.Object);
        var client = new HttpClient(handler);
        await client.GetAsync("http://example.com");

        Assert.Equal("new", captured?.Headers.Authorization?.Parameter);
        auth.Verify(a => a.RefreshAsync("r1"), Times.Once());
    }

    [Fact]
    public async Task RefreshesOnUnauthorized()
    {
        var cache = new TokenCache();
        cache.Current = new TokenModel { AccessToken = "old", RefreshToken = "r1", ExpiresAt = DateTime.UtcNow.AddMinutes(5) };
        var auth = new Mock<IAuthenticationService>();
        auth.Setup(a => a.RefreshAsync("r1")).ReturnsAsync(new TokenModel { AccessToken = "new", RefreshToken = "r2", ExpiresAt = DateTime.UtcNow.AddMinutes(5) });
        string? firstAuth = null; string? secondAuth = null; int call = 0;
        var inner = new Mock<HttpMessageHandler>();
        inner.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns<HttpRequestMessage, CancellationToken>((m, _) =>
            {
                call++;
                if (call == 1)
                {
                    firstAuth = m.Headers.Authorization?.Parameter;
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }
                secondAuth = m.Headers.Authorization?.Parameter;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

        var handler = new BearerTokenHandler(cache, auth.Object, inner.Object);
        var client = new HttpClient(handler);
        var resp = await client.GetAsync("http://example.com");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.Equal("old", firstAuth);
        Assert.Equal("new", secondAuth);
        auth.Verify(a => a.RefreshAsync("r1"), Times.Once());
    }
}
