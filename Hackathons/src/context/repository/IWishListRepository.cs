using hackathon.context.model;

namespace hackathon.context.repository;

public interface IWishListRepository
{
    public void Save(Wishlist wishlist);
}
