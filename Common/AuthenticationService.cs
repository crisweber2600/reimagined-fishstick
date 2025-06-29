using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common;

public class AuthenticationService
{
    private readonly HttpClient _client;
    private readonly string _authEndpoint;

    public AuthenticationService(HttpClient client, string authEndpoint)
    {
        _client = client;
        _authEndpoint = authEndpoint;
    }

    /// TASK: Acquire bearer token from TAS
    public async Task<TokenModel> GetBearerTokenAsync(string username, string password)
    {
        var content = new StringContent($"{{\"username\":\"{username}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(_authEndpoint, content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<TokenModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return model ?? new TokenModel();
    }
}
