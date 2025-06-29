using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common;

public interface ITokenRefresher
{
    Task<TokenModel> RefreshAsync(string refreshToken);
}

public class TokenRefresher : ITokenRefresher
{
    private readonly HttpClient _client;
    private readonly string _refreshEndpoint;

    public TokenRefresher(HttpClient client, string refreshEndpoint)
    {
        _client = client;
        _refreshEndpoint = refreshEndpoint;
    }

    /// TASK: Refresh token using refresh token
    public async Task<TokenModel> RefreshAsync(string refreshToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _refreshEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "Y2Y6");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        });
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<TokenModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new TokenModel();
        model.SetExpiration();
        return model;
    }
}
