// Your instructor's fact, written in class before the fix — red first.

using Microsoft.EntityFrameworkCore;

namespace Lab.Tests;

public class SwitchboardTests
{
    [Fact]
    public void ACallerRemembersTheirCalls()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Caller dorothy = new Caller("Dorothy");
        dorothy.Calls();
        dorothy.Calls();
        dorothy.Calls();
        dorothy.Calls();

        Switchboard board = new Switchboard();
        board.Add(dorothy);
        board.Save(new DeskContext(options));

        Switchboard reopened = new Switchboard();
        reopened.Load(new DeskContext(options));

        Assert.Equal(1, reopened.Count);
        Assert.Equal(4, reopened.Find("Dorothy")!.CallsTonight);
    }

    [Fact]
    public void FindHandsBackTheCallerOrNothing()
    {
        Switchboard board = new Switchboard();
        Caller dorothy = board.Take("Dorothy");

        Assert.Same(dorothy, board.Find("Dorothy"));
        Assert.Null(board.Find("Ray"));
    }
}
