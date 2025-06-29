namespace Common;

public class TasClient : ITasClient
{
    private readonly IAuthenticationService _authService;

    public TasClient(IAuthenticationService authService)
    {
        _authService = authService;
    }

    /// TASK: Authenticate via AuthenticationService
    public Task<TokenModel> AuthenticateAsync(string username, string password)
        => _authService.GetBearerTokenAsync(username, password);
}
