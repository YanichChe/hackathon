using hackathon.context.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace hackathon.context;

using Microsoft.EntityFrameworkCore;
using model;

public class HackathonContext : DbContext
{
    public DbSet<Hackathon> Hackathons { get; set; }
    public DbSet<Junior> Juniors { get; set; }
    public DbSet<TeamLead> TeamLeads { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Team> Teams { get; set; }
    
    public HackathonContext(DbContextOptions<HackathonContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Host=localhost;Database=HackathonDB;Username=admin;Password=admin";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
