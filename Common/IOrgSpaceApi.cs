using System.Net;
using System.Net.Http;
using System.Text.Json.Nodes;

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

    private async Task<string> GetPagedJsonAsync(string url)
    {
        JsonObject? root = null;
        var all = new JsonArray();
        var next = url;
        var firstJson = await GetWithRetryAsync(next);
        var node = JsonNode.Parse(firstJson)!.AsObject();
        if (node["pagination"] is null)
            return firstJson;
        root = node;
        if (node["resources"] is JsonArray firstItems)
            foreach (var r in firstItems)
                all.Add(r.DeepClone());
        next = node["pagination"]?["next"]?["href"]?.GetValue<string>();
        while (!string.IsNullOrEmpty(next))
        {
            var json = await GetWithRetryAsync(next);
            var page = JsonNode.Parse(json)!.AsObject();
            if (page["resources"] is JsonArray items)
                foreach (var r in items)
                    all.Add(r.DeepClone());
            next = page["pagination"]?["next"]?["href"]?.GetValue<string>();
        }

        root["resources"] = all;
        if (root["pagination"]?["next"] is JsonObject nextObj)
            nextObj["href"] = null;

        return root.ToJsonString();
    }

    /// TASK: Get all orgs as raw JSON with pagination
    public Task<string> GetAllOrgsAsync() => GetPagedJsonAsync($"{_baseUri}/v3/organizations");

    /// TASK: Get spaces for an org as raw JSON with pagination
    public Task<string> GetSpacesForOrgAsync(string orgId) => GetPagedJsonAsync($"{_baseUri}/v3/organizations/{orgId}/spaces");
}
