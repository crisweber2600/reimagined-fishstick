namespace Common;

public class TasClient
{
    private readonly AuthenticationService _authService;

    public TasClient(AuthenticationService authService)
    {
        _authService = authService;
    }

    /// TASK: Authenticate via AuthenticationService
    public Task<TokenModel> AuthenticateAsync(string username, string password)
        => _authService.GetBearerTokenAsync(username, password);
}
