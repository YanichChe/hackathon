using HrManager;
using HrManager.context;
using HrManager.context.model;
using HrManager.context.repository;
using HrManager.di;
using HrManager.services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

const string juniorsFileKey = "FilePaths:JuniorsFilePath";
const string teamLeadsFileKey = "FilePaths:TeamLeadsFilePath";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Services.ConfigureLogging();
builder.Services.ConfigureStrategy();
builder.Services.ConfigureServices();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureHttpClient();
builder.Services.ConfigureHttpClient();
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
    var hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var hackathon = new Hackathon { Id = 1 };
        hackathonRepository.Save(hackathon);
        logger.LogInformation($"Create new Hackathon {hackathon.Id}");

        var juniors = loaderService.LoadEmployees(juniorsFileKey);
        juniorRepository.Save(juniors.Select(j => new Junior { Id = j.Id, Name = j.Name }).ToList());
        logger.LogInformation($"Loaded {juniors.Count} juniors");

        var teamLeads = loaderService.LoadEmployees(teamLeadsFileKey);
        teamLeadRepository.Save(teamLeads.Select(t => new TeamLead { Id = t.Id, Name = t.Name }).ToList());
        logger.LogInformation($"Loaded {teamLeads.Count} team leads");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while loading files");
    }
}

app.UseMiddleware<MiddlewareService>();
app.MapHealthChecks("/health");
app.UseAuthorization();
app.MapControllers();
app.Run();