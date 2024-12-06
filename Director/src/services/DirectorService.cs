using Director.context;
using Director.context.repository;
using Nsu.HackathonProblem.Contracts;

namespace Director.services;

public interface IDirectorService
{
    public double CalculateHarmonicity(List<Team> pairs, List<Wishlist> teamLeadWishlists,
        List<Wishlist> juniorWishlists);
}

public class DirectorService(
    HackathonContext hackathonContext,
    IHackathonRepository hackathonRepository
) : IDirectorService
{
    public double CalculateHarmonicity(List<Team> pairs, List<Wishlist> teamLeadWishlists,
        List<Wishlist> juniorWishlists)
    {
        using (hackathonContext)
        {
            List<int> satisfactionPoints = new();
            foreach (var team in pairs)
            {
                satisfactionPoints.Add(GetSatisfactionPoint(team.TeamLead, team.Junior, teamLeadWishlists));
                satisfactionPoints.Add(GetSatisfactionPoint(team.Junior, team.TeamLead, juniorWishlists));
            }

            var harmonicSum = satisfactionPoints.Sum(x => 1.0 / x);
            var count = satisfactionPoints.Count;
            var harmonicMean = count / harmonicSum;

            var hackathon = hackathonRepository.GetHackathon(1)!;
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