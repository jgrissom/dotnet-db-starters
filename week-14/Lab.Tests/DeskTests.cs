// ═══════════════════════════════════════════════════════════════════
//  YOURS. A fresh test project for week 14 — your earlier facts stay in
//  their own week folders.
//
//  Run it with:   dotnet test week-14/Lab.Tests
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

    // Week 11. A cart pulled on the second night is gone from the table, and
    // the cart that wasn't pulled is still there.
    [Fact]
    public void APulledCartStaysPulled()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Rotation rotation = new Rotation();
        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));
        rotation.Add(new Song("Slack Water", "Marguerite Vance", 252));
        rotation.Save(new DeskContext(options));

        DeskContext db = new DeskContext(options);
        Rotation nextNight = new Rotation();
        nextNight.Load(db);
        nextNight.Remove("Nightjar");
        nextNight.Save(db);

        Rotation reopened = new Rotation();
        reopened.Load(new DeskContext(options));

        Assert.Equal(1, reopened.Count);
        Assert.Null(reopened.Find("Nightjar"));
    }

    // A cart that went out on air comes back the next night with
    // its airing.
    [Fact]
    public void ACartsAiringsComeBack()
    {
        DbContextOptions<DeskContext> options = new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Song nightjar = new Song("Nightjar", "The Lamplighters", 227);
        nightjar.Play();

        Rotation rotation = new Rotation();
        rotation.Add(nightjar);
        rotation.Save(new DeskContext(options));

        Rotation reopened = new Rotation();
        reopened.Load(new DeskContext(options));

        Assert.Single(reopened.Find("Nightjar")!.Airings);
    }

    // Task 3. A delivery with one bad cart throws, and adds nothing at all:
    // written before Import checked anything, and red until it did.
    [Fact]
    public void ABadDeliveryAddsNothing()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(path, """
            [
              { "Title": "Harbor Lights", "Artist": "The Ferrymen", "Seconds": 244 },
              { "Title": "Dead Air", "Artist": "Nobody At All", "Seconds": -198 }
            ]
            """);

        Rotation rotation = new Rotation();

        Assert.Throws<InvalidDataException>(() => rotation.Import(path));
        Assert.Equal(0, rotation.Count);
    }

    // TODO — Task 2: a fact that reads your migration, written before you fix it.
}
