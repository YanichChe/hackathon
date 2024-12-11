using Director.services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts.events;

namespace Director.consumer;

public class TeamsConsumer: IConsumer<TeamsGeneratedEvent>
{
    private readonly ILogger<TeamsConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TeamsConsumer(ILogger<TeamsConsumer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task Consume(ConsumeContext<TeamsGeneratedEvent> context)
    {
        _logger.LogInformation("Teams  message: {MessageId}", context.Message);

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var directorService = scope.ServiceProvider.GetRequiredService<IDirectorService>();
                directorService.CalculateHarmonicity(context.Message.hackathonId, context.Message.teams);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing teams message: {MessageId}", context.Message);
        }
        
        return Task.CompletedTask;
    }
}