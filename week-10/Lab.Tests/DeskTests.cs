// ═══════════════════════════════════════════════════════════════════
//  YOURS. A fresh test project for week 10 — your earlier facts stay in
//  their own week folders.
//
//  Run it with:   dotnet test week-10/Lab.Tests
//
//  Four facts ship written. Two of them use a file, and in Task 4 they
//  stop compiling, because Save and Load stop taking a path. You move both
//  onto a table that lives inside the test run. In Task 5 you write one
//  more, BEFORE you change the method it is about.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Tests;

public class DeskTests
{
    // Set the scene, do the thing, check the answer — except this one
    // needs no scene at all, because CallSign is static and asks for nothing.
    [Fact]
    public void TheStationKnowsItsOwnName()
    {
        Assert.Equal("KDXR", Broadcast.CallSign());
    }

    // TODO — Task 4: a table nobody has saved to yet, instead of a missing file.
    //
    // A desk that has never signed off has no file, and loading nothing
    // must leave the carts it already had alone.
    [Fact]
    public void AFirstNightKeepsItsCarts()
    {
        string path = Path.Combine(Path.GetTempPath(), "kdxr-mine-nofile.json");
        File.Delete(path);

        Rotation rotation = new Rotation();
        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));

        rotation.Load(path);

        Assert.Equal(1, rotation.Count);
    }

    // TODO — Task 4: a table that lives inside the test run, instead of a file.
    //
    // A cart aired twice, saved and loaded, still says it aired twice.
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

    // Week 9. Written before Find was rewritten, to prove nothing moved.
    [Fact]
    public void FindHandsBackTheCartOrNothing()
    {
        Rotation rotation = new Rotation();
        Song nightjar = new Song("Nightjar", "The Lamplighters", 227);
        rotation.Add(nightjar);

        Assert.Same(nightjar, rotation.Find("Nightjar"));
        Assert.Null(rotation.Find("Owl Hours"));
    }

    // TODO — Task 5: your fact about the second save, written before the fix.
}
