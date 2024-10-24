using hackathon.context.model;

namespace hackathon.context.repository.impl;

public class TeamRepository : ITeamRepository
{
    private readonly HackathonContext _hackathonContext;

    public TeamRepository(HackathonContext hackathonContext)
    {
        _hackathonContext = hackathonContext;
    }
    
    public void Save(Team team)
    {
        _hackathonContext.Teams.Add(team);
        _hackathonContext.SaveChanges();
    }
}
