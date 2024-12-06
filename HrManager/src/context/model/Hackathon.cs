namespace HrManager.context.model;

public class Hackathon
{
    public int Id { get; set; }
    public double Harmoniousness { get; set; }

    public List<Wishlist> Wishlists { get; set; } = new();
    public List<Team> Teams { get; set; } = new();
}

public class Junior
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Wishlist> Wishlists { get; set; } = new();
}

public class TeamLead
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Wishlist> Wishlists { get; set; } = new();
}

public class Wishlist
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public bool IsForTeamLead { get; set; }

    public Junior Junior { get; set; }
    public TeamLead TeamLead { get; set; }
    public Hackathon Hackathon { get; set; }
}

public class Team
{
    public int Id { get; set; }

    public Hackathon Hackathon { get; set; }
    public Junior Junior { get; set; }
    public TeamLead TeamLead { get; set; }
}