using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class DeskContext : DbContext
{
    public DbSet<Caller> Callers { get; set; }

    public DbSet<Song> Songs { get; set; }

    // TODO — Task 1: one table for your instructor's requests, and one for
    // your airings.

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

        // TODO — Task 3: the log, so the desk prints every command it sends.
    }
}
