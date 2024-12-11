using Director.services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts.events;

public class WishlistConsumer : IConsumer<WishlistCreatedEvent>
{
    private readonly ILogger<WishlistConsumer> _logger;
    private readonly IDirectorService _directorService;
	private readonly object syncLock = new();

    public WishlistConsumer(ILogger<WishlistConsumer> logger, IDirectorService directorService)
    {
        _logger = logger;
        _directorService = directorService;
    }

    public Task Consume(ConsumeContext<WishlistCreatedEvent> context)
    {
        _logger.LogInformation("Consuming wishlist message: {MessageId} {Example} {HackathonId}", 
            context.Message.isForTeamLead, context.Message.Wishlist.EmployeeId, context.Message.hackathonId);

        try
        {
            WishlistCreatedEvent wishlistCreatedEvent = context.Message as WishlistCreatedEvent;

            lock (syncLock)
            {
                if (wishlistCreatedEvent.isForTeamLead)
                {
                    _directorService.AddTeamLead(wishlistCreatedEvent.hackathonId, wishlistCreatedEvent.Wishlist);
                } else
                { 
                    _directorService.AddJunior(wishlistCreatedEvent.hackathonId, wishlistCreatedEvent.Wishlist);
                }
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing wishlist message: {MessageId}", context.Message);
        }
        
        return Task.CompletedTask;
    }
}