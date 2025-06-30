using System.Net.Http;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Common;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly ITokenCache _cache;
    private readonly IAuthenticationService _auth;
    private readonly SemaphoreSlim _lock = new(1,1);

    public BearerTokenHandler(ITokenCache cache, IAuthenticationService auth, HttpMessageHandler? innerHandler = null)
    {
        _cache = cache;
        _auth = auth;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await EnsureValidTokenAsync(cancellationToken);
        var token = _cache.Current;
        if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized && token != null && token.ExpiresAt > DateTime.UtcNow)
        {
            response.Dispose();
            await ForceRefreshAsync(true, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _cache.Current?.AccessToken);
            response = await base.SendAsync(request, cancellationToken);
        }
        return response;
    }

    private async Task EnsureValidTokenAsync(CancellationToken ct)
    {
        var token = _cache.Current;
        if (token == null)
            return;
        if (token.ExpiresAt > DateTime.UtcNow.AddMinutes(1))
            return;
        await ForceRefreshAsync(false, ct);
    }

    private async Task ForceRefreshAsync(bool ignoreExpiry, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var token = _cache.Current;
            if (token != null && (ignoreExpiry || token.ExpiresAt <= DateTime.UtcNow.AddMinutes(1)))
            {
                var newToken = await _auth.RefreshAsync(token.RefreshToken);
                _cache.Current = newToken;
            }
        }
        finally
        {
            _lock.Release();
        }
    }
}
