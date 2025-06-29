using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Common.UnitTests;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task GetBearerTokenAsync_ReturnsToken()
    {
        var handler = new SequenceHandler(
            new HttpResponseMessage(HttpStatusCode.OK){Content = new StringContent("{\"token_endpoint\":\"http://uaa\"}")},
            new HttpResponseMessage(HttpStatusCode.OK){Content = new StringContent("{\"access_token\":\"abc\",\"refresh_token\":\"xyz\",\"expires_in\":3600}")});
        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(_ => new AuthenticationService(new HttpClient(handler), "http://api"));
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IAuthenticationService>();

        var token = await service.GetBearerTokenAsync("user", "pass");

        Assert.Equal("abc", token.AccessToken);
        Assert.Equal("xyz", token.RefreshToken);
    }
}

public class SequenceHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses;
    public SequenceHandler(params HttpResponseMessage[] responses) => _responses = new Queue<HttpResponseMessage>(responses);
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responses.Dequeue());
}
