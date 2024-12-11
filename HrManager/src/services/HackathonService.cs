using System.Collections.Concurrent;
using HrManager.context;
using HrManager.context.repository;
using HrManager.publisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;

namespace HrManager.services;

public interface IHackathonService
{
    public List<Team> StartHackathon(int id);
    public void SendTeams(int id, List<Team> teams);
    public bool IsAllPreferencesPresented(int hackathonId);
    public void AddJunior(int hackathonId, Wishlist wishlist);
    public void AddTeamLead(int hackathonId, Wishlist wishlist);
}

public class HackathonService(
    ITeamBuildingStrategy teamBuildingStrategy,
    IServiceProvider serviceProvider
) : IHackathonService
{
    private readonly ConcurrentDictionary<int, List<Wishlist>> juniorWishlists = new();
    private readonly ConcurrentDictionary<int, List<Wishlist>> teamLeadWishLists = new();
    private readonly ConcurrentDictionary<int, bool> hackathonStarted = new();
    
    public void AddJunior(int hackathonId, Wishlist wishlist)
    {
        juniorWishlists.AddOrUpdate(
            hackathonId,
            _ => [wishlist],
            (_, list) =>
            {
                lock (list)
                {
                    list.Add(wishlist);
                }

                return list;
            }
        );    
    }
    
    public void AddTeamLead(int hackathonId, Wishlist wishlist)
    {
        teamLeadWishLists.AddOrUpdate(
            hackathonId,
            _ => [wishlist],
            (_, list) =>
            {
                lock (list)
                {
                    list.Add(wishlist);
                }

                return list;
            }
        );    
    }
    
    public bool IsAllPreferencesPresented(int hackathonId)
    {
        return juniorWishlists.TryGetValue(hackathonId, out var juniorList) &&
               juniorList.Count == 5 && teamLeadWishLists.TryGetValue(hackathonId, out var teamLeadList) &&
               teamLeadList.Count == 5;
    }
    
    public List<Team> StartHackathon(int id)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var hackathonContext = scope.ServiceProvider.GetRequiredService<HackathonContext>();
            var hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
            var juniorRepository = scope.ServiceProvider.GetRequiredService<IJuniorRepository>();
            var teamLeadRepository = scope.ServiceProvider.GetRequiredService<ITeamLeadRepository>();
            var wishListRepository = scope.ServiceProvider.GetRequiredService<IWishListRepository>();
            var teamRepository = scope.ServiceProvider.GetRequiredService<ITeamRepository>();
            var _logger = scope.ServiceProvider.GetRequiredService<ILogger<HackathonService>>();

            _logger.LogInformation($"Starting hackathon {id}");
            var hackathon = hackathonRepository.GetHackathon(id);
            
            List<Wishlist> teamLeadsWishlists = teamLeadWishLists[hackathon.Id];
            List<Wishlist> juniorsWishlists = juniorWishlists[hackathon.Id];
            
            foreach (var teamLeadsWishlist in teamLeadsWishlists)
                for (var j = 0; j < teamLeadsWishlist.DesiredEmployees.Length; j++)
                    wishListRepository.Save(new context.model.Wishlist
                        {
                            Rank = j + 1,
                            Junior = juniorRepository.FindById(teamLeadsWishlist.DesiredEmployees[j])!,
                            TeamLead = teamLeadRepository.FindById(teamLeadsWishlist.EmployeeId)!,
                            IsForTeamLead = true,
                            Hackathon = hackathon
                        }
                    );

            foreach (var juniorWishList in juniorsWishlists)
                for (var j = 0; j < juniorWishList.DesiredEmployees.Length; j++)
                    wishListRepository.Save(new context.model.Wishlist
                        {
                            Rank = j + 1,
                            Junior = juniorRepository.FindById(juniorWishList.DesiredEmployees[j])!,
                            TeamLead = teamLeadRepository.FindById(juniorWishList.EmployeeId)!,
                            IsForTeamLead = false,
                            Hackathon = hackathon
                        }
                    );

            var juniors = juniorRepository.GetAll().Select(j => new Employee(j.Id, j.Name)).ToList();
            var teamLeads = teamLeadRepository.GetAll().Select(teamLead => new Employee(teamLead.Id, teamLead.Name))
                .ToList();
            var pairs = teamBuildingStrategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists)
                .ToList();

            foreach (var team in pairs)
                teamRepository.Save(new context.model.Team
                    {
                        Hackathon = hackathon,
                        Junior = juniorRepository.FindById(team.Junior.Id)!,
                        TeamLead = teamLeadRepository.FindById(team.TeamLead.Id)!
                    }
                );

            return pairs;
        }
    }

    public void SendTeams(int id, List<Team> teams)
    {
        if (hackathonStarted.ContainsKey(id))
        {
            return;
        }

        hackathonStarted.TryAdd(id, true);
        using (var scope = serviceProvider.CreateScope())
        {
            IHrManagerPublisher hrManagerPublisher = scope.ServiceProvider.GetRequiredService<IHrManagerPublisher>();
            hrManagerPublisher.SendTeams(teams, id);   
        }
    }
}