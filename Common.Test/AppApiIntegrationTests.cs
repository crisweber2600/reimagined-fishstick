using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Xunit;

namespace Common.Test;

public class AppApiIntegrationTests
{
    [Fact]
    public async Task GetAppsAsync_ReturnsJson()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"apps\":\"a\"}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<IAppApi>(_ => new AppApi(new HttpClient(handler.Object), "http://localhost"));
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IAppApi>();

        var json = await api.GetAppsAsync("space1");

        Assert.Equal("{\"apps\":\"a\"}", json);
    }

    [Fact]
    public async Task GetAppsAsync_ThrowsOnNotFound()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
        var services = new ServiceCollection();
        services.AddSingleton<IAppApi>(_ => new AppApi(new HttpClient(handler.Object), "http://localhost"));
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IAppApi>();

        await Assert.ThrowsAsync<HttpRequestException>(() => api.GetAppsAsync("space1"));
    }
}
