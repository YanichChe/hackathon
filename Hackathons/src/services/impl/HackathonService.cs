using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services.impl;

public class HackathonService(
    IHRManagerService hrManagerService,
    IHRDirectorService hrDirectorService,
    IWishlistsGeneratorService wishlistsGeneratorService,
    IConfiguration configuration,
    ILoaderService loaderService,
    IHostApplicationLifetime appLifetime
) : IHostedService
{
    private const string JuniorsFileKey = "FilePaths:JuniorsFilePath";
    private const string TeamLeadsFileKey = "FilePaths:TeamLeadsFilePath";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            double averageHarmonicity = Start();
            Console.WriteLine($"\nAverage Harmonicity after hackathons: {averageHarmonicity:F2}");

            appLifetime.StopApplication();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public double Start()
    {
        var hackathonsCountString = configuration["HackathonsCount"];

        if (string.IsNullOrEmpty(hackathonsCountString))
        {
            throw new ArgumentException("HackathonsCount not found in configuration");
        }

        if (!int.TryParse(hackathonsCountString, out var hackathonsCount))
        {
            throw new ArgumentException("HackathonsCount is not a valid number");
        }

        var juniors = loaderService.LoadEmployees(JuniorsFileKey);
        var teamLeads = loaderService.LoadEmployees(TeamLeadsFileKey);

        double totalHarmonicity = 0;
        for (int i = 0; i < hackathonsCount; i++)
        {
            List<Wishlist> teamLeadsWishlists = wishlistsGeneratorService.GenerateWishlist(teamLeads, juniors);
            List<Wishlist> juniorsWishlists = wishlistsGeneratorService.GenerateWishlist(juniors, teamLeads);
            
            var pairs = hrManagerService.MatchParticipants(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
            List<int> satisfactionPoints = new List<int>();
            foreach (Team team in pairs)
            {
                satisfactionPoints.Add(GetSatisfactionPoint(team.TeamLead, team.Junior, teamLeadsWishlists));
                satisfactionPoints.Add(GetSatisfactionPoint(team.Junior, team.TeamLead, juniorsWishlists));
            }
            
            double harmonicity = hrDirectorService.CalculateHarmonicity(satisfactionPoints);
            Console.WriteLine($"\n Average Harmonicity after Hackathons {i + 1}: {harmonicity:F2}");
            totalHarmonicity += harmonicity;
        }
        return totalHarmonicity / hackathonsCount;
    }

    private int GetSatisfactionPoint(Employee employee, Employee desiredEmployee, List<Wishlist> wishlists)
    {
        var employeeWishlist = wishlists.FirstOrDefault(wishlist => wishlist.EmployeeId == employee.Id);

        if (employeeWishlist == null) throw new NullReferenceException();

        var index = employeeWishlist.DesiredEmployees.ToList().IndexOf(desiredEmployee.Id);
        return wishlists.Count - index;
    }
}
