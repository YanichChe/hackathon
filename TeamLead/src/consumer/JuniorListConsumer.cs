using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts.events;
using TeamLead.Services;

namespace TeamLead.consumer;

public class JuniorListConsumer : IConsumer<JuniorListGeneratedEvent>
{
    private readonly ILogger<JuniorListConsumer> _logger;
    private readonly ITeamLeadService _teamLeadService;
    
    public JuniorListConsumer(ILogger<JuniorListConsumer> logger, ITeamLeadService teamLeadService)
    {
        _logger = logger;
        _teamLeadService = teamLeadService;
    }

    public Task Consume(ConsumeContext<JuniorListGeneratedEvent> context)
    {
        _logger.LogInformation("Consuming juniors list: {MessageId}", context.Message);

        try
        {
            _teamLeadService.SendWishList(context.Message.juniors, context.Message.hackathonId);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing juniors list message: {MessageId}", context.Message);
        }
        
        return Task.CompletedTask;
    }
}