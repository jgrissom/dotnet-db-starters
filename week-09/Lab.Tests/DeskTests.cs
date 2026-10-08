// ═══════════════════════════════════════════════════════════════════
//  YOURS. A fresh test project for week 9 — your earlier facts stay in
//  their own week folders.
//
//  Run it with:   dotnet test week-09/Lab.Tests
//
//  Three facts ship written, and all three are about things that already
//  work. Your instructor's facts live next door, in SwitchboardTests.cs.
//
//  Tonight you write one more, in Task 1, and you write it BEFORE you
//  change the method it is about. Names are yours; name it after the rule
//  it proves.
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

    // A cart aired twice, saved and loaded, still says it aired twice.
    // This is the fact that is watching Load while you rewrite its loop.
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

    // TODO — Task 1. One fact about Rotation.Find: it hands back the cart the
    // rotation is holding, and null for a title nobody has. Write it BEFORE
    // you touch Find.
}
