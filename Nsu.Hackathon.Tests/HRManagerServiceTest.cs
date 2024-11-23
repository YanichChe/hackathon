using Hackathon.services.impl;
using hackathon.strategy;
using Nsu.HackathonProblem.Contracts;
using Xunit;
using Assert = Xunit.Assert;
using Moq;

namespace Nsu.Hackathon.Tests;

public class HRManagerServiceTest
{
    [Fact]
    public void MatchParticipants_MethodCalledOnce()
    {
        // Arrange
        var strategy = new Mock<ITeamBuildingStrategy>();
        var hrManagerService = new HRManagerService(strategy.Object);
        var teamLeads = new List<Employee>
        {
            new Employee(1, "TeamLead1"),
            new Employee(2, "TeamLead2"),
            new Employee(3, "TeamLead3")
        };

        var juniors = new List<Employee>
        {
            new Employee(1, "Junior1"),
            new Employee(2, "Junior2"),
            new Employee(3, "Junior3")
        };

        var teamLeadsWishlists = new List<Wishlist>
        {
            new Wishlist(1, [2, 1, 3]),
            new Wishlist(2, [1, 2, 3]),
            new Wishlist(3, [1, 3, 2])
        };

        var juniorsWishlists = new List<Wishlist>
        {
            new Wishlist(1, [1, 2, 3]),
            new Wishlist(2, [2, 1, 3]),
            new Wishlist(3, [1, 3, 2])
        };

        // Act
        hrManagerService.MatchParticipants(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
        
        // Assert
        strategy.Verify(x => x.BuildTeams(
            It.IsAny<IEnumerable<Employee>>(),
            It.IsAny<IEnumerable<Employee>>(),
            It.IsAny<IEnumerable<Wishlist>>(),
            It.IsAny<IEnumerable<Wishlist>>()), Times.Once); // Стратегия HRManager-а должна быть вызвана ровно один раз
    }

    [Fact]
    public void MatchParticipants_TeamLeadJuniorCount_Equal()
    {
        // Arrange
        var strategy = new GaleShapleyStrategy();
        var hrManagerService = new HRManagerService(strategy);
        var teamLeads = new List<Employee>
        {
            new Employee(1, "TeamLead1"),
            new Employee(2, "TeamLead2"),
            new Employee(3, "TeamLead3")
        };

        var juniors = new List<Employee>
        {
            new Employee(1, "Junior1"),
            new Employee(2, "Junior2"),
            new Employee(3, "Junior3")
        };

        var teamLeadsWishlists = new List<Wishlist>
        {
            new Wishlist(1, [2, 1, 3]),
            new Wishlist(2, [1, 2, 3]),
            new Wishlist(3, [1, 3, 2])
        };

        var juniorsWishlists = new List<Wishlist>
        {
            new Wishlist(1, [1, 2, 3]),
            new Wishlist(2, [2, 1, 3]),
            new Wishlist(3, [1, 3, 2])
        };

        // Act 
        var result = hrManagerService.MatchParticipants(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
        
        //Assert
        Assert.Equal(3, result.Count); //Количество команд должно совпадать с заранее заданным
    }

    [Fact]
    public void MatchParticipants_PredeterminedPreferences_ReturnsExpectedDistribution()
    {
        // Arrange
        var strategy = new GaleShapleyStrategy();
        var hrManagerService = new HRManagerService(strategy);
        var teamLeads = new List<Employee>
        {
            new Employee(1, "TeamLead1"),
            new Employee(2, "TeamLead2"),
            new Employee(3, "TeamLead3")
        };

        var juniors = new List<Employee>
        {
            new Employee(1, "Junior1"),
            new Employee(2, "Junior2"),
            new Employee(3, "Junior3")
        };

        var teamLeadsWishlists = new List<Wishlist>
        {
            new Wishlist(1, [2, 1, 3]),
            new Wishlist(2, [1, 2, 3]),
            new Wishlist(3, [1, 3, 2])
        };

        var juniorsWishlists = new List<Wishlist>
        {
            new Wishlist(1, [1, 2, 3]),
            new Wishlist(2, [2, 1, 3]),
            new Wishlist(3, [1, 3, 2])
        };

        var expectedTeams = new List<Team>
        {
            new Team(teamLeads[0], juniors[0]),
            new Team(teamLeads[1], juniors[1]),
            new Team(teamLeads[2], juniors[2]) 
        };

        // Act
        var result = hrManagerService.MatchParticipants(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

        // Assert
        Assert.Equal(expectedTeams, result); //Стратегия HRManager-a – на заранее определённых предпочтениях, должна возвращать заранее определённое распределение
    }
}