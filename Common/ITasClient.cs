namespace Common;

public interface ITasClient
{
    Task<TokenModel> AuthenticateAsync(string username, string password);

    /// TASK: Retrieve foundation information as raw JSON
    Task<string> GetFoundationAsync();
}
