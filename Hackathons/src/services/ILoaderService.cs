using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services;

public interface ILoaderService
{
    List<Employee> LoadEmployees(string path);
}
