using Nsu.HackathonProblem.Contracts;

namespace hackathon.strategy;

public class GaleShapleyStrategy : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsDict = teamLeads.ToDictionary(tl => tl.Id);
        var juniorsDict = juniors.ToDictionary(j => j.Id);

        var freeJuniors = new Queue<Employee>(juniors);
        var matches = new Dictionary<int, int>(teamLeadsDict.Count);

        var teamLeadsWishlistsDict = teamLeadsWishlists
            .ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);
        var juniorsWishlistsDict = juniorsWishlists
            .ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);

        while (freeJuniors.Any())
        {
            var junior = freeJuniors.Dequeue();
            var juniorWishlist = juniorsWishlistsDict[junior.Id];

            foreach (var preferredTeamLeadId in juniorWishlist)
            {
                if (!teamLeadsDict.TryGetValue(preferredTeamLeadId, out var teamLead)) 
                    throw new NullReferenceException();
                
                var teamLeadWishlist = teamLeadsWishlistsDict[teamLead.Id];

                if (matches.TryGetValue(teamLead.Id, out var currentJuniorId))
                {
                    var currentJuniorRank = Array.IndexOf(teamLeadWishlist, currentJuniorId);
                    var newJuniorRank = Array.IndexOf(teamLeadWishlist, junior.Id);

                    if (newJuniorRank >= 0 && newJuniorRank < currentJuniorRank)
                    {
                        matches[teamLead.Id] = junior.Id;
                        freeJuniors.Enqueue(juniorsDict[currentJuniorId]);
                        break;
                    }
                }
                else
                {
                    matches[teamLead.Id] = junior.Id;
                    break;
                }
            }
        }

        return matches
            .Select(match => new Team(teamLeadsDict[match.Key], juniorsDict[match.Value]))
            .ToList();
    }
}
