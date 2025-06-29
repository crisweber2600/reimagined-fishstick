using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace Common.Test;

public class TasClientIntegrationTests
{
    [Fact]
    public async Task AuthenticateAsync_UsesAuthenticationService()
    {
        var authHandler = new SequenceHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"authorization_endpoint\":\"http://uaa\"}")
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"abc\",\"refresh_token\":\"xyz\",\"expires_in\":3600}")
            });

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(_ =>
            new AuthenticationService(new HttpClient(authHandler), "http://api"));
        services.AddSingleton<IFoundationApi>(_ =>
            new FoundationApi(new HttpClient(new Mock<HttpMessageHandler>().Object), "http://localhost/info"));
        services.AddSingleton<IOrgSpaceApi>(new Mock<IOrgSpaceApi>().Object);
        services.AddSingleton<IAppApi>(new Mock<IAppApi>().Object);
        services.AddSingleton<IProcessApi>(new Mock<IProcessApi>().Object);
        services.AddSingleton<ITasClient, TasClient>();
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ITasClient>();

        var token = await client.AuthenticateAsync("user", "pass");

        Assert.Equal("abc", token.AccessToken);
        Assert.Equal("xyz", token.RefreshToken);
    }

    [Fact]
    public void AddTasClient_RegistersDependencies()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"abc\",\"refresh_token\":\"xyz\"}")
            });

        var services = new ServiceCollection();
        services.AddTasClient(b => b
            .WithFoundationUri("https://api.tas")
            .WithCredentials("user","pass"));
        services.AddSingleton<IAuthenticationService>(new AuthenticationService(new HttpClient(handler.Object), "http://api"));
        var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredService<ITasClient>();
        var options = provider.GetRequiredService<TasClientOptions>();
        var api = provider.GetRequiredService<IFoundationApi>();
        var orgApi = provider.GetRequiredService<IOrgSpaceApi>();
        var appApi = provider.GetRequiredService<IAppApi>();

        Assert.NotNull(client);
        Assert.Equal("https://api.tas/", options.FoundationUri.ToString());
        Assert.NotNull(api);
        Assert.NotNull(orgApi);
        Assert.NotNull(appApi);
    }

    [Fact]
    public async Task TokenRefreshingHandler_RefreshesOnUnauthorized()
    {
        var refresher = new Mock<ITokenRefresher>();
        refresher.Setup(r => r.RefreshAsync(It.IsAny<string>()))
            .ReturnsAsync(new TokenModel { AccessToken = "new", RefreshToken = "r2", ExpiresAt = DateTime.UtcNow.AddMinutes(5) });

        var innerHandler = new Mock<HttpMessageHandler>();
        innerHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var handler = new TokenRefreshingHandler(refresher.Object, new TokenModel { AccessToken = "old", RefreshToken = "r1", ExpiresAt = DateTime.UtcNow.AddSeconds(-1) }, innerHandler.Object);
        var client = new HttpClient(handler);

        var resp = await client.GetAsync("http://example.com");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        refresher.Verify(r => r.RefreshAsync(It.IsAny<string>()), Times.AtLeastOnce());
    }

    [Fact]
    public async Task GetFoundationAsync_UsesFoundationApi()
    {
        var apiHandler = new Mock<HttpMessageHandler>();
        apiHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"name\":\"tas\"}")
            });

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(new Mock<IAuthenticationService>().Object);
        services.AddSingleton<IFoundationApi>(_ => new FoundationApi(new HttpClient(apiHandler.Object), "http://localhost/info"));
        services.AddSingleton<IOrgSpaceApi>(new Mock<IOrgSpaceApi>().Object);
        services.AddSingleton<IAppApi>(new Mock<IAppApi>().Object);
        services.AddSingleton<IProcessApi>(new Mock<IProcessApi>().Object);
        services.AddSingleton<ITasClient, TasClient>();
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ITasClient>();

        var json = await client.GetFoundationAsync();

        Assert.Equal("{\"name\":\"tas\"}", json);
    }

    [Fact]
    public async Task GetOrgsAndSpaces_UseOrgSpaceApi()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                var content = req.RequestUri!.AbsolutePath.Contains("spaces")
                    ? "{\"spaces\":\"s\"}" : "{\"orgs\":\"o\"}";
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(new Mock<IAuthenticationService>().Object);
        services.AddSingleton<IFoundationApi>(new Mock<IFoundationApi>().Object);
        services.AddSingleton<IOrgSpaceApi>(_ => new OrgSpaceApi(new HttpClient(handler.Object), "http://localhost"));
        services.AddSingleton<IAppApi>(new Mock<IAppApi>().Object);
        services.AddSingleton<IProcessApi>(new Mock<IProcessApi>().Object);
        services.AddSingleton<ITasClient, TasClient>();
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ITasClient>();

        var orgs = await client.GetOrgsAsync();
        var spaces = await client.GetSpacesAsync("org1");

        Assert.Equal("{\"orgs\":\"o\"}", orgs);
        Assert.Equal("{\"spaces\":\"s\"}", spaces);
    }

    [Fact]
    public void MathProcessor_Sum_ReturnsExpected()
    {
        var processor = new MathProcessor(new Calculator());
        var result = processor.Sum(2, 3);
        Assert.Equal(5, result);
    }
}

public class SequenceHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses;
    public HttpRequestMessage? LastRequest { get; private set; }
    public SequenceHandler(params HttpResponseMessage[] responses) => _responses = new Queue<HttpResponseMessage>(responses);
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(_responses.Dequeue());
    }
}

