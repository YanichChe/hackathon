namespace Nsu.HackathonProblem.Contracts.events
{
    public class WishlistCreatedEvent
    {
        public int hackathonId { get; set; }
        public Boolean isForTeamLead { get; set; }
        public Wishlist Wishlist { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
