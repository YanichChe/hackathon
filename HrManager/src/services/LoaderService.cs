using Microsoft.Extensions.Configuration;
using Nsu.HackathonProblem.Contracts;

namespace HrManager.services;

public interface ILoaderService
{
    List<Employee> LoadEmployees(string path);
}

public class LoaderService(IConfiguration configuration) : ILoaderService
{
    public List<Employee> LoadEmployees(string path)
    {
        var key = configuration[path];
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException($"Path for data not found in configuration for key '{path}'");

        return File.ReadAllLines(key)
            .Skip(1)
            .Select(line => line.Split(';'))
            .Select(parts => new Employee(int.Parse(parts[0]), parts[1]))
            .ToList();
    }
}