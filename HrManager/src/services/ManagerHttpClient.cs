using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;

namespace HrManager;

public class ManagerHttpClient
{
    private readonly string _directorUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ManagerHttpClient> _logger;

    public ManagerHttpClient(HttpClient httpClient, ILogger<ManagerHttpClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _directorUrl = configuration["DIRECTOR_URL"] ?? throw new ArgumentNullException(nameof(_directorUrl));
    }

    public async Task SendReport(List<Wishlist> teamLeadsWishlists, List<Wishlist> juniorsWishlists, List<Team> teams)
    {
        try
        {
            var hackathonResultRequest = new HackathonResultRequest(juniorsWishlists, teamLeadsWishlists, teams);
            var response = await _httpClient.PostAsJsonAsync(_directorUrl, hackathonResultRequest);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("POST request to {url} was successful", _directorUrl);
            else
                _logger.LogError("POST request to {url} failed with status code: {statusCode}", _directorUrl,
                    response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending POST request to {url}", _directorUrl);
        }
    }

    public class HackathonResultRequest
    {
        public HackathonResultRequest(
            List<Wishlist> juniorWishlists,
            List<Wishlist> teamLeadWishlists,
            List<Team> teams)
        {
            JuniorWishlists = juniorWishlists ?? new List<Wishlist>();
            TeamLeadWishlists = teamLeadWishlists ?? new List<Wishlist>();
            Teams = teams ?? new List<Team>();
        }

        public HackathonResultRequest()
        {
            JuniorWishlists = new List<Wishlist>();
            TeamLeadWishlists = new List<Wishlist>();
            Teams = new List<Team>();
        }

        public List<Wishlist> JuniorWishlists { get; set; }
        public List<Wishlist> TeamLeadWishlists { get; set; }
        public List<Team> Teams { get; set; }
    }
}