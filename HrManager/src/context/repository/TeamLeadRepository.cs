using HrManager.context.model;

namespace HrManager.context.repository;

public interface ITeamLeadRepository
{
    public void Save(List<TeamLead> teamLeads);
    public TeamLead? FindById(int id);
    public List<TeamLead> GetAll();
}

public class TeamLeadRepository(HackathonContext hackathonContext) : ITeamLeadRepository
{
    public void Save(List<TeamLead> teamLeads)
    {
        foreach (var teamLead in teamLeads)
        {
            var existingTeamLead = hackathonContext.TeamLeads
                .FirstOrDefault(t => t.Id == teamLead.Id);

            if (existingTeamLead != null)
                hackathonContext.Entry(existingTeamLead).CurrentValues.SetValues(teamLead);
            else
                hackathonContext.TeamLeads.Add(teamLead);
        }

        hackathonContext.SaveChanges();
    }


    public TeamLead? FindById(int id)
    {
        return hackathonContext.TeamLeads.Find(id);
    }

    public List<TeamLead> GetAll()
    {
        return hackathonContext.TeamLeads.ToList();
    }
}