// Your instructor's fact about the second night, written before the fix.

using Microsoft.EntityFrameworkCore;

namespace Lab.Tests;

public class SecondSaveTests
{
    [Fact]
    public void ACallerWhoRingsAgainIsStillOneRow()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Switchboard board = new Switchboard();
        board.Take("Dorothy");
        board.Save(new DeskContext(options));

        DeskContext db = new DeskContext(options);
        Switchboard nextNight = new Switchboard();
        nextNight.Load(db);
        nextNight.Take("Dorothy");
        nextNight.Save(db);

        Switchboard reopened = new Switchboard();
        reopened.Load(new DeskContext(options));

        Assert.Equal(1, reopened.Count);
        Assert.Equal(2, reopened.Find("Dorothy")!.CallsTonight);
    }
}
