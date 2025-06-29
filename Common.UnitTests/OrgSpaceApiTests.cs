using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class OrgSpaceApiTests
{
    [Fact]
    public async Task GetAllOrgsAsync_ReturnsJson()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"orgs\":\"o\"}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<IOrgSpaceApi>(_ => new OrgSpaceApi(new HttpClient(handler.Object), "http://localhost"));
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IOrgSpaceApi>();

        var json = await api.GetAllOrgsAsync();

        Assert.Equal("{\"orgs\":\"o\"}", json);
    }

    [Fact]
    public async Task GetSpacesForOrgAsync_RetriesOnFailure()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.TooManyRequests))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"spaces\":\"s\"}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<IOrgSpaceApi>(_ => new OrgSpaceApi(new HttpClient(handler.Object), "http://localhost"));
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IOrgSpaceApi>();

        var json = await api.GetSpacesForOrgAsync("org1");

        Assert.Equal("{\"spaces\":\"s\"}", json);
        handler.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }
}
