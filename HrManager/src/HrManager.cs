using System.Reflection;
using HrManager.context;
using HrManager.context.model;
using HrManager.context.repository;
using HrManager.di;
using HrManager.publisher;
using HrManager.services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

const string juniorsFileKey = "FilePaths:JuniorsFilePath";
const string teamLeadsFileKey = "FilePaths:TeamLeadsFilePath";

var builder = WebApplication.CreateBuilder(args);

var rabbitMqUri = builder.Configuration.GetValue<string>("RabbitMQ:Uri");

builder.Services.AddMassTransit(registerConfig =>
{
    var entryAssembly = Assembly.GetExecutingAssembly();
    registerConfig.AddConsumers(entryAssembly);
    registerConfig.SetSnakeCaseEndpointNameFormatter();
    registerConfig.UsingRabbitMq((context, factoryConfig) =>
    {
        factoryConfig.Host(new Uri(rabbitMqUri), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        factoryConfig.ConfigureEndpoints(context);
    });
});

builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Services.ConfigureLogging();
builder.Services.ConfigureStrategy();
builder.Services.ConfigureServices();
builder.Services.ConfigureRepositories();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HackathonContext>();
    dbContext.Database.Migrate();

    var loaderService = scope.ServiceProvider.GetRequiredService<ILoaderService>();
    var juniorRepository = scope.ServiceProvider.GetRequiredService<IJuniorRepository>();
    var teamLeadRepository = scope.ServiceProvider.GetRequiredService<ITeamLeadRepository>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var juniors = loaderService.LoadEmployees(juniorsFileKey);
        juniorRepository.Save(juniors.Select(j => new Junior { Id = j.Id, Name = j.Name }).ToList());
        logger.LogInformation($"Loaded {juniors.Count} juniors");

        var teamLeads = loaderService.LoadEmployees(teamLeadsFileKey);
        teamLeadRepository.Save(teamLeads.Select(t => new TeamLead { Id = t.Id, Name = t.Name }).ToList());
        logger.LogInformation($"Loaded {teamLeads.Count} team leads");
        
        logger.LogInformation("END INIT");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while loading files");
    }
}

app.MapHealthChecks("/health");
app.Run();
