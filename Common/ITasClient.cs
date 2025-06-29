namespace Common;

public interface ITasClient
{
    Task<TokenModel> AuthenticateAsync(string username, string password);
}
