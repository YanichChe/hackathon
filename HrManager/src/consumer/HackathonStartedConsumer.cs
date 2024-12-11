using HrManager.context.repository;
using HrManager.publisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;
using Nsu.HackathonProblem.Contracts.events;

namespace HrManager.consumer;
using MassTransit;

public class HackathonStartedConsumer : IConsumer<HackathonCreatedEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public HackathonStartedConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task Consume(ConsumeContext<HackathonCreatedEvent> context)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
            var juniorRepository = scope.ServiceProvider.GetRequiredService<IJuniorRepository>();
            var teamLeadRepository = scope.ServiceProvider.GetRequiredService<ITeamLeadRepository>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var publisher = scope.ServiceProvider.GetRequiredService<IHrManagerPublisher>();

            HackathonCreatedEvent hackathonCreatedEvent = context.Message as HackathonCreatedEvent;
            logger.LogInformation("Consuming message");

            try
            {
                var hackathon = hackathonRepository.GetHackathon(hackathonCreatedEvent.hackathonId)!;
                logger.LogInformation($"Created new Hackathon {hackathon.Id}");
                var juniors = juniorRepository.GetAll();
                var teamLeads = teamLeadRepository.GetAll();
                publisher.SendJuniors(juniors.Select(j => new Employee(j.Id, j.Name)).ToList(), hackathon.Id);
                publisher.SendTeamLeads(teamLeads.Select(j => new Employee(j.Id, j.Name)).ToList(), hackathon.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save hackathon");
                throw;
            }
            
            logger.LogInformation("END INIT");
            return Task.CompletedTask;
        }
    }
}