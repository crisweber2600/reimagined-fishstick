using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Common.UnitTests;

public class BearerTokenHandlerTests
{
    [Fact]
    public async Task AddsAuthorizationHeader()
    {
        var cache = new Mock<ITokenCache>();
        cache.SetupGet(c => c.Current).Returns(new TokenModel { AccessToken = "abc" });
        HttpRequestMessage? captured = null;
        var inner = new Mock<HttpMessageHandler>();
        inner.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((m, _) => captured = m)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var handler = new BearerTokenHandler(cache.Object, inner.Object);
        var client = new HttpClient(handler);
        await client.GetAsync("http://example.com");

        Assert.Equal("Bearer", captured?.Headers.Authorization?.Scheme);
        Assert.Equal("abc", captured?.Headers.Authorization?.Parameter);
    }
}
