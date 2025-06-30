using System.Net;
using System.Net.Http;

namespace Common;

public interface IOrgSpaceApi
{
    Task<string> GetAllOrgsAsync();
    Task<string> GetSpacesForOrgAsync(string orgId);
}

public class OrgSpaceApi : IOrgSpaceApi
{
    private readonly HttpClient _client;
    private readonly string _baseUri;

    public OrgSpaceApi(HttpClient client, string baseUri)
    {
        _client = client;
        _baseUri = baseUri.TrimEnd('/');
    }

    private async Task<string> GetWithRetryAsync(string uri)
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            var resp = await _client.GetAsync(uri);
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadAsStringAsync();
            if (attempt == 2 || (resp.StatusCode != HttpStatusCode.TooManyRequests && (int)resp.StatusCode < 500))
                resp.EnsureSuccessStatusCode();
            await Task.Delay(100);
        }
        return string.Empty;
    }

    /// TASK: Get all orgs as raw JSON with simple retry
    public Task<string> GetAllOrgsAsync() => GetWithRetryAsync($"{_baseUri}/v3/organizations");

    /// TASK: Get spaces for an org as raw JSON with simple retry
    public Task<string> GetSpacesForOrgAsync(string orgId) => GetWithRetryAsync($"{_baseUri}/v3/organizations/{orgId}/spaces");
}
