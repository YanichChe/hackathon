using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Junior.publisher;
using Junior.services;
using Junior.Services;

namespace Junior.di;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IJuniorPublisher, JuniorPublisher>();
        services.AddSingleton<IWishlistsGeneratorService, WishlistsGeneratorService>();
        services.AddSingleton<IJuniorService, JuniorService>();
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