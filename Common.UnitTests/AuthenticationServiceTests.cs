using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task GetBearerTokenAsync_ReturnsToken()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"abc\",\"refresh_token\":\"xyz\"}")
            });
        var client = new HttpClient(handler.Object);
        var service = new AuthenticationService(client, "http://localhost/token");

        var token = await service.GetBearerTokenAsync("user", "pass");

        Assert.Equal("abc", token.AccessToken);
        Assert.Equal("xyz", token.RefreshToken);
    }
}
