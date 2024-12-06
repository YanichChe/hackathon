using Director.context.model;

namespace Director.context.repository;

public interface IHackathonRepository
{
    public void Update(Hackathon hackathon);
    public Hackathon? GetHackathon(int id);
}

public class HackathonRepository(HackathonContext hackathonContext) : IHackathonRepository
{
    public Hackathon? GetHackathon(int id)
    {
        return hackathonContext.Hackathons.Find(id);
    }

    public void Update(Hackathon hackathon)
    {
        hackathonContext.Update(hackathon);
        hackathonContext.SaveChanges();
    }
}