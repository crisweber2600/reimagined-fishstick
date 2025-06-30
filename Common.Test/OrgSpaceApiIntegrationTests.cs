using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Common.Test;

public class OrgSpaceApiIntegrationTests
{
    [Fact]
    public async Task GetAllOrgsAsync_MergesPages()
    {
        var handler = new SequenceHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"resources\":[{\"id\":1}],\"pagination\":{\"next\":{\"href\":\"http://next\"}}}")
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"resources\":[{\"id\":2}],\"pagination\":{\"next\":{\"href\":null}}}")
            });
        var services = new ServiceCollection();
        services.AddSingleton<IOrgSpaceApi>(_ => new OrgSpaceApi(new HttpClient(handler), "http://localhost"));
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IOrgSpaceApi>();

        var json = await api.GetAllOrgsAsync();

        Assert.Contains("\"id\":1", json);
        Assert.Contains("\"id\":2", json);
    }
}
