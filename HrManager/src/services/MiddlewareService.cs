using System.Collections.Concurrent;
using System.Text.Json;
using HrManager.services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;

namespace HrManager;

public class MiddlewareService
{
    private readonly ILogger<MiddlewareService> _logger;
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<int, bool> juniorIds = new();
    private readonly List<Wishlist> juniorsWishlists = new();

    private readonly object syncLock = new();
    private readonly ConcurrentDictionary<int, bool> teamLeadIds = new();
    private readonly List<Wishlist> teamLeadsWishlists = new();
    private bool methodCalled;

    public MiddlewareService(RequestDelegate next, ILogger<MiddlewareService> logger, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var hackathonService = scope.ServiceProvider.GetRequiredService<IHackathonService>();
            var managerHttpClient = scope.ServiceProvider.GetRequiredService<ManagerHttpClient>();

            context.Request.EnableBuffering();

            if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                var path = context.Request.Path;
                var bodyAsString = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };

                        var wishlist = JsonSerializer.Deserialize<Wishlist>(bodyAsString, options);
                        if (wishlist != null)
                            lock (syncLock)
                            {
                                if (path.ToString().Contains("/team-lead", StringComparison.OrdinalIgnoreCase))
                                {
                                    teamLeadIds.TryAdd(wishlist.EmployeeId, true);
                                    teamLeadsWishlists.Add(wishlist);
                                }

                                if (path.ToString().Contains("/junior", StringComparison.OrdinalIgnoreCase))
                                {
                                    juniorIds.TryAdd(wishlist.EmployeeId, true);
                                    juniorsWishlists.Add(wishlist);
                                }

                                if (!methodCalled && teamLeadIds.Count == 5 && juniorIds.Count == 5)
                                {
                                    _logger.LogInformation("Both sets contain 5 elements. Calling the method.");
                                    var teams = hackathonService.StartHackathon(1, teamLeadsWishlists,
                                        juniorsWishlists);
                                    var result =
                                        managerHttpClient.SendReport(teamLeadsWishlists, juniorsWishlists, teams);
                                    methodCalled = true;
                                }
                            }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"Error parsing Wishlist JSON: {ex.Message}");
                    }
            }

            await _next(context);
        }
    }
}