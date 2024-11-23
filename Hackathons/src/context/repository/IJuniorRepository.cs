using hackathon.context.model;

namespace hackathon.context.repository;

public interface IJuniorRepository
{
    public void Save(List<Junior> juniors);
    public Junior? FindById(int id);
    public void Clean();
}
