namespace Nsu.HackathonProblem.Contracts.events;

public class TeamLeadListGeneratedEvent
{
    public int hackathonId { get; set; }
    public List<Employee> teamLeads { get; set; }
}