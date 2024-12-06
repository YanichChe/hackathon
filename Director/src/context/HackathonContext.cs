using Director.context.model;
using Microsoft.EntityFrameworkCore;

namespace Director.context;

public class HackathonContext(DbContextOptions<HackathonContext> options) : DbContext
{
    public DbSet<Hackathon> Hackathons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Host=db;Database=HackathonDB;Username=admin;Password=admin";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}