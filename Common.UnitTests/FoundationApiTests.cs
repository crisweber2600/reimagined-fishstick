using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class FoundationApiTests
{
    [Fact]
    public async Task GetFoundationAsync_ReturnsJson()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"name\":\"tas\"}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<IFoundationApi>(_ => new FoundationApi(new HttpClient(handler.Object), "http://localhost/info"));
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IFoundationApi>();

        var json = await api.GetFoundationAsync();

        Assert.Equal("{\"name\":\"tas\"}", json);
    }
}
