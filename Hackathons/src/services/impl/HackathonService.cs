using hackathon.context;
using hackathon.context.model;
using hackathon.context.repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nsu.HackathonProblem.Contracts;
using Team = Nsu.HackathonProblem.Contracts.Team;
using Wishlist = Nsu.HackathonProblem.Contracts.Wishlist;

namespace Hackathon.services.impl;

public class HackathonService(
    IHRManagerService hrManagerService,
    IHRDirectorService hrDirectorService,
    IWishlistsGeneratorService wishlistsGeneratorService,
    IConfiguration configuration,
    ILoaderService loaderService,
    IHostApplicationLifetime appLifetime,
    IJuniorRepository juniorRepository,
    ITeamLeadRepository teamLeadRepository,
    IWishListRepository wishListRepository,
    IHackathonRepository hackathonRepository,
    ITeamRepository teamRepository,
    HackathonContext hackathonContext
) : IHostedService
{
    private const string JuniorsFileKey = "FilePaths:JuniorsFilePath";
    private const string TeamLeadsFileKey = "FilePaths:TeamLeadsFilePath";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            juniorRepository.Clean();
            teamLeadRepository.Clean();
            hackathonRepository.Clean();
            
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
        
        using (hackathonContext)
        {
            juniorRepository.Save(juniors.Select(j => new Junior { Id = j.Id, Name = j.Name }).ToList());
            teamLeadRepository.Save(teamLeads.Select(t => new TeamLead { Id = t.Id, Name = t.Name }).ToList());

            for (int i = 0; i < hackathonsCount; i++)
            {
                var hackathon = new hackathon.context.model.Hackathon { Id = i };
                hackathonRepository.Save(hackathon);

                List<Wishlist> teamLeadsWishlists = wishlistsGeneratorService.GenerateWishlist(teamLeads, juniors);
                List<Wishlist> juniorsWishlists = wishlistsGeneratorService.GenerateWishlist(juniors, teamLeads);

                foreach (var teamLeadsWishlist in teamLeadsWishlists)
                {
                    for (int j = 0; j < teamLeadsWishlist.DesiredEmployees.Length; j++)
                    {
                        wishListRepository.Save(new hackathon.context.model.Wishlist
                            {
                                Rank = j + 1,
                                Junior = juniorRepository.FindById(teamLeadsWishlist.DesiredEmployees[j])!,
                                TeamLead = teamLeadRepository.FindById(teamLeadsWishlist.EmployeeId)!,
                                IsForTeamLead = true,
                                Hackathon = hackathon
                            }
                        );
                    }
                }

                foreach (var juniorWishList in juniorsWishlists)
                {
                    for (int j = 0; j < juniorWishList.DesiredEmployees.Length; j++)
                    {
                        wishListRepository.Save(new hackathon.context.model.Wishlist
                            {
                                Rank = j + 1,
                                Junior = juniorRepository.FindById(juniorWishList.DesiredEmployees[j])!,
                                TeamLead = teamLeadRepository.FindById(juniorWishList.EmployeeId)!,
                                IsForTeamLead = false,
                                Hackathon = hackathon
                            }
                        );
                    }
                }

                var pairs = hrManagerService.MatchParticipants(teamLeads, juniors, teamLeadsWishlists,
                    juniorsWishlists);
                List<int> satisfactionPoints = new();
                foreach (Team team in pairs)
                {
                    teamRepository.Save(new hackathon.context.model.Team
                        {
                            Hackathon = hackathon,
                            Junior = juniorRepository.FindById(team.Junior.Id)!,
                            TeamLead = teamLeadRepository.FindById(team.TeamLead.Id)!,
                        }
                    );
                    satisfactionPoints.Add(GetSatisfactionPoint(team.TeamLead, team.Junior, teamLeadsWishlists));
                    satisfactionPoints.Add(GetSatisfactionPoint(team.Junior, team.TeamLead, juniorsWishlists));
                }

                double harmonicity = hrDirectorService.CalculateHarmonicity(satisfactionPoints);
                Console.WriteLine($"\n Average Harmonicity after Hackathon {i + 1}: {harmonicity:F2}");
                hackathon.Harmoniousness = harmonicity;
                hackathonRepository.Update(hackathon);
            }
            
            return hackathonRepository.FindAll().Sum(h => h.Harmoniousness) / hackathonsCount;
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
