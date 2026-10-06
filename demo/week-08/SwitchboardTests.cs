// Your instructor's fact, written in class before the fix — red first.

namespace Lab.Tests;

public class SwitchboardTests
{
    [Fact]
    public void ACallerRemembersTheirCalls()
    {
        string path = Path.Combine(Path.GetTempPath(), "kdxr-demo-switchboard.json");
        File.Delete(path);

        Caller dorothy = new Caller("Dorothy");
        dorothy.Calls();
        dorothy.Calls();
        dorothy.Calls();
        dorothy.Calls();

        Switchboard board = new Switchboard();
        board.Add(dorothy);
        board.Save(path);

        Switchboard reopened = new Switchboard();
        reopened.Load(path);

        Assert.Equal(1, reopened.Count);
        Assert.Equal(4, reopened.Find("Dorothy")!.CallsTonight);
    }
}
