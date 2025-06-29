using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        var content = new StringContent($"{{\"refresh_token\":\"{refreshToken}\"}}", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(_refreshEndpoint, content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<TokenModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return model ?? new TokenModel();
    }
}
