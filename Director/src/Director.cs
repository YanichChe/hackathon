using System.Reflection;
using Director.consumer;
using Director.context;
using Director.context.repository;
using Director.publisher;
using Director.services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
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
        factoryConfig.ReceiveEndpoint("director-wishlist", config =>
        {
            config.ConfigureConsumer<WishlistConsumer>(context);
            config.ConfigureConsumer<TeamsConsumer>(context);
        });
    });
});

builder.Services.AddScoped<IDirectorPublisher, DirectorPublisher>();
builder.Services.AddSingleton<IDirectorService, DirectorService>();
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

using (var scope = app.Services.CreateScope())
{
    var publisher = scope.ServiceProvider.GetRequiredService<IDirectorPublisher>();

    for (int i = 1; i <= 10; i++)
    {
        publisher.SendStartHackathon();
    }
}

app.MapHealthChecks("/health");
app.UseAuthorization();
app.MapControllers();
app.Run();