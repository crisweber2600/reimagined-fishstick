using System.Net.Http;

namespace Common;

public interface IProcessApi
{
    Task<string> GetProcessesAsync(string appId);
}

public class ProcessApi : IProcessApi
{
    private readonly HttpClient _client;
    private readonly string _baseUri;

    public ProcessApi(HttpClient client, string baseUri)
    {
        _client = client;
        _baseUri = baseUri.TrimEnd('/');
    }

    /// TASK: Get processes for an app as raw JSON
    public async Task<string> GetProcessesAsync(string appId)
    {
        var resp = await _client.GetAsync($"{_baseUri}/v3/apps/{appId}/processes");
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync();
    }
}
