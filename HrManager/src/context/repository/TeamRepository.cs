using HrManager.context.model;

namespace HrManager.context.repository;

public interface ITeamRepository
{
    public void Save(Team team);
}

public class TeamRepository(HackathonContext hackathonContext) : ITeamRepository
{
    public void Save(Team team)
    {
        hackathonContext.Teams.Add(team);
        hackathonContext.SaveChanges();
    }
}