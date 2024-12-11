using System.Collections.Concurrent;
using Director.context;
using Director.context.repository;
using Microsoft.Extensions.DependencyInjection;
using Nsu.HackathonProblem.Contracts;

namespace Director.services;

public interface IDirectorService
{
    public void AddJunior(int hackathonId, Wishlist wishlist);
    public void AddTeamLead(int hackathonId, Wishlist wishlist);
    public double CalculateHarmonicity(int hackathonId,List<Team> pairs);
}

public class DirectorService(
    IServiceProvider serviceProvider
) : IDirectorService
{
    private readonly ConcurrentDictionary<int, List<Wishlist>> juniorWishlists = new();
    private readonly ConcurrentDictionary<int, List<Wishlist>> teamLeadWishLists = new();
    
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
    
    public double CalculateHarmonicity(int hackathonId, List<Team> pairs)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var hackathonContext = scope.ServiceProvider.GetRequiredService<HackathonContext>();
            var hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
            
            List<int> satisfactionPoints = new();
            var teamLeadsWishlists = teamLeadWishLists[hackathonId];
            var juniorsWishlists = juniorWishlists[hackathonId];
            
            foreach (var team in pairs)
            {
                satisfactionPoints.Add(GetSatisfactionPoint(team.TeamLead, team.Junior, teamLeadsWishlists));
                satisfactionPoints.Add(GetSatisfactionPoint(team.Junior, team.TeamLead, juniorsWishlists));
            }

            var harmonicSum = satisfactionPoints.Sum(x => 1.0 / x);
            var count = satisfactionPoints.Count;
            var harmonicMean = count / harmonicSum;

            var hackathon = hackathonRepository.GetHackathon(hackathonId)!;
            hackathon.Harmoniousness = harmonicMean;
            hackathonRepository.Update(hackathon);
            return harmonicMean;
        }
    }

    private int GetSatisfactionPoint(Employee employee, Employee desiredEmployee, List<Wishlist> wishlists)
    {
        var employeeWishlist = wishlists.FirstOrDefault(wishlist => wishlist.EmployeeId == employee.Id);

        if (employeeWishlist == null) throw new NullReferenceException();

        var index = employeeWishlist.DesiredEmployees.ToList().IndexOf(desiredEmployee.Id);
        return wishlists.Count - index;
    }
}