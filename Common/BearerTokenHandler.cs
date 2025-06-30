using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Common;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly ITokenCache _cache;
    public BearerTokenHandler(ITokenCache cache, HttpMessageHandler? innerHandler = null)
    {
        _cache = cache;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _cache.Current;
        if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return base.SendAsync(request, cancellationToken);
    }
}
