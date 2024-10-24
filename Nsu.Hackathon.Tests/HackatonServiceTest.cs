using Hackathon.services;
using Hackathon.services.impl;
using hackathon.strategy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Nsu.HackathonProblem.Contracts;
using Xunit;
using Assert = Xunit.Assert;

namespace Nsu.Hackathon.Tests;

public class HackatonServiceTest
{
    [Fact]
    public void Hackathon_WithPredefinedParticipants_ReturnsExpectedHarmonicity()
    {
        // Arrange
        var strategy = new GaleShapleyStrategy();
        var hrManagerService = new HRManagerService(strategy);
        var hrDirectorService = new HRDirectorService();
        var mockWishlistsGeneratorService = new Mock<IWishlistsGeneratorService>();
        var mockLoaderService = new Mock<ILoaderService>();
        var mockAppLifetime = new Mock<IHostApplicationLifetime>();

        var juniors = new List<Employee>
        {
            new Employee(1, "Junior1"),
            new Employee(2, "Junior2"),
            new Employee(3, "Junior3")
        };

        var teamLeads = new List<Employee>
        {
            new Employee(1, "TeamLead1"),
            new Employee(2, "TeamLead2"),
            new Employee(3, "TeamLead3")
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "HackathonsCount", "1" },
                { "FilePaths:JuniorsFilePath", "path/to/juniors" },
                { "FilePaths:TeamLeadsFilePath", "path/to/teamLeads" }
            })
            .Build();
        
        mockLoaderService.Setup(x => x.LoadEmployees("FilePaths:JuniorsFilePath"))
            .Returns(juniors);
        mockLoaderService.Setup(x => x.LoadEmployees("FilePaths:TeamLeadsFilePath"))
            .Returns(teamLeads);

        mockWishlistsGeneratorService.Setup(x => x.GenerateWishlist(teamLeads, juniors))
            .Returns(new List<Wishlist>
            {
                new Wishlist(1, [2, 1, 3]),
                new Wishlist(2, [1, 2, 3]),
                new Wishlist(3, [1, 3, 2])
            });

        mockWishlistsGeneratorService.Setup(x => x.GenerateWishlist(juniors, teamLeads))
            .Returns(new List<Wishlist>
            {
                new Wishlist(1, [1, 2, 3]),
                new Wishlist(2, [2, 1, 3]),
                new Wishlist(3, [1, 3, 2])
            });
        
        var hackathonService = new HackathonService(
            hrManagerService,
            hrDirectorService,
            mockWishlistsGeneratorService.Object,
            configuration,
            mockLoaderService.Object,
            mockAppLifetime.Object
        );

        // Act
        double result = hackathonService.Start();

        // Assert
        Assert.Equal(2.25, result);
    }
}