using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services;

public interface IHRDirectorService
{
    public double CalculateHarmonicity(List<int> satisfactionPoints);
}
