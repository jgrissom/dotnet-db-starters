// ═══════════════════════════════════════════════════════════════════
//  YOURS. Started in week 7, added to every week since.
//
//  Three facts in it so far. One more goes in tonight, at Task 1 — and it
//  is a different KIND of fact from the three above it. Those three
//  caught something. This one gives you permission to change code that
//  already works.
//
//  ⚠️ Every path here is a scratch path. `dotnet test` runs from inside
//  Lab.Tests/bin/Debug/net10.0 and `dotnet run` runs from the top of the
//  repo, so a plain name would mean two different files.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Tests;

public class DeskTests
{
    // Week 7's worked example. Set the scene, do the thing, check the
    // answer — except this one needs no scene at all, because CallSign is
    // static and asks for nothing.
    [Fact]
    public void TheStationKnowsItsOwnName()
    {
        Assert.Equal("KDXR", Broadcast.CallSign());
    }

    // Week 8. A fact about a file — the only new thing was the scratch path.
    //
    // ⚠️ This one is quietly load-bearing tonight: Task 1 rewrites LastShift,
    // and the empty-file case is exactly what LastOrDefault changes. It was
    // green before and it is green after.
    [Fact]
    public void ADeskNobodyHasSignedOffHasNothingToSay()
    {
        string path = Path.Combine(Path.GetTempPath(), "kdxr-mine-nolog.txt");
        File.Delete(path);

        Assert.Equal("", Broadcast.LastShift(path));
    }

    // Week 8. A cart aired twice, saved and loaded, still says it aired twice.
    [Fact]
    public void ACartRemembersItsPlays()
    {
        string path = Path.Combine(Path.GetTempPath(), "kdxr-mine-rotation.json");
        File.Delete(path);

        Song nightjar = new Song("Nightjar", "The Lamplighters", 227);
        nightjar.Play();
        nightjar.Play();

        Rotation rotation = new Rotation();
        rotation.Add(nightjar);
        rotation.Save(path);

        // A second rotation, holding nothing, reading the same file. Loading
        // into the one that just saved would prove nothing.
        Rotation reopened = new Rotation();
        reopened.Load(path);

        Assert.Equal(1, reopened.Count);
        Assert.Equal(2, reopened.All()[0].PlaysTonight);
    }

    // TODO — Task 1. A fact about something you are ABOUT TO REWRITE.
    //
    // Pin down what Switchboard.TheRegular() answers right now, while it is
    // still the loop you have read. Two situations are worth pinning, and the
    // second is the one a rewrite breaks — the lab says which.
    //
    // Then run it. It goes green immediately, which is not a mistake: it is
    // describing code that already works. It is what makes deleting that code
    // a safe thing to do rather than a brave one.
}
