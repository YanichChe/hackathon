using MassTransit;
using Nsu.HackathonProblem.Contracts;
using Nsu.HackathonProblem.Contracts.events;

namespace Junior.publisher;

public interface IJuniorPublisher
{
    public void SendWishList(Wishlist wishlist, int hackathonId);
}

public class JuniorPublisher(IBus bus) : IJuniorPublisher
{
    public void SendWishList(Wishlist wishlist, int hackathonId)
    {
        bus.Publish(new WishlistCreatedEvent
        {
            hackathonId = hackathonId,
            Wishlist = wishlist,
            CreatedAt = DateTime.UtcNow
        });
    }
}
