namespace Common;

public class TasClient : ITasClient
{
    private readonly IAuthenticationService _authService;
    private readonly IFoundationApi _foundationApi;
    private readonly IOrgSpaceApi _orgSpaceApi;
    private readonly IAppApi _appApi;
    private readonly IProcessApi _processApi;

    public TasClient(IAuthenticationService authService, IFoundationApi foundationApi, IOrgSpaceApi orgSpaceApi, IAppApi appApi, IProcessApi processApi)
    {
        _authService = authService;
        _foundationApi = foundationApi;
        _orgSpaceApi = orgSpaceApi;
        _appApi = appApi;
        _processApi = processApi;
    }

    /// TASK: Authenticate via AuthenticationService
    public Task<TokenModel> AuthenticateAsync(string username, string password)
        => _authService.GetBearerTokenAsync(username, password);

    /// TASK: Delegate to FoundationApi
    public Task<string> GetFoundationAsync()
        => _foundationApi.GetFoundationAsync();

    /// TASK: Delegate to OrgSpaceApi for orgs
    public Task<string> GetOrgsAsync()
        => _orgSpaceApi.GetAllOrgsAsync();

    /// TASK: Delegate to OrgSpaceApi for spaces
    public Task<string> GetSpacesAsync(string orgId)
        => _orgSpaceApi.GetSpacesForOrgAsync(orgId);

    /// TASK: Delegate to AppApi for apps
    public Task<string> GetAppsAsync(string spaceId)
        => _appApi.GetAppsAsync(spaceId);

    /// TASK: Delegate to ProcessApi for processes
    public Task<string> GetProcessesAsync(string appId)
        => _processApi.GetProcessesAsync(appId);
}
