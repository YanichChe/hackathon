namespace hackathon.context.repository.impl;

public class HackathonRepository : IHackathonRepository
{
    private readonly HackathonContext _hackathonContext;

    public HackathonRepository(HackathonContext hackathonContext)
    {
        _hackathonContext = hackathonContext;
    }

    public void Save(model.Hackathon hackathon)
    {
        _hackathonContext.Hackathons.Add(hackathon);
        _hackathonContext.SaveChanges();
    }

    public void Update(model.Hackathon hackathon)
    {
        _hackathonContext.Update(hackathon);
        _hackathonContext.SaveChanges();
    }

    public List<model.Hackathon> FindAll()
    {
        return _hackathonContext.Hackathons.ToList();
    }

    public void Clean()
    {
        _hackathonContext.Hackathons.RemoveRange(_hackathonContext.Hackathons);
        _hackathonContext.SaveChanges();
    }
}
