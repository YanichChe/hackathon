using HrManager.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;

namespace HrManager.controllers;

[Route("api/[controller]")]
[ApiController]
public class HrManagerController(
    IHrManagerService hrManagerService,
    ILogger<HrManagerController> logger
) : ControllerBase
{
    [HttpPost("team-lead")]
    public IResult SaveTeamLeadWishList([FromBody] Wishlist wishlist)
    {
        logger.Log(LogLevel.Information, "Got team lead wishlist");
        hrManagerService.SaveTeamLeadWishList(wishlist, 1);
        return Results.Ok(new { message = "Team lead wishlist saved successfully." });
    }

    [HttpPost("junior")]
    public IResult SaveJuniorWishList([FromBody] Wishlist wishlist)
    {
        logger.Log(LogLevel.Information, "Got junior wishlist");
        hrManagerService.SaveJuniorWishList(wishlist, 1);
        return Results.Ok(new { message = "Junior wishlist saved successfully." });
    }

    [HttpGet("juniors")]
    public IResult GetJuniors()
    {
        logger.Log(LogLevel.Information, "Got juniors");
        var juniors = hrManagerService.GetJuniors();
        if (!juniors.Any()) return Results.NotFound("No juniors found.");

        return Results.Ok(juniors);
    }

    [HttpGet("team-leads")]
    public IResult GetTeamLeads()
    {
        logger.Log(LogLevel.Information, "Got team-leads");
        var teamLeads = hrManagerService.GetTeamLeads();
        if (!teamLeads.Any()) return Results.NotFound("No team leads found.");

        return Results.Ok(teamLeads);
    }
}