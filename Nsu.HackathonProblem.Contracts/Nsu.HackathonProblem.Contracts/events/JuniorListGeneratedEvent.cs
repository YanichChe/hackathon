namespace Nsu.HackathonProblem.Contracts.events;

public class JuniorListGeneratedEvent
{
    public int hackathonId { get; set; }
    public List<Employee> juniors { get; set; }
}