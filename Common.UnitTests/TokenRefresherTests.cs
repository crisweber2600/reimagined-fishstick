using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class TokenRefresherTests
{
    [Fact]
    public async Task RefreshAsync_ReturnsNewToken()
    {
        var handler = new Mock<HttpMessageHandler>();
        HttpRequestMessage? captured = null;
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((m, _) => captured = m)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"newabc\",\"refresh_token\":\"newxyz\",\"expires_in\":3600}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<ITokenRefresher>(_ => new TokenRefresher(new HttpClient(handler.Object), "http://uaa/oauth/token"));
        var provider = services.BuildServiceProvider();
        var refresher = provider.GetRequiredService<ITokenRefresher>();

        var token = await refresher.RefreshAsync("oldrefresh");

        Assert.Equal("newabc", token.AccessToken);
        Assert.Equal("newxyz", token.RefreshToken);
        Assert.Equal("Basic", captured?.Headers.Authorization?.Scheme);
        Assert.Equal("Y2Y6", captured?.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task RefreshAsync_ThrowsWhenUnauthorized()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var services = new ServiceCollection();
        services.AddSingleton<ITokenRefresher>(_ => new TokenRefresher(new HttpClient(handler.Object), "http://uaa/oauth/token"));
        var provider = services.BuildServiceProvider();
        var refresher = provider.GetRequiredService<ITokenRefresher>();

        await Assert.ThrowsAsync<HttpRequestException>(() => refresher.RefreshAsync("badrefresh"));
    }
}
