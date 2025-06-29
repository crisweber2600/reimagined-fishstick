using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class ProcessApiTests
{
    [Fact]
    public async Task GetProcessesAsync_ReturnsJson()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"procs\":\"p\"}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<IProcessApi>(_ => new ProcessApi(new HttpClient(handler.Object), "http://localhost"));
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IProcessApi>();

        var json = await api.GetProcessesAsync("app1");

        Assert.Equal("{\"procs\":\"p\"}", json);
    }

    [Fact]
    public async Task GetProcessesAsync_RefreshesTokenOnUnauthorized()
    {
        var refreshHandler = new Mock<HttpMessageHandler>();
        refreshHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"new\",\"refresh_token\":\"r2\",\"expires_in\":3600}")
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
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"procs\":\"p\"}")
            });

        var handler = new TokenRefreshingHandler(refresher, new TokenModel { AccessToken = "old", RefreshToken = "r1" }, innerHandler.Object);
        var api = new ProcessApi(new HttpClient(handler), "http://localhost");

        var json = await api.GetProcessesAsync("app1");

        Assert.Equal("{\"procs\":\"p\"}", json);
        refreshHandler.Protected().Verify("SendAsync", Times.AtLeastOnce(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }
}
