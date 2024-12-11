using HrManager.context;
using HrManager.context.repository;
using HrManager.publisher;
using HrManager.services;
using HrManager.strategy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;

namespace HrManager.di;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ILoaderService, LoaderService>();
        services.AddScoped<IHrManagerService, HrManagerService>();
        services.AddSingleton<IHackathonService, HackathonService>();
        services.AddScoped<IHrManagerPublisher, HrManagerPublisher>();
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddDbContext<HackathonContext>();
        services.AddScoped<IHackathonRepository, HackathonRepository>();
        services.AddScoped<IJuniorRepository, JuniorRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamLeadRepository, TeamLeadRepository>();
        services.AddScoped<IWishListRepository, WishListRepository>();
    }

    public static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
    }
    
    public static void ConfigureStrategy(this IServiceCollection services)
    {
        services.AddSingleton<ITeamBuildingStrategy, GreedyStrategy>();
    }
}