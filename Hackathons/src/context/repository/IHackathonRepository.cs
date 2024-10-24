namespace hackathon.context.repository;

public interface IHackathonRepository
{
    public void Save(model.Hackathon hackathon);
    public void Update(model.Hackathon hackathon);
    public List<model.Hackathon> FindAll();
    public void Clean();
}
