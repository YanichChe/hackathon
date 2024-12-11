using MassTransit;
using Nsu.HackathonProblem.Contracts;
using Nsu.HackathonProblem.Contracts.events;

namespace TeamLead.publisher;

public interface ITeamLeadPublisher
{
    public void SendWishList(Wishlist wishlist, int hackathonId);
}

public class TeamLeadPublisher(IBus bus) : ITeamLeadPublisher
{
    public void SendWishList(Wishlist wishlist, int hackathonId)
    {
        bus.Publish(new WishlistCreatedEvent
        {
            isForTeamLead = true,
            hackathonId = hackathonId,
            Wishlist = wishlist,
            CreatedAt = DateTime.UtcNow
        });
    }
}
