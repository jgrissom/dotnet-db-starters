// Your instructor's fact about a bad day list, written before the fix.

namespace Lab.Tests;

public class DayListTests
{
    [Fact]
    public void ABadListAddsNobody()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(path, """[ "Ray", "" ]""");

        Switchboard board = new Switchboard();

        Assert.Throws<InvalidDataException>(() => board.Import(path));
        Assert.Equal(0, board.Count);
    }
}
