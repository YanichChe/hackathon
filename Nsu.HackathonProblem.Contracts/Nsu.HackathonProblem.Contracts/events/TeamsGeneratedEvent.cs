namespace Nsu.HackathonProblem.Contracts.events;

public class TeamsGeneratedEvent
{
    public int hackathonId { get; set; }
    public List<Team> teams { get; set; }
}