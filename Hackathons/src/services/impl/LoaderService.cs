using hackathon.context;
using Microsoft.Extensions.Configuration;
using Nsu.HackathonProblem.Contracts;

namespace Hackathon.services.impl;

public class LoaderService(IConfiguration configuration): ILoaderService
{
    public List<Employee> LoadEmployees(string path)
    {
        var key = configuration[path];
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException($"Path for teamleads data not found in configuration for key '{path}'");
        }
        
        var a =  File.ReadAllLines(key)
            .Skip(1)
            .Select(line => line.Split(';'))
            .Select(parts => new Employee(Int32.Parse(parts[0]), parts[1]))
            .ToList();

        return a;
    }
}
