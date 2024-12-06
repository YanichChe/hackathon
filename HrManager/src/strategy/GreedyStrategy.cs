using System.Collections.Concurrent;
using Nsu.HackathonProblem.Contracts;

namespace HrManager.strategy;

public class GreedyStrategy : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsDict = teamLeads.ToDictionary(tl => tl.Id);
        var juniorsDict = juniors.ToDictionary(j => j.Id);

        var teamLeadsWishlistsDict = teamLeadsWishlists
            .ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);
        var juniorsWishlistsDict = juniorsWishlists
            .ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);


        var scoreMap = new ConcurrentDictionary<Tuple<int, int>, int>();
        Parallel.ForEach(teamLeadsDict, teamLead =>
        {
            Parallel.ForEach(juniorsDict, junior =>
            {
                var teamLeadWishlist = teamLeadsWishlistsDict[teamLead.Key];
                var juniorWishList = juniorsWishlistsDict[junior.Key];

                var currentJuniorRank = Array.IndexOf(teamLeadWishlist, junior.Key);
                var currentTeamLeadRank = Array.IndexOf(juniorWishList, teamLead.Key);

                scoreMap.TryAdd(new Tuple<int, int>(teamLead.Key, junior.Key),
                    20 - currentJuniorRank + 20 - currentTeamLeadRank);
            });
        });

        var sortedScores = scoreMap.OrderByDescending(kvp => kvp.Value).ToArray();
        var results = new List<Tuple<int, int>>();

        foreach (var entry in sortedScores)
        {
            var teamLeadId = entry.Key.Item1;
            var juniorId = entry.Key.Item2;

            if (results.Any(r => r.Item1 == teamLeadId || r.Item2 == juniorId)) continue;

            results.Add(entry.Key);
        }

        return results.Select(x => new Team(teamLeadsDict[x.Item1], juniorsDict[x.Item2]));
    }
}