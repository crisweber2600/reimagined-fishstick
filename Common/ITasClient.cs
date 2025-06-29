namespace Common;

public interface ITasClient
{
    Task<TokenModel> AuthenticateAsync(string username, string password);

    /// TASK: Retrieve foundation information as raw JSON
    Task<string> GetFoundationAsync();

    /// TASK: Retrieve all orgs as raw JSON
    Task<string> GetOrgsAsync();

    /// TASK: Retrieve spaces for a specific org as raw JSON
    Task<string> GetSpacesAsync(string orgId);
}
