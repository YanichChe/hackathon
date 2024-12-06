using HrManager.context.model;

namespace HrManager.context.repository;

public interface IJuniorRepository
{
    public void Save(List<Junior> juniors);
    public Junior? FindById(int id);
    public List<Junior> GetAll();
}

public class JuniorRepository(HackathonContext hackathonContext) : IJuniorRepository
{
    public void Save(List<Junior> juniors)
    {
        foreach (var junior in juniors)
        {
            var existingJunior = hackathonContext.Juniors
                .FirstOrDefault(t => t.Id == junior.Id);

            if (existingJunior != null)
                hackathonContext.Entry(existingJunior).CurrentValues.SetValues(junior);
            else
                hackathonContext.Juniors.Add(junior);
        }

        hackathonContext.SaveChanges();
    }

    public Junior? FindById(int id)
    {
        return hackathonContext.Juniors.Find(id);
    }

    public List<Junior> GetAll()
    {
        return hackathonContext.Juniors.ToList();
    }
}