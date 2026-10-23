using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class DeskContext : DbContext
{
    public DbSet<Caller> Callers { get; set; }

    public DbSet<Song> Songs { get; set; }

    public DeskContext()
    {
    }

    public DeskContext(DbContextOptions<DeskContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (options.IsConfigured)
        {
            return;
        }

        IConfiguration config = new ConfigurationBuilder()
            .AddUserSecrets<DeskContext>()
            .Build();

        options.UseSqlServer(config["ConnectionStrings:Desk"]);
    }
}
