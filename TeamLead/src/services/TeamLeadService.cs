using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nsu.HackathonProblem.Contracts;
using TeamLead.services;

namespace TeamLead.Services;

public class TeamLeadService : IHostedService
{
    private readonly HttpClient _client;
    private readonly ILogger<TeamLeadService> _logger;
    private readonly string _managerUrl;
    private readonly int _teamLeadId;
    private readonly IWishlistsGeneratorService _wishlistsGeneratorService;

    public TeamLeadService(HttpClient client,
        IWishlistsGeneratorService wishlistsGeneratorService,
        ILogger<TeamLeadService> logger,
        IConfiguration configuration)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _wishlistsGeneratorService = wishlistsGeneratorService ??
                                     throw new ArgumentNullException(nameof(wishlistsGeneratorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _managerUrl = configuration["MANAGER_URL"] ?? throw new ArgumentNullException(nameof(_managerUrl));
        _teamLeadId = int.TryParse(configuration["TEAMLEAD_ID"], out var teamLeadId)
            ? teamLeadId
            : throw new ArgumentNullException(nameof(_teamLeadId));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("TeamLeadService started. Connecting to manager at {URL}.", _managerUrl);

        try
        {
            var response = await _client.GetStringAsync($"{_managerUrl}/juniors", cancellationToken);
            var juniors = JsonConvert.DeserializeObject<List<Employee>>(response);

            var wishlist = _wishlistsGeneratorService.GenerateWishlist(_teamLeadId, juniors);
            _logger.LogInformation("Successfully generated TeamLead wishlist with {Size} juniors.",
                wishlist.DesiredEmployees.Length);

            var responseMessage =
                await _client.PostAsJsonAsync($"{_managerUrl}/team-lead", wishlist, cancellationToken);
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
            _logger.LogError(ex, "An unexpected error occurred in TeamLeadService.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("TeamLeadService is stopping.");
        return Task.CompletedTask;
    }
}