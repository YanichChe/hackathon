using Director.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;

namespace Director.controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectorController(
    IDirectorService directorService,
    ILogger<DirectorController> logger
) : ControllerBase
{
    public IActionResult GetHackathonResult([FromBody] HackathonResultRequest request)
    {
        logger.Log(LogLevel.Information, "Got hackathon result");
        var harmonicity =
            directorService.CalculateHarmonicity(request.Teams, request.TeamLeadWishlists, request.JuniorWishlists);
        logger.LogInformation($"Hackathon result = {harmonicity}");
        return Ok();
    }

    public class HackathonResultRequest
    {
        public List<Wishlist> JuniorWishlists { get; set; }
        public List<Wishlist> TeamLeadWishlists { get; set; }
        public List<Team> Teams { get; set; }
    }
}