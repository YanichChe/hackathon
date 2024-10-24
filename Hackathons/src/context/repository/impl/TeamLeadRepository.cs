using hackathon.context.model;

namespace hackathon.context.repository.impl;

public class TeamLeadRepository : ITeamLeadRepository
{
    private readonly HackathonContext _hackathonContext;

    public TeamLeadRepository(HackathonContext hackathonContext)
    {
        _hackathonContext = hackathonContext;
    }

    public void Save(List<TeamLead> teamLeads)
    {
        teamLeads.ForEach(t => _hackathonContext.TeamLeads.Add(t));
        _hackathonContext.SaveChanges();
    }

    public TeamLead? FindById(int id)
    {
        return _hackathonContext.TeamLeads.Find(id);
    }

    public void Clean()
    {
        _hackathonContext.TeamLeads.RemoveRange(_hackathonContext.TeamLeads);
        _hackathonContext.SaveChanges();
    }
}
