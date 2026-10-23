// The request line's one table, in your instructor's database.

using Microsoft.EntityFrameworkCore;

public class LineContext : DbContext
{
    private readonly string _connection;

    public DbSet<Request> Requests { get; set; }

    public LineContext(string connection)
    {
        _connection = connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_connection);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        // The table has a trigger that writes the login of whoever rang onto
        // the row, so nobody can ring in under somebody else's name. EF Core
        // has to be told the trigger is there, or it refuses to save.
        model.Entity<Request>().ToTable("Requests", table => table.HasTrigger("Requests_StampLogin"));

        model.Entity<Request>().Property(request => request.RangAt).ValueGeneratedOnAdd();
    }
}
