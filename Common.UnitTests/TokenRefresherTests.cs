using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.DependencyInjection;

namespace Common.UnitTests;

public class TokenRefresherTests
{
    [Fact]
    public async Task RefreshAsync_ReturnsNewToken()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"newabc\",\"refresh_token\":\"newxyz\"}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<ITokenRefresher>(_ =>
            new TokenRefresher(new HttpClient(handler.Object), "http://localhost/refresh"));
        var provider = services.BuildServiceProvider();
        var refresher = provider.GetRequiredService<ITokenRefresher>();

        var token = await refresher.RefreshAsync("oldrefresh");

        Assert.Equal("newabc", token.AccessToken);
        Assert.Equal("newxyz", token.RefreshToken);
    }

    [Fact]
    public async Task RefreshAsync_ThrowsWhenUnauthorized()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var services = new ServiceCollection();
        services.AddSingleton<ITokenRefresher>(_ =>
            new TokenRefresher(new HttpClient(handler.Object), "http://localhost/refresh"));
        var provider = services.BuildServiceProvider();
        var refresher = provider.GetRequiredService<ITokenRefresher>();

        await Assert.ThrowsAsync<HttpRequestException>(() => refresher.RefreshAsync("badrefresh"));
    }
}
