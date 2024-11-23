using hackathon.context.model;

namespace hackathon.context.repository.impl;

public class WishListRepository : IWishListRepository
{
    private readonly HackathonContext _hackathonContext;

    public WishListRepository(HackathonContext hackathonContext)
    {
        _hackathonContext = hackathonContext;
    }

    public void Save(Wishlist wishlist)
    {
        _hackathonContext.Wishlists.Add(wishlist);
        _hackathonContext.SaveChanges();
    }
}
