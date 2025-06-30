using Microsoft.Extensions.DependencyInjection;

namespace Common.UnitTests;

public class TasClientBuilderTests
{
    [Fact]
    public void Build_ReturnsClient()
    {
        var builder = new TasClientBuilder()
            .WithFoundationUri("https://api.tas")
            .WithCredentials("user","pass");

        var client = builder.Build();

        Assert.NotNull(client);
        Assert.Same(builder.HttpClient, typeof(FoundationApi).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(builder.FoundationApi));
        Assert.Same(builder.HttpClient, typeof(OrgSpaceApi).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(builder.OrgSpaceApi));
        Assert.Same(builder.HttpClient, typeof(AppApi).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(builder.AppApi));
        Assert.Same(builder.HttpClient, typeof(ProcessApi).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(builder.ProcessApi));
    }

    [Fact]
    public void Build_ThrowsWithoutUsername()
    {
        var builder = new TasClientBuilder()
            .WithFoundationUri("https://api.tas");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }
}
