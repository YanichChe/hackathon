using Director.context.model;
using Director.context.repository;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts.events;

namespace Director.publisher;

public interface IDirectorPublisher
{
    public void SendStartHackathon();
}

public class DirectorPublisher(IBus bus, IHackathonRepository hackathonRepository, ILogger<DirectorPublisher> logger)
    : IDirectorPublisher
{
    public void SendStartHackathon()
    {
        var hackathon = new Hackathon();
        hackathonRepository.Save(hackathon);
        
        bus.Publish(new HackathonCreatedEvent
        {
            hackathonId = hackathon.Id,
        });
        logger.LogInformation("Hackathon id sent");
    }
}