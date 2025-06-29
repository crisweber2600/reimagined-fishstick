using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class TokenRefreshingHandlerTests
{
    [Fact]
    public async Task SendsRefreshRequestOnUnauthorized()
    {
        var refresher = new Mock<ITokenRefresher>();
        refresher.Setup(r => r.RefreshAsync("r1"))
            .ReturnsAsync(new TokenModel { AccessToken = "new", RefreshToken = "r2" });

        var innerHandler = new Mock<HttpMessageHandler>();
        innerHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var handler = new TokenRefreshingHandler(refresher.Object, new TokenModel { AccessToken = "old", RefreshToken = "r1" }, innerHandler.Object);
        var client = new HttpClient(handler);

        var resp = await client.GetAsync("http://example.com");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        refresher.Verify(r => r.RefreshAsync("r1"), Times.Once);
    }
}
