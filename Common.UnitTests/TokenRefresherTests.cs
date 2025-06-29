using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

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
        var client = new HttpClient(handler.Object);
        var refresher = new TokenRefresher(client, "http://localhost/refresh");

        var token = await refresher.RefreshAsync("oldrefresh");

        Assert.Equal("newabc", token.AccessToken);
        Assert.Equal("newxyz", token.RefreshToken);
    }
}
