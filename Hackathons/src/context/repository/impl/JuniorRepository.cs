using hackathon.context.model;

namespace hackathon.context.repository.impl;

public class JuniorRepository : IJuniorRepository
{
    private readonly HackathonContext _hackathonContext;

    public JuniorRepository(HackathonContext hackathonContext)
    {
        _hackathonContext = hackathonContext;
    }

    public void Save(List<Junior> juniors)
    {
        juniors.ForEach(j => _hackathonContext.Juniors.Add(j));
        _hackathonContext.SaveChanges();
    }

    public Junior? FindById(int id)
    {
        return _hackathonContext.Juniors.Find(id);
    }

    public void Clean()
    {
        _hackathonContext.Juniors.RemoveRange(_hackathonContext.Juniors);
        _hackathonContext.SaveChanges();
    }
}
