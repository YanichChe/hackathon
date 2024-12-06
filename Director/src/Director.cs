using Director.context;
using Director.context.repository;
using Director.services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Services.AddScoped<IDirectorService, DirectorService>();
builder.Services.AddDbContext<HackathonContext>();
builder.Services.AddScoped<IHackathonRepository, HackathonRepository>();
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
var app = builder.Build();

app.MapHealthChecks("/health");
app.UseAuthorization();
app.MapControllers();
app.Run();