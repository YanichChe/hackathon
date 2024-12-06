using HrManager.context.model;

namespace HrManager.context.repository;

public interface IHackathonRepository
{
    public Hackathon? GetHackathon(int id);
    public void Save(Hackathon hackathon);
}

public class HackathonRepository(HackathonContext hackathonContext) : IHackathonRepository
{
    public Hackathon? GetHackathon(int id)
    {
        return hackathonContext.Hackathons.Find(id);
    }

    public void Save(Hackathon hackathon)
    {
        hackathonContext.Hackathons.Add(hackathon);
        hackathonContext.SaveChanges();
    }
}