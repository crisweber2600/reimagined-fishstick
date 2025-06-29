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
}
