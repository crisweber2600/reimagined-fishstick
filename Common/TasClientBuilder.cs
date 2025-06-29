using System;
using System.Net.Http;

namespace Common;

public class TasClientBuilder
{
    private readonly TasClientOptions _options = new();
    public TasClientOptions Options => _options;
    public IAuthenticationService? AuthenticationService { get; private set; }
    public IFoundationApi? FoundationApi { get; private set; }
    public IOrgSpaceApi? OrgSpaceApi { get; private set; }
    public IAppApi? AppApi { get; private set; }

    public TasClientBuilder WithFoundationUri(string uri)
    {
        _options.FoundationUri = new Uri(uri);
        return this;
    }

    public TasClientBuilder WithCredentials(string username, string password)
    {
        _options.Username = username;
        _options.Password = password;
        return this;
    }

    /// TASK: Build TasClient using provided options
    public TasClient Build()
    {
        _options.Validate();
        AuthenticationService = new AuthenticationService(new HttpClient(), $"{_options.FoundationUri}/oauth/token");
        FoundationApi = new FoundationApi(new HttpClient(), $"{_options.FoundationUri}/v3/info");
        OrgSpaceApi = new OrgSpaceApi(new HttpClient(), _options.FoundationUri.ToString());
        AppApi = new AppApi(new HttpClient(), _options.FoundationUri.ToString());
        return new TasClient(AuthenticationService, FoundationApi, OrgSpaceApi, AppApi);
    }
}
