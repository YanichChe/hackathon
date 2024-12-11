using Junior.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts.events;

namespace Junior.consumer;

public class TeamLeadListConsumer : IConsumer<TeamLeadListGeneratedEvent>
{
    private readonly ILogger<TeamLeadListConsumer> _logger;
    private readonly IJuniorService _juniorService;
    
    public TeamLeadListConsumer(ILogger<TeamLeadListConsumer> logger, IJuniorService juniorService)
    {
        _logger = logger;
        _juniorService = juniorService;
    }

    public Task Consume(ConsumeContext<TeamLeadListGeneratedEvent> context)
    {
        _logger.LogInformation("Consuming team lead list: {MessageId}", context.Message);

        try
        {
            _juniorService.SendWishList(context.Message.teamLeads, context.Message.hackathonId);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing team lead list message: {MessageId}", context.Message);
        }
        
        return Task.CompletedTask;
    }
}