using Hackathon.services;
using Hackathon.services.impl;
using hackathon.strategy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.HackathonProblem.Contracts;

namespace Hackathon.di;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<ITeamBuildingStrategy, GreedyStrategy>();
        services.AddSingleton<IHRManagerService, HRManagerService>();
        services.AddSingleton<IHRDirectorService, HRDirectorService>();
        services.AddSingleton<ILoaderService, LoaderService>();
        services.AddSingleton<IWishlistsGeneratorService, WishlistsGeneratorService>();
        services.AddHostedService<HackathonService>();
    }
}
