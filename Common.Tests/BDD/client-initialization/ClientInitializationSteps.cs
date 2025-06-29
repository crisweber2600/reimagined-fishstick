using System;
using Xunit;

namespace Common.Tests.BDD.ClientInitialization;

public class ClientInitializationSteps
{
    private TasClientBuilder? _builder;
    private ITasClient? _client;

    [Given("I configure TasClient with foundation URI {uri}, username {username}, password {password}")]
    public void GivenIConfigureTasClient(string uri, string username, string password)
    {
        _builder = new TasClientBuilder()
            .WithFoundationUri(uri)
            .WithCredentials(username, password);
    }

    [When("I build the TasClient")]
    public void WhenIBuildTheTasClient()
    {
        _client = _builder?.Build();
    }

    [Then("the client is initialized successfully")]
    public void ThenTheClientIsInitializedSuccessfully()
    {
        Assert.NotNull(_client);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class GivenAttribute : Attribute
{
    public GivenAttribute(string text) { }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class WhenAttribute : Attribute
{
    public WhenAttribute(string text) { }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ThenAttribute : Attribute
{
    public ThenAttribute(string text) { }
}
