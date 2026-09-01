// ═══════════════════════════════════════════════════════════════════
//  YOURS. This is the suite you started last week, carried forward.
//
//  Run it with:   dotnet test week-08/Lab.Tests
//
//  Two facts ship written, and both are about things that already work —
//  a worked example to read, and one that shows what a fact looks like
//  when the thing it asks about lives in a file.
//
//  Tonight you write one more, in Task 4, and you write it BEFORE the fix
//  so you get to watch it go red. Names are yours; name it after the rule
//  it proves.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Tests;

public class DeskTests
{
    // The worked example, from last week. Set the scene, do the thing,
    // check the answer — except this one needs no scene at all, because
    // CallSign is static and asks for nothing.
    [Fact]
    public void TheStationKnowsItsOwnName()
    {
        Assert.Equal("KDXR", Broadcast.CallSign());
    }

    // A fact about a file. The only new thing here is the first line: a
    // scratch path, in the folder the system keeps for exactly this, so the
    // test never touches week-08/air-log.txt.
    //
    // ⚠️ It has to be a path of its own for a reason you can measure:
    // `dotnet run` stands at the top of your repo and `dotnet test` stands
    // inside bin/Debug/net10.0. The same relative name means two different
    // files depending on which one you typed.
    [Fact]
    public void ADeskNobodyHasSignedOffHasNothingToSay()
    {
        string path = Path.Combine(Path.GetTempPath(), "kdxr-mine-nolog.txt");
        File.Delete(path);

        Assert.Equal("", Broadcast.LastShift(path));
    }

    // TODO — Task 4: a cart aired twice, saved and loaded, still says it
    // aired twice.
    //
    // Set the scene:  a Song, played twice, in a Rotation
    // Do the thing:   Save it to a scratch path, then Load it into a NEW Rotation
    // Check:          Assert.Equal — what should PlaysTonight say?
    //
    // Write it, run it, and watch it fail before you touch Song.cs.
}
