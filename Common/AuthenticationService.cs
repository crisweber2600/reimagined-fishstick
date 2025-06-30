using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _client;
    private readonly string _foundationUri;

    public AuthenticationService(HttpClient client, string foundationUri)
    {
        _client = client;
        _foundationUri = foundationUri.TrimEnd('/');
    }

    /// TASK: Acquire bearer token from TAS
    public async Task<TokenModel> GetBearerTokenAsync(string username, string password)
    {
        var infoResp = await _client.GetAsync($"{_foundationUri}/info");
        infoResp.EnsureSuccessStatusCode();
        var infoJson = await infoResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(infoJson);
        var endpoint = doc.RootElement.TryGetProperty("token_endpoint", out var te)
            ? te.GetString()
            : doc.RootElement.GetProperty("authorization_endpoint").GetString();
        var tokenUri = $"{endpoint?.TrimEnd('/')}/oauth/token";

        var request = new HttpRequestMessage(HttpMethod.Post, tokenUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "Y2Y6");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["username"] = username,
            ["password"] = password
        });

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<TokenModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new TokenModel();
        model.SetExpiration();
        return model;
    }

    /// TASK: Refresh bearer token using refresh token
    public async Task<TokenModel> RefreshAsync(string refreshToken)
    {
        var infoResp = await _client.GetAsync($"{_foundationUri}/info");
        infoResp.EnsureSuccessStatusCode();
        var infoJson = await infoResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(infoJson);
        var endpoint = doc.RootElement.TryGetProperty("token_endpoint", out var te)
            ? te.GetString()
            : doc.RootElement.GetProperty("authorization_endpoint").GetString();
        var tokenUri = $"{endpoint?.TrimEnd('/')}/oauth/token";

        var request = new HttpRequestMessage(HttpMethod.Post, tokenUri);
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
