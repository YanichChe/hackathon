using hackathon.context;
using hackathon.context.repository;
using hackathon.context.repository.impl;
using Hackathon.services;
using Hackathon.services.impl;
using hackathon.strategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.HackathonProblem.Contracts;

namespace Hackathon.di;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddDbContext<HackathonContext>();
        services.AddScoped<IHRManagerService, HRManagerService>();
        services.AddScoped<IHRDirectorService, HRDirectorService>();
        services.AddScoped<ILoaderService, LoaderService>();
        services.AddScoped<IWishlistsGeneratorService, WishlistsGeneratorService>();
        services.AddScoped<IHostedService, HackathonService>();
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IJuniorRepository, JuniorRepository>();
        services.AddScoped<ITeamLeadRepository, TeamLeadRepository>();
        services.AddScoped<IHackathonRepository, HackathonRepository>();
        services.AddScoped<IWishListRepository, WishListRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
    }

    public static void ConfigureStrategy(this IServiceCollection services)
    {
        services.AddScoped<ITeamBuildingStrategy, GreedyStrategy>();
    }
}
