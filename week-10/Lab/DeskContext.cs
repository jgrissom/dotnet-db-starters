// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the desk's database.
//
//  One line of Task 2 happens in here, and the rest of the file ships
//  finished — the two constructors and OnConfiguring are ceremony you will
//  meet again in your own project and never have to think hard about.
//
//  A DbContext is two things at once: the list of tables this program knows
//  about, and the thing that knows how to reach them. The list is the part
//  that is missing.
// ═══════════════════════════════════════════════════════════════════
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class DeskContext : DbContext
{
    // TODO — Task 2. One line, and it is the line that turns a class into a
    // table. A DbSet<T> property says "this program keeps a table of these",
    // and the NAME you give the property is the name of the table.
    //
    // Call it Carts. The checks do not care what it is called, but the rest
    // of this lab does, and so does every SELECT you are about to write.

    // The desk uses this one, and so does `dotnet ef`. Neither hands in a
    // connection string, so OnConfiguring goes and finds one.
    public DeskContext()
    {
    }

    // The tests use this one. They hand in somewhere else to keep the rows,
    // which is why the suite needs no database, no password and no network.
    public DeskContext(DbContextOptions<DeskContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Somebody has already said where the rows go — that is a test,
        // handing in its own. Do not argue with it.
        if (options.IsConfigured)
        {
            return;
        }

        // ⚠️ A console program does not get configuration for free. Nothing
        // has read a settings file, and there is no settings file to read.
        // This builds the configuration by hand, from one source: the secret
        // store on this machine, which is not in the repo and never will be.
        IConfiguration config = new ConfigurationBuilder()
            .AddUserSecrets<DeskContext>()
            .Build();

        options.UseSqlServer(config["ConnectionStrings:Desk"]);
    }
}
