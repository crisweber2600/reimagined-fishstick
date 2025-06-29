using System.Net.Http;

namespace Common;

public interface IAppApi
{
    Task<string> GetAppsAsync(string spaceId);
}

public class AppApi : IAppApi
{
    private readonly HttpClient _client;
    private readonly string _baseUri;

    public AppApi(HttpClient client, string baseUri)
    {
        _client = client;
        _baseUri = baseUri.TrimEnd('/');
    }

    /// TASK: Get apps for a space as raw JSON
    public async Task<string> GetAppsAsync(string spaceId)
    {
        var resp = await _client.GetAsync($"{_baseUri}/v3/spaces/{spaceId}/apps");
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync();
    }
}
