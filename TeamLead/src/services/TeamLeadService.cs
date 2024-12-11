using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nsu.HackathonProblem.Contracts;
using TeamLead.publisher;
using TeamLead.services;

namespace TeamLead.Services;

public interface ITeamLeadService
{
    public Task SendWishList(List<Employee> juniors, int hackathonId);
}

public class TeamLeadService : ITeamLeadService
{
    private readonly ILogger<TeamLeadService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWishlistsGeneratorService _wishlistsGeneratorService;
    private readonly string _managerUrl;
    private readonly int _teamLeadId;
    
    public TeamLeadService(
        IWishlistsGeneratorService wishlistsGeneratorService,
        ILogger<TeamLeadService> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _wishlistsGeneratorService = wishlistsGeneratorService ??
                                     throw new ArgumentNullException(nameof(wishlistsGeneratorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _managerUrl = configuration["MANAGER_URL"] ?? throw new ArgumentNullException(nameof(_managerUrl));
        _teamLeadId = int.TryParse(configuration["TEAMLEAD_ID"], out var teamLeadId)
            ? teamLeadId
            : throw new ArgumentNullException(nameof(_teamLeadId));
        _serviceProvider = serviceProvider;
    }

    public async Task SendWishList(List<Employee> juniors, int hackathonId)
    {
        _logger.LogInformation("TeamLeadService started. Connecting to manager at {URL}.", _managerUrl);

        try
        {
            var wishlist = _wishlistsGeneratorService.GenerateWishlist(_teamLeadId, juniors);
            _logger.LogInformation("Successfully generated TeamLead wishlist with {Size} juniors.",
                wishlist.DesiredEmployees.Length);
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var publisher = scope.ServiceProvider.GetRequiredService<ITeamLeadPublisher>();
                publisher.SendWishList(wishlist, hackathonId);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while connecting to the manager.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing the response from the manager.");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning("Operation was canceled: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in TeamLeadService.");
        }
    }
}