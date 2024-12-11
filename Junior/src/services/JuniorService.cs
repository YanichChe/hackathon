using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nsu.HackathonProblem.Contracts;
using Junior.publisher;
using Junior.services;

namespace Junior.Services;

public interface IJuniorService
{
    public Task SendWishList(List<Employee> juniors, int hackathonId);
}

public class JuniorService : IJuniorService
{
    private readonly ILogger<JuniorService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWishlistsGeneratorService _wishlistsGeneratorService;
    private readonly string _managerUrl;
    private readonly int _juniorId;
    
    public JuniorService(
        IWishlistsGeneratorService wishlistsGeneratorService,
        ILogger<JuniorService> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _wishlistsGeneratorService = wishlistsGeneratorService ??
                                     throw new ArgumentNullException(nameof(wishlistsGeneratorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _managerUrl = configuration["MANAGER_URL"] ?? throw new ArgumentNullException(nameof(_managerUrl));
        _juniorId = int.TryParse(configuration["JUNIOR_ID"], out var juniorId)
            ? juniorId
            : throw new ArgumentNullException(nameof(_juniorId));
        _serviceProvider = serviceProvider;
    }

    public async Task SendWishList(List<Employee> juniors, int hackathonId)
    {
        _logger.LogInformation("JuniorService started. Connecting to manager at {URL}.", _managerUrl);

        try
        {
            var wishlist = _wishlistsGeneratorService.GenerateWishlist(_juniorId, juniors);
            _logger.LogInformation("Successfully generated Junior wishlist with {Size} juniors.",
                wishlist.DesiredEmployees.Length);
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var publisher = scope.ServiceProvider.GetRequiredService<IJuniorPublisher>();
                publisher.SendWishList(wishlist, hackathonId );
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
            _logger.LogError(ex, "An unexpected error occurred in JuniorService.");
        }
    }
}