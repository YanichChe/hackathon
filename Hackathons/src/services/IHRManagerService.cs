using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services;

public interface IHRManagerService
{
    public List<Team> MatchParticipants(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists);
}
