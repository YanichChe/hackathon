using HrManager.context;
using HrManager.context.repository;
using Nsu.HackathonProblem.Contracts;

namespace HrManager.services;

public interface IHackathonService
{
    public List<Team> StartHackathon(int id, List<Wishlist> teamLeadsWishlists, List<Wishlist> juniorsWishlists);
}

public class HackathonService(
    HackathonContext hackathonContext,
    IHackathonRepository hackathonRepository,
    IJuniorRepository juniorRepository,
    ITeamLeadRepository teamLeadRepository,
    IWishListRepository wishListRepository,
    ITeamBuildingStrategy teamBuildingStrategy,
    ITeamRepository teamRepository
) : IHackathonService
{
    public List<Team> StartHackathon(int id, List<Wishlist> teamLeadsWishlists, List<Wishlist> juniorsWishlists)
    {
        using (hackathonContext)
        {
            var hackathon = hackathonRepository.GetHackathon(id)!;

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
}