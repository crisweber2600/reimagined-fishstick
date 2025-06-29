using System;
using System.Net.Http;

namespace Common;

public class TasClientBuilder
{
    private readonly TasClientOptions _options = new();
    public TasClientOptions Options => _options;
    public IAuthenticationService? AuthenticationService { get; private set; }

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
        return new TasClient(AuthenticationService);
    }
}
