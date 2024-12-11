using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamLead.consumer;
using TeamLead.di;

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
        
        string prefix = "teamlead";
        string queueName = $"{prefix}_{Environment.GetEnvironmentVariable("TEAMLEAD_ID")}";
        Console.WriteLine($"Queue Name: {queueName}");
        factoryConfig.ReceiveEndpoint(queueName, config =>
        {
            config.ConfigureConsumer<JuniorListConsumer>(context);
        });
    });
});

builder.Services.ConfigureLogging();
builder.Services.ConfigureServices();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();