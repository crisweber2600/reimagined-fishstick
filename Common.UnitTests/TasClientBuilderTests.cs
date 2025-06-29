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
    }

    [Fact]
    public void Build_ThrowsWithoutUsername()
    {
        var builder = new TasClientBuilder()
            .WithFoundationUri("https://api.tas");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }
}
