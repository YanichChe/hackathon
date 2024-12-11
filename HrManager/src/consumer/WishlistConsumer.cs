using HrManager.services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts.events;

public class WishlistConsumer : IConsumer<WishlistCreatedEvent>
{
    private readonly ILogger<WishlistConsumer> _logger;
    private readonly IHackathonService _hackathonService;
	private readonly object syncLock = new();
    private bool methodCalled;

    public WishlistConsumer(ILogger<WishlistConsumer> logger, IHackathonService hackathonService)
    {
        _logger = logger;
        _hackathonService = hackathonService;
    }

    public Task Consume(ConsumeContext<WishlistCreatedEvent> context)
    {
        _logger.LogInformation("Consuming wishlist message: {MessageId} {Example}", context.Message.isForTeamLead, context.Message.Wishlist.EmployeeId);

        try
        {
            WishlistCreatedEvent wishlistCreatedEvent = context.Message as WishlistCreatedEvent;

            lock (syncLock)
            {
                if (wishlistCreatedEvent.isForTeamLead)
                {
                    _hackathonService.AddTeamLead(wishlistCreatedEvent.hackathonId, wishlistCreatedEvent.Wishlist);
                } else
                { 
                    _hackathonService.AddJunior(wishlistCreatedEvent.hackathonId, wishlistCreatedEvent.Wishlist);
                }
                
                if (_hackathonService.IsAllPreferencesPresented(wishlistCreatedEvent.hackathonId))
                {
                    var teams = _hackathonService.StartHackathon(wishlistCreatedEvent.hackathonId);
                    _hackathonService.SendTeams(wishlistCreatedEvent.hackathonId, teams);
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