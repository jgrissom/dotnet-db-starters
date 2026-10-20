// Your instructor's fact about WHEN a question gets asked.

namespace Lab.Tests;

public class BusyCallerTests
{
    [Fact]
    public void AnAnswerDoesNotChangeAfterItIsGiven()
    {
        Switchboard board = new Switchboard();
        board.Take("Dorothy");
        board.Take("Dorothy");
        board.Take("Bex");

        IEnumerable<Caller> busy = board.All().Where(caller => caller.CallsTonight > 1).ToList();

        board.Take("Bex");

        Assert.Single(busy);
    }
}
