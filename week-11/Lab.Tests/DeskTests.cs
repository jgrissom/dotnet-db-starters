// ═══════════════════════════════════════════════════════════════════
//  YOURS. A fresh test project for week 11 — your earlier facts stay in
//  their own week folders.
//
//  Run it with:   dotnet test week-11/Lab.Tests
//
//  Five facts ship written, every one of them green. Task 3 adds a
//  sixth, and it is red until Save learns to take a row out.
// ═══════════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;

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

    // A table nobody has saved to yet: loading nothing must leave the carts
    // the desk already has alone.
    [Fact]
    public void AFirstNightKeepsItsCarts()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Rotation rotation = new Rotation();
        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));

        rotation.Load(new DeskContext(options));

        Assert.Equal(1, rotation.Count);
    }

    // A cart aired twice, saved and loaded, still says it aired twice.
    [Fact]
    public void ACartRemembersItsPlays()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Song nightjar = new Song("Nightjar", "The Lamplighters", 227);
        nightjar.Play();
        nightjar.Play();

        Rotation rotation = new Rotation();
        rotation.Add(nightjar);
        rotation.Save(new DeskContext(options));

        // A second rotation, holding nothing, reading the same table. Loading
        // into the one that just saved would prove nothing.
        Rotation reopened = new Rotation();
        reopened.Load(new DeskContext(options));

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

    // The second night: a cart loaded, aired and saved again is still ONE
    // row, with the new play on it.
    [Fact]
    public void ACartAiredAgainIsStillOneRow()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Rotation rotation = new Rotation();
        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));
        rotation.Save(new DeskContext(options));

        DeskContext db = new DeskContext(options);
        Rotation nextNight = new Rotation();
        nextNight.Load(db);
        nextNight.All()[0].Play();
        nextNight.Save(db);

        Rotation reopened = new Rotation();
        reopened.Load(new DeskContext(options));

        Assert.Equal(1, reopened.Count);
        Assert.Equal(1, reopened.All()[0].PlaysTonight);
    }

    // TODO — Task 3: your fact about a pulled cart, written before the fix.
}
