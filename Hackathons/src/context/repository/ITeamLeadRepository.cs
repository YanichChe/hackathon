using hackathon.context.model;

namespace hackathon.context.repository;

public interface ITeamLeadRepository
{
    public void Save(List<TeamLead> teamLeads);
    public TeamLead? FindById(int id);
    public void Clean();
}
