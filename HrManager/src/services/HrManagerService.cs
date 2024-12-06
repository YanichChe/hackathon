using HrManager.context.repository;
using Nsu.HackathonProblem.Contracts;
using Wishlist = Nsu.HackathonProblem.Contracts.Wishlist;

namespace HrManager.services;

public interface IHrManagerService
{
    public void SaveTeamLeadWishList(Wishlist wishlist, int hackathonId);
    public void SaveJuniorWishList(Wishlist wishlist, int hackathonId);
    public List<Employee> GetJuniors();
    public List<Employee> GetTeamLeads();
}

public class HrManagerService(
    IJuniorRepository juniorRepository,
    ITeamLeadRepository teamLeadRepository,
    IWishListRepository wishListRepository,
    IHackathonRepository hackathonRepository
) : IHrManagerService
{
    public void SaveJuniorWishList(Wishlist wishlist, int hackathonId)
    {
        var hackathon = hackathonRepository.GetHackathon(hackathonId) ?? throw new ArgumentNullException();

        for (var j = 0; j < wishlist.DesiredEmployees.Length; j++)
            wishListRepository.Save(new context.model.Wishlist
                {
                    Rank = j + 1,
                    Junior = juniorRepository.FindById(wishlist.EmployeeId)!,
                    TeamLead = teamLeadRepository.FindById(wishlist.DesiredEmployees[j])!,
                    IsForTeamLead = false,
                    Hackathon = hackathon
                }
            );
    }

    public void SaveTeamLeadWishList(Wishlist wishlist, int hackathonId)
    {
        var hackathon = hackathonRepository.GetHackathon(hackathonId) ?? throw new ArgumentNullException();

        for (var j = 0; j < wishlist.DesiredEmployees.Length; j++)
            wishListRepository.Save(new context.model.Wishlist
                {
                    Rank = j + 1,
                    Junior = juniorRepository.FindById(wishlist.DesiredEmployees[j])!,
                    TeamLead = teamLeadRepository.FindById(wishlist.EmployeeId)!,
                    IsForTeamLead = true,
                    Hackathon = hackathon
                }
            );
    }

    public List<Employee> GetJuniors()
    {
        return juniorRepository.GetAll().Select(junior => new Employee(junior.Id, junior.Name)).ToList();
    }

    public List<Employee> GetTeamLeads()
    {
        return teamLeadRepository.GetAll().Select(teamLead => new Employee(teamLead.Id, teamLead.Name)).ToList();
    }
}