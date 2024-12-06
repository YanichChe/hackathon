using HrManager.context.model;
using Microsoft.EntityFrameworkCore;

namespace HrManager.context;

public class HackathonContext(DbContextOptions<HackathonContext> options) : DbContext
{
    public DbSet<Hackathon> Hackathons { get; set; }
    public DbSet<Junior> Juniors { get; set; }
    public DbSet<TeamLead> TeamLeads { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Host=db;Database=HackathonDB;Username=admin;Password=admin";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}