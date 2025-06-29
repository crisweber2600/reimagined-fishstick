using System.Net.Http;
using System.Threading.Tasks;

namespace Common;

public interface IFoundationApi
{
    Task<string> GetFoundationAsync();
}

public class FoundationApi : IFoundationApi
{
    private readonly HttpClient _client;
    private readonly string _endpoint;

    public FoundationApi(HttpClient client, string endpoint)
    {
        _client = client;
        _endpoint = endpoint;
    }

    /// TASK: Get foundation info as raw JSON
    public async Task<string> GetFoundationAsync()
    {
        var response = await _client.GetAsync(_endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
