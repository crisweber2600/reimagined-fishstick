namespace Common;

public class TasClient : ITasClient
{
    private readonly IAuthenticationService _authService;
    private readonly IFoundationApi _foundationApi;

    public TasClient(IAuthenticationService authService, IFoundationApi foundationApi)
    {
        _authService = authService;
        _foundationApi = foundationApi;
    }

    /// TASK: Authenticate via AuthenticationService
    public Task<TokenModel> AuthenticateAsync(string username, string password)
        => _authService.GetBearerTokenAsync(username, password);

    /// TASK: Delegate to FoundationApi
    public Task<string> GetFoundationAsync()
        => _foundationApi.GetFoundationAsync();
}
