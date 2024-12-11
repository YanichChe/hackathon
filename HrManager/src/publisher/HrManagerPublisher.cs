using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;
using Nsu.HackathonProblem.Contracts.events;

namespace HrManager.publisher;

public interface IHrManagerPublisher
{
    public void SendJuniors(List<Employee> employee, int hackathonId);
    public void SendTeamLeads(List<Employee> employee, int hackathonId);
    public void SendTeams(List<Team> teams, int hackathonId);
}

public class HrManagerPublisher(IBus bus, ILogger<HrManagerPublisher> logger) : IHrManagerPublisher
{
    public void SendJuniors(List<Employee> employee, int hackathonId)
    {
        bus.Publish(new JuniorListGeneratedEvent
        {
            hackathonId = hackathonId,
            juniors =  employee
        });
        
        logger.LogInformation("Juniors sent");
    }
    
    public void SendTeamLeads(List<Employee> employee, int hackathonId)
    {
        bus.Publish(new TeamLeadListGeneratedEvent
        {
            hackathonId = hackathonId,
            teamLeads =  employee
        });
        
        logger.LogInformation("TeamLeads sent");
    }

    public void SendTeams(List<Team> teams, int hackathonId)
    {
        bus.Publish(new TeamsGeneratedEvent
        {
            teams = teams,
            hackathonId = hackathonId
        });
        logger.LogInformation("Teams sent");
    }
}
