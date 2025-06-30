namespace Common;

public interface IAuthenticationService
{
    Task<TokenModel> GetBearerTokenAsync(string username, string password);
    Task<TokenModel> RefreshAsync(string refreshToken);
}
