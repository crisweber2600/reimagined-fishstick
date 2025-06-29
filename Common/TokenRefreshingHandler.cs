using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;

namespace Common;

public class TokenRefreshingHandler : DelegatingHandler
{
    private readonly ITokenRefresher _refresher;
    private TokenModel _token;
    private readonly SemaphoreSlim _lock = new(1,1);

    public TokenRefreshingHandler(ITokenRefresher refresher, TokenModel token, HttpMessageHandler? innerHandler = null)
    {
        _refresher = refresher;
        _token = token;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await EnsureValidTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response.Dispose();
            await ForceRefreshAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
            response = await base.SendAsync(request, cancellationToken);
        }
        return response;
    }

    private async Task EnsureValidTokenAsync(CancellationToken ct)
    {
        if (_token.ExpiresAt > DateTime.UtcNow)
            return;
        await ForceRefreshAsync(ct);
    }

    private async Task ForceRefreshAsync(CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            if (_token.ExpiresAt <= DateTime.UtcNow)
                _token = await _refresher.RefreshAsync(_token.RefreshToken);
        }
        finally
        {
            _lock.Release();
        }
    }
}
