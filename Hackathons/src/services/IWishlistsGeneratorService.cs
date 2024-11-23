using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services;

public interface IWishlistsGeneratorService
{
    List<Wishlist> GenerateWishlist(List<Employee> employees, List<Employee> desiredEmployees);
}
