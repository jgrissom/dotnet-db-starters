// Your instructor's fact about taking a caller off, written before the fix.

using Microsoft.EntityFrameworkCore;

namespace Lab.Tests;

public class RemovedCallerTests
{
    [Fact]
    public void ACallerTakenOffStaysOff()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Switchboard board = new Switchboard();
        board.Take("Dorothy");
        board.Take("Bex");
        board.Save(new DeskContext(options));

        DeskContext db = new DeskContext(options);
        Switchboard nextNight = new Switchboard();
        nextNight.Load(db);
        nextNight.Remove("Bex");
        nextNight.Save(db);

        Switchboard reopened = new Switchboard();
        reopened.Load(new DeskContext(options));

        Assert.Equal(1, reopened.Count);
        Assert.Null(reopened.Find("Bex"));
    }
}
