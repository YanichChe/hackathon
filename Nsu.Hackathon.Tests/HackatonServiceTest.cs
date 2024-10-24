using hackathon.context;
using hackathon.context.model;
using hackathon.context.repository;
using hackathon.context.repository.impl;
using Hackathon.services;
using Hackathon.services.impl;
using hackathon.strategy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Nsu.HackathonProblem.Contracts;
using Xunit;
using Assert = Xunit.Assert;
using Wishlist = Nsu.HackathonProblem.Contracts.Wishlist;

public class HackathonServiceTests
{
    private readonly HackathonContext _context;
    private readonly Mock<IHRManagerService> _hrManagerServiceMock;
    private readonly Mock<IHRDirectorService> _hrDirectorServiceMock;
    private readonly Mock<IWishlistsGeneratorService> _wishlistsGeneratorServiceMock;
    private readonly Mock<ILoaderService> _loaderServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly HackathonService _service;

    public HackathonServiceTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<HackathonContext>()
            .UseSqlite(connection)
            .Options;

        _context = new HackathonContext(options);
        _context.Database.EnsureCreated();

        _hrManagerServiceMock = new Mock<IHRManagerService>();
        _hrDirectorServiceMock = new Mock<IHRDirectorService>();
        _wishlistsGeneratorServiceMock = new Mock<IWishlistsGeneratorService>();
        _loaderServiceMock = new Mock<ILoaderService>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["HackathonsCount"]).Returns("3");
        _configurationMock.Setup(c => c["FilePaths:JuniorsFilePath"]).Returns("path/to/juniors");
        _configurationMock.Setup(c => c["TeamLeadsFilePath"]).Returns("path/to/teamleads");

        _service = new HackathonService(
            _hrManagerServiceMock.Object,
            _hrDirectorServiceMock.Object,
            _wishlistsGeneratorServiceMock.Object,
            _configurationMock.Object,
            _loaderServiceMock.Object,
            Mock.Of<IHostApplicationLifetime>(),
            new JuniorRepository(_context),
            new TeamLeadRepository(_context),
            new WishListRepository(_context),
            new HackathonRepository(_context),
            new TeamRepository(_context),
            _context
        );
    }

    [Fact]
    public async Task Test_SaveHackathonEventToDb()
    {
        // Arrange
        var hackathon = new hackathon.context.model.Hackathon { Id = 1, Harmoniousness = 0.8 };

        // Act
        await _context.Hackathons.AddAsync(hackathon);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Hackathons.FindAsync(1);
        Assert.NotNull(result);
        Assert.Equal(0.8, result.Harmoniousness);
    }

    [Fact]
    public async Task Test_ReadHackathonEventFromDb()
    {
        // Arrange
        var hackathon = new hackathon.context.model.Hackathon { Id = 1, Harmoniousness = 0.9 };
        await _context.Hackathons.AddAsync(hackathon);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Hackathons.FindAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.9, result.Harmoniousness);
    }

    [Fact]
    public void Test_CalculateHarmonicity()
    {
        // Mocking satisfaction points
        List<int> satisfactionPoints = new List<int> { 4, 3, 5, 2 };
        _hrDirectorServiceMock.Setup(s => s.CalculateHarmonicity(satisfactionPoints))
            .Returns(0.75);

        // Act
        double harmonicity = _hrDirectorServiceMock.Object.CalculateHarmonicity(satisfactionPoints);

        // Assert
        Assert.Equal(0.75, harmonicity, precision: 2);
    }

    [Fact]
    public void Test_CalculateHarmonicityAndSave()
    {
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
        _loaderServiceMock.Setup(x => x.LoadEmployees("FilePaths:JuniorsFilePath"))
            .Returns(juniors);
        _loaderServiceMock.Setup(x => x.LoadEmployees("FilePaths:TeamLeadsFilePath"))
            .Returns(teamLeads);

        _wishlistsGeneratorServiceMock.Setup(x => x.GenerateWishlist(teamLeads, juniors))
            .Returns(new List<Wishlist>
            {
                new Wishlist(1, [2, 1, 3]),
                new Wishlist(2, [1, 2, 3]),
                new Wishlist(3, [1, 3, 2])
            });
        _wishlistsGeneratorServiceMock.Setup(x => x.GenerateWishlist(juniors, teamLeads))
            .Returns(new List<Wishlist>
            {
                new Wishlist(1, [1, 2, 3]),
                new Wishlist(2, [2, 1, 3]),
                new Wishlist(3, [1, 3, 2])
            });

        var strategy = new GaleShapleyStrategy();
        var hrManagerService = new HRManagerService(strategy);
        var hrDirectorService = new HRDirectorService();
        
        var hackathonService = new HackathonService(
            hrManagerService,
            hrDirectorService,
            _wishlistsGeneratorServiceMock.Object,
            configuration,
            _loaderServiceMock.Object,
            Mock.Of<IHostApplicationLifetime>(),
            new JuniorRepository(_context),
            new TeamLeadRepository(_context),
            new WishListRepository(_context),
            new HackathonRepository(_context),
            new TeamRepository(_context),
            _context
        );

        // Act
        double result = hackathonService.Start();

        // Assert
        Assert.Equal(2.25, result);
    }
}