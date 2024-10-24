using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services.impl;

using System.Collections.Generic;
using System.Linq;

public class HRManagerService(ITeamBuildingStrategy teamBuildingStrategy) : IHRManagerService
{
    public List<Team> MatchParticipants(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        return teamBuildingStrategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();
    }
}
