using Microsoft.Extensions.DependencyInjection;

namespace Common.UnitTests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTasClient_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddTasClient(b => b
            .WithFoundationUri("https://api.tas")
            .WithCredentials("user","pass"));
        var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredService<ITasClient>();
        var auth = provider.GetRequiredService<IAuthenticationService>();
        var api = provider.GetRequiredService<IFoundationApi>();

        Assert.NotNull(client);
        Assert.NotNull(auth);
        Assert.NotNull(api);
    }
}
