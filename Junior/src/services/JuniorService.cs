using System.Net.Http.Json;
using Junior.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nsu.HackathonProblem.Contracts;

namespace Junior.Services;

public class JuniorService : IHostedService
{
    private readonly HttpClient _client;
    private readonly int _juniorId;
    private readonly ILogger<JuniorService> _logger;
    private readonly string _managerUrl;
    private readonly IWishlistsGeneratorService _wishlistsGeneratorService;

    public JuniorService(HttpClient client,
        IWishlistsGeneratorService wishlistsGeneratorService,
        ILogger<JuniorService> logger,
        IConfiguration configuration)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _wishlistsGeneratorService = wishlistsGeneratorService ??
                                     throw new ArgumentNullException(nameof(wishlistsGeneratorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _managerUrl = configuration["MANAGER_URL"] ?? throw new ArgumentNullException(nameof(_managerUrl));
        _juniorId = int.TryParse(configuration["JUNIOR_ID"], out var juniorId)
            ? juniorId
            : throw new ArgumentNullException(nameof(_juniorId));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JuniorService started. Connecting to manager at {URL}.", _managerUrl);

        try
        {
            var response = await _client.GetStringAsync($"{_managerUrl}/team-leads", cancellationToken);
            var teamLeads = JsonConvert.DeserializeObject<List<Employee>>(response);

            var wishlist = _wishlistsGeneratorService.GenerateWishlist(_juniorId, teamLeads);
            _logger.LogInformation("Successfully generated Junior wishlist with {Size} team leads.",
                wishlist.DesiredEmployees.Length);

            var responseMessage = await _client.PostAsJsonAsync($"{_managerUrl}/junior", wishlist, cancellationToken);
            if (responseMessage.IsSuccessStatusCode)
                _logger.LogInformation("Wishlist successfully submitted to manager.");
            else
                _logger.LogWarning("Failed to submit wishlist to manager. Status code: {StatusCode}",
                    responseMessage.StatusCode);
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JuniorService is stopping.");
        return Task.CompletedTask;
    }
}