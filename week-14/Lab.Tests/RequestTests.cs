// Your instructor's fact about a caller's requests, written before the fix.

using Microsoft.EntityFrameworkCore;

namespace Lab.Tests;

public class RequestTests
{
    [Fact]
    public void ACallersRequestsComeBack()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Switchboard board = new Switchboard();
        Caller dorothy = board.Take("Dorothy");
        dorothy.Asks(new Song("Nightjar", "The Lamplighters", 227));
        board.Save(new DeskContext(options));

        Switchboard reopened = new Switchboard();
        reopened.Load(new DeskContext(options));

        Assert.Single(reopened.Find("Dorothy")!.Requests);
    }
}
