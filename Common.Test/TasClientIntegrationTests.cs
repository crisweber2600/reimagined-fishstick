using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace Common.Test;

public class TasClientIntegrationTests
{
    [Fact]
    public async Task AuthenticateAsync_UsesAuthenticationService()
    {
        var authHandler = new Mock<HttpMessageHandler>();
        authHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"abc\",\"refresh_token\":\"xyz\"}")
            });

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(_ =>
            new AuthenticationService(new HttpClient(authHandler.Object), "http://localhost/token"));
        services.AddSingleton<ITasClient, TasClient>();
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ITasClient>();

        var token = await client.AuthenticateAsync("user", "pass");

        Assert.Equal("abc", token.AccessToken);
        Assert.Equal("xyz", token.RefreshToken);
    }

    [Fact]
    public void AddTasClient_RegistersDependencies()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"abc\",\"refresh_token\":\"xyz\"}")
            });

        var services = new ServiceCollection();
        services.AddTasClient(b => b
            .WithFoundationUri("https://api.tas")
            .WithCredentials("user","pass"));
        services.AddSingleton<IAuthenticationService>(new AuthenticationService(new HttpClient(handler.Object), "http://localhost/token"));
        var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredService<ITasClient>();
        var options = provider.GetRequiredService<TasClientOptions>();

        Assert.NotNull(client);
        Assert.Equal("https://api.tas/", options.FoundationUri.ToString());
    }

    [Fact]
    public async Task TokenRefreshingHandler_RefreshesOnUnauthorized()
    {
        var refresher = new Mock<ITokenRefresher>();
        refresher.Setup(r => r.RefreshAsync("r1"))
            .ReturnsAsync(new TokenModel { AccessToken = "new", RefreshToken = "r2" });

        var innerHandler = new Mock<HttpMessageHandler>();
        innerHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var handler = new TokenRefreshingHandler(refresher.Object, new TokenModel { AccessToken = "old", RefreshToken = "r1" }, innerHandler.Object);
        var client = new HttpClient(handler);

        var resp = await client.GetAsync("http://example.com");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        refresher.Verify(r => r.RefreshAsync("r1"), Times.Once);
    }

    [Fact]
    public void MathProcessor_Sum_ReturnsExpected()
    {
        var processor = new MathProcessor(new Calculator());
        var result = processor.Sum(2, 3);
        Assert.Equal(5, result);
    }
}
