using Xunit;
using Hackathon.services.impl;
using Nsu.HackathonProblem.Contracts;
using Assert = Xunit.Assert;

namespace Nsu.Hackathon.Tests;
    
public class WishlistsGeneratorServiceTests
{
    private readonly WishlistsGeneratorService _service;

    public WishlistsGeneratorServiceTests()
    {
        _service = new WishlistsGeneratorService();
    }

    [Fact]
    public void GenerateWishlists_WishlistSizeMatchesTeamLeadsAndJuniorsCount()
    {
        // Arrange
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

        // Act
        List<Wishlist> teamLeadsWishlists = _service.GenerateWishlist(teamLeads, juniors);
        List<Wishlist> juniorsWishlists = _service.GenerateWishlist(juniors, teamLeads);

        // Assert
        Assert.All(teamLeads, teamLead =>
            Assert.Equal(juniors.Count,
                teamLeadsWishlists.Count)); // Размер списка должен совпадать с количеством джунов

        Assert.All(juniors, junior =>
            Assert.Equal(teamLeads.Count,
                juniorsWishlists.Count)); // Размер списка должен совпадать с количеством тимлидов
    }

    [Fact]
    public void GenerateWishlists_PredefinedJuniorPresentInTeamLeadWishlist()
    {
        // Arrange
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

        var predefinedJunior = juniors.First(j => j.Name == "Junior1");

        // Act
        List<Wishlist> teamLeadsWishlists = _service.GenerateWishlist(teamLeads, juniors);

        // Assert
        Assert.All(teamLeads, teamLead =>
            Assert.NotEqual(-1,
                GetIndex(teamLead, predefinedJunior,
                    teamLeadsWishlists))); //Заранее определённый сотрудник должен присутствовать в списке;
    }

    private int GetIndex(Employee employee, Employee desiredEmployee, List<Wishlist> wishlists)
    {
        var employeeWishlist = wishlists.FirstOrDefault(wishlist => wishlist.EmployeeId == employee.Id);

        if (employeeWishlist == null) throw new NullReferenceException();

        return employeeWishlist.DesiredEmployees.ToList().IndexOf(desiredEmployee.Id);
    }

    [Fact]
    public void GenerateWishlists_PredefinedTeamLeadPresentInJuniorWishlist()
    {
        
        // Arrange
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

        var predefinedTeamLead = teamLeads.First(j => j.Name == "TeamLead1");

        // Act
        List<Wishlist> juniorsWishlists = _service.GenerateWishlist(juniors, teamLeads);

        // Assert
        Assert.All(juniors, junior =>
            Assert.NotEqual(-1,
                GetIndex(junior, predefinedTeamLead,
                    juniorsWishlists))); //Заранее определённый сотрудник должен присутствовать в списке;
    }
}