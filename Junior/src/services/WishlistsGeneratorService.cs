using Nsu.HackathonProblem.Contracts;

namespace Junior.services;

public interface IWishlistsGeneratorService
{
    Wishlist GenerateWishlist(int id, List<Employee> desiredEmployees);
}

public class WishlistsGeneratorService : IWishlistsGeneratorService
{
    private readonly Random _random = new();

    public Wishlist GenerateWishlist(int id, List<Employee> desiredEmployees)
    {
        List<Employee> randomizedEmployees = desiredEmployees.OrderBy(j => _random.Next()).ToList();
        return new Wishlist(id, randomizedEmployees.Select(employee => employee.Id).ToArray());
    }
}