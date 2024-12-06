using Junior.services;
using Junior.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Junior.di;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IWishlistsGeneratorService, WishlistsGeneratorService>();
        services.AddHostedService<JuniorService>();
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