using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services.impl;

public class WishlistsGeneratorService : IWishlistsGeneratorService
{
    private readonly Random _random = new Random();
    
    public  List<Wishlist> GenerateWishlist(List<Employee> employees, List<Employee> desiredEmployees)
    {
        List<Wishlist> wishlists = new List<Wishlist>();
        foreach (Employee employee in employees)
        {
            List<Employee> randomizedEmployees = desiredEmployees.OrderBy(j => _random.Next()).ToList();
            wishlists.Add(new Wishlist(employee.Id, randomizedEmployees.Select(employee => employee.Id).ToArray()));
        }

        return wishlists;
    }
}
