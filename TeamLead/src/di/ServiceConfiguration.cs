using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamLead.services;
using TeamLead.Services;

namespace TeamLead.di;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IWishlistsGeneratorService, WishlistsGeneratorService>();
        services.AddHostedService<TeamLeadService>();
    }

    public static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
    }
}