using System.Net;
using System.Net.Http.Headers;

namespace Common;

public class TokenRefreshingHandler : DelegatingHandler
{
    private readonly ITokenRefresher _refresher;
    private TokenModel _token;

    public TokenRefreshingHandler(ITokenRefresher refresher, TokenModel token, HttpMessageHandler? innerHandler = null)
    {
        _refresher = refresher;
        _token = token;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response.Dispose();
            _token = await _refresher.RefreshAsync(_token.RefreshToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
            response = await base.SendAsync(request, cancellationToken);
        }
        return response;
    }
}
