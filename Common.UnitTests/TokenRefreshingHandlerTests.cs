using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.DependencyInjection;

namespace Common.UnitTests;

public class TokenRefreshingHandlerTests
{
    [Fact]
    public async Task SendsRefreshRequestOnUnauthorized()
    {
        var refreshHandler = new Mock<HttpMessageHandler>();
        refreshHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"new\",\"refresh_token\":\"r2\"}")
            });

        var services = new ServiceCollection();
        services.AddSingleton<ITokenRefresher>(_ =>
            new TokenRefresher(new HttpClient(refreshHandler.Object), "http://localhost/refresh"));
        var provider = services.BuildServiceProvider();
        var refresher = provider.GetRequiredService<ITokenRefresher>();

        var innerHandler = new Mock<HttpMessageHandler>();
        innerHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var handler = new TokenRefreshingHandler(refresher, new TokenModel { AccessToken = "old", RefreshToken = "r1" }, innerHandler.Object);
        var client = new HttpClient(handler);

        var resp = await client.GetAsync("http://example.com");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        refreshHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }
}
