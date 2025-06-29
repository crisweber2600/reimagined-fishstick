using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class ServiceCollectionExtensions
{
    /// TASK: Add TasClient and dependencies to the service collection
    public static IServiceCollection AddTasClient(this IServiceCollection services, Action<TasClientBuilder> configure)
    {
        var builder = new TasClientBuilder();
        configure(builder);
        var client = builder.Build();

        services.AddSingleton(builder.Options);
        services.AddSingleton(builder.AuthenticationService!);
        services.AddSingleton(builder.FoundationApi!);
        services.AddSingleton(builder.OrgSpaceApi!);
        services.AddSingleton(builder.AppApi!);
        services.AddSingleton(builder.ProcessApi!);
        services.AddSingleton<ITasClient>(client);
        return services;
    }
}
