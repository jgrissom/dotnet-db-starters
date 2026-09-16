// ═══════════════════════════════════════════════════════════════════
//  ANSWER KEY — the student's own test file, week 10.
//
//  Three facts ship written in the starter (weeks 7 and 8). The fourth is
//  Task 1's, and the name is the student's to choose. This is one honest
//  set. Nothing anywhere asserts on these names.
//
//  ⭐ Task 1's fact is the one this week is about, and it is the FIRST
//  test in this course that is green before the work and green after it.
//  It was watched green against the shipped hand-written TheRegular, then
//  watched green again against the one-liner. That is the whole of week
//  7's promise: a suite is permission to rewrite.
//
//  ⚠️ Every path here is a scratch path. `dotnet test` runs from inside
//  Lab.Tests/bin/Debug/net10.0 and `dotnet run` runs from the top of the
//  repo, so a plain name would mean two different files.
// ═══════════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;

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

    // Week 8, and week 10 moved it. The fact has not changed a word: a cart
    // aired twice, saved and loaded, still says it aired twice. Where it is
    // kept changed.
    //
    // ⚠️ THIS TEST NEVER TOUCHES THE COLLEGE'S SERVER, and that is the point
    // of it. It hands the context somewhere else to put the rows — a table
    // that lives in this test run and nowhere else. So the suite needs no
    // database, no password and no network, and it answers in milliseconds.
    [Fact]
    public void ACartRemembersItsPlays()
    {
        // Somewhere of its own, named after this run, so two tests going at
        // the same time can never see each other's carts.
        DbContextOptions<DeskContext> somewhereElse =
            new DbContextOptionsBuilder<DeskContext>()
                .UseInMemoryDatabase("kdxr-" + Guid.NewGuid())
                .Options;

        Song nightjar = new Song("Nightjar", "The Lamplighters", 227);
        nightjar.Play();
        nightjar.Play();

        Rotation rotation = new Rotation();
        rotation.Add(nightjar);

        using (DeskContext db = new DeskContext(somewhereElse))
        {
            rotation.Save(db);
        }

        // A second rotation, holding nothing, and a second context — reading
        // the same carts. Loading into the one that just saved would prove
        // nothing.
        Rotation reopened = new Rotation();

        using (DeskContext db = new DeskContext(somewhereElse))
        {
            reopened.Load(db);
        }

        Assert.Equal(1, reopened.Count);
        Assert.Equal(2, reopened.All()[0].PlaysTonight);
    }

    // Week 9, Task 1. Written BEFORE the seven loops came out, and it is why
    // they were allowed to.
    //
    // Two situations, because the second one is the one a rewrite breaks: a
    // board nobody has rung has no busiest caller, and the answer to that has
    // been the string "nobody yet" since week 3.
    [Fact]
    public void TheRegularIsWhoeverRangMost()
    {
        Switchboard quiet = new Switchboard();

        Assert.Equal("nobody yet", quiet.TheRegular());
        Assert.Equal(0, quiet.TotalCalls);

        Switchboard board = new Switchboard();
        board.Take("Dorothy");
        board.Take("Bex");
        board.Take("Dorothy");

        Assert.Equal("Dorothy", board.TheRegular());
        Assert.Equal(3, board.TotalCalls);
    }
}
