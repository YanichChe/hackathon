using HrManager.context.model;

namespace HrManager.context.repository;

public interface IWishListRepository
{
    public void Save(Wishlist wishlist);
}

public class WishListRepository(HackathonContext hackathonContext) : IWishListRepository
{
    public void Save(Wishlist wishlist)
    {
        hackathonContext.Wishlists.Add(wishlist);
        hackathonContext.SaveChanges();
    }
}