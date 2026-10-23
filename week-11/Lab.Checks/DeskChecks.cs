// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-11/Lab.Checks
//
//  CHECK 1 is everything the desk already does, the database included. It
//  is green before you start, and its job is to still be green at the end.
//
//  Checks 2 and 3 are tonight's: a cart can be pulled off the rotation, and
//  a pulled cart's row comes out of the table, so it stays pulled.
//
//  ⚠️ NOT ONE OF THESE CHECKS TOUCHES THE COLLEGE'S SERVER. They hand your
//  DeskContext a table that lives inside the test run and nowhere else. So
//  they need no password and no network, and they say nothing about whether
//  YOUR connection string is right. Running the desk is what tells you that.
//
//  ⚠️ Nothing in here reads what the desk prints. Every assert is on a
//  value or on an object's state.
// ═══════════════════════════════════════════════════════════════════

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lab.Checks;

public class DeskChecks
{
    // Call something and turn a crash into something you can read.
    private static T Ask<T>(string what, Func<T> question)
    {
        try
        {
            return question();
        }
        catch (Exception e)
        {
            Exception inner = e is TargetInvocationException && e.InnerException != null ? e.InnerException : e;
            throw new Xunit.Sdk.XunitException(
                $"{what} threw {inner.GetType().Name} instead of answering:\n"
                + $"    {inner.Message}\n"
                + "⚠️ First(), Last() and Single() OBJECT when there is nothing to hand "
                + "back. The ...OrDefault versions hand back null instead.\n"
                + "👉 Next: run the desk and try the same thing by hand.");
        }
    }

    private static void Do(string what, Action action) =>
        Ask(what, () => { action(); return 0; });

    // Three carts, the same three as the desk, in a rotation of their own.
    private static Rotation ThreeCarts()
    {
        var rotation = new Rotation();
        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));
        rotation.Add(new Song("Slack Water", "Marguerite Vance", 252));
        rotation.Add(new Song("Long Way Round", "The Ferrymen", 331));
        return rotation;
    }

    private static string Titles(List<Song> songs) =>
        songs.Count == 0 ? "(nothing)" : string.Join(", ", songs.Select(s => s.Title));

    // A context pointed at a table that lives in this test run and nowhere else.
    private static DeskContext Somewhere(string name) =>
        new DeskContext(new DbContextOptionsBuilder<DeskContext>().UseInMemoryDatabase(name).Options);

    private static string Fresh(string what) => $"kdxr-check-{what}-{Guid.NewGuid()}";

    private static bool Derives(Type type, string fullName)
    {
        for (Type? t = type.BaseType; t != null; t = t.BaseType)
        {
            if (t.FullName == fullName)
            {
                return true;
            }
        }

        return false;
    }

    // ── Check 1 — everything the desk already does, and still has to ───────

    [Fact] // green before you start, and it has to STAY green
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — that line ships finished and "
            + "nothing this week touches it. If this check is red, something in "
            + $"Broadcast.cs got changed by hand. (It returned: \"{Broadcast.CallSign()}\")");

        Assert.True(Broadcast.Clock(605) == "10:05",
            $"Broadcast.Clock(605) says \"{Broadcast.Clock(605)}\" and should say \"10:05\". "
            + "That method ships finished — undo whatever changed in it.");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. Song.cs ships finished "
            + "and nothing this week touches it.");

        var rotation = ThreeCarts();
        rotation.All().Clear();
        Assert.True(rotation.Count == 3,
            "Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. All() hands back a copy, and it ships finished.");

        Song? found = Ask("Rotation.Find(\"Slack Water\")", () => rotation.Find("Slack Water"));
        Assert.True(found != null && ReferenceEquals(found, rotation.All()[1]),
            "Rotation.Find(\"Slack Water\") did not hand back the Slack Water the rotation "
            + $"is holding. It handed back: {(found == null ? "nothing" : found.Title)}. "
            + "Find ships finished — undo whatever changed in it.");

        Assert.True(Ask("Rotation.Find(\"No Such Cart\")", () => rotation.Find("No Such Cart")) == null,
            "Rotation.Find was asked for a title no cart has, and it handed something back "
            + "instead of nothing. Find ships finished — undo whatever changed in it.");

        // ── the switchboard and the hour ────────────────────────────────────

        var board = new Switchboard();
        var dorothy = new Caller("Dorothy");
        board.Add(dorothy);
        Assert.True(ReferenceEquals(board.Take("Dorothy"), dorothy),
            "Switchboard.Take handed back a new Caller instead of the Dorothy on the board. "
            + "Switchboard.cs is your instructor's file — copy it in again from the "
            + "starters repo rather than editing it.");

        var hour = new Hour();
        hour.Add(new Ad("Pham's Bakery", "open at five", 3));
        hour.Add(new Song("Nightjar", "The Lamplighters", 227));
        Assert.True(hour.TotalSeconds == 257,
            $"An hour holding a 30-second spot and a 227-second song adds up to "
            + $"{hour.TotalSeconds} and should add up to 257. Hour.cs ships finished.");

        // ── week 9's questions ──────────────────────────────────────────────

        var asked = ThreeCarts();
        List<Song> longOnes = Ask("Rotation.LongerThan(240)", () => asked.LongerThan(240));
        Assert.True(longOnes.Select(s => s.Title).SequenceEqual(new[] { "Slack Water", "Long Way Round" }),
            $"Rotation.LongerThan(240) gave {Titles(longOnes)} and should give Slack Water, "
            + "Long Way Round. Nothing tonight goes near it.");

        List<Song> sorted = Ask("Rotation.ByTitle()", () => asked.ByTitle());
        Assert.True(sorted.Select(s => s.Title).SequenceEqual(new[] { "Long Way Round", "Nightjar", "Slack Water" })
                    && asked.All()[0].Title == "Nightjar",
            $"Rotation.ByTitle() gave {Titles(sorted)} and should give Long Way Round, "
            + "Nightjar, Slack Water, leaving the rotation itself alone.");

        // ── week 10: the database ───────────────────────────────────────────

        using (DeskContext shape = Somewhere(Fresh("shape")))
        {
            IEntityType? cart = shape.Model.FindEntityType(typeof(Song));
            Assert.True(cart != null && cart.FindPrimaryKey()?.Properties[0].Name == "Id",
                "DeskContext has no Songs table with an Id any more. Week 10 gave it one:\n"
                + "    public DbSet<Song> Songs { get; set; }\n"
                + "and Song an Id. Both ship in tonight's starter — undo whatever changed.");
        }

        Assert.True(typeof(Rotation).Assembly.GetTypes()
                .Any(t => !t.IsAbstract && Derives(t, "Microsoft.EntityFrameworkCore.Migrations.Migration")),
            "The Migrations folder is gone from Lab/. It ships in tonight's starter, and "
            + "`dotnet ef database update` needs it to make your tables. Copy week 11 in "
            + "again, or copy Lab/Migrations/ back from the starters clone.");

        string table = Fresh("restart");
        var saved = ThreeCarts();
        saved.All()[0].Play();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Save", () => saved.Save(db));
        }

        var reopened = new Rotation();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Load", () => reopened.Load(db));
        }

        Assert.True(reopened.Count == 3
                    && reopened.All().Select(s => s.Title).SequenceEqual(new[] { "Nightjar", "Slack Water", "Long Way Round" })
                    && reopened.All()[0].PlaysTonight == 1,
            $"Three carts were saved and the rotation that loaded them holds {Titles(reopened.All())}. "
            + "Save and Load ship finished from week 10 — Task 3 adds to Save, and nothing "
            + "it adds should stop the three carts going in and coming back.\n"
            + (reopened.Count > 3
                ? "⚠️ More carts came back than went in, so Save put some in twice. Task 3's "
                  + "loop only takes rows OUT. It never adds any.\n"
                : ""));

        var nextNight = new Rotation();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Load", () => nextNight.Load(db));
            nextNight.All()[2].Play();
            Do("Rotation.Save, the second night", () => nextNight.Save(db));
        }

        using (DeskContext db = Somewhere(table))
        {
            int rows = db.Songs.Count();
            Assert.True(rows == 3,
                $"Saved, loaded and saved again, and the table holds {rows} carts. It should "
                + "still hold 3.\n"
                + (rows == 0
                    ? "⚠️ The table is EMPTY, and nothing was pulled. Save took out the rows the "
                      + "list does hold. Look at Task 3's loop: the ! in front of _songs.Any(...) "
                      + "is what makes it take out only the rows the list doesn't hold.\n"
                    : rows > 3
                        ? "⚠️ More rows than carts, so Save put some in twice. Task 3's loop only "
                          + "takes rows OUT. It never adds any.\n"
                        : "A cart that came out of the table gets Update, not Add — week 10's "
                          + "if/else, which ships finished.\n"));
        }

        var firstNight = ThreeCarts();

        using (DeskContext db = Somewhere(Fresh("first-night")))
        {
            Do("Rotation.Load on an empty table", () => firstNight.Load(db));
        }

        Assert.True(firstNight.Count == 3,
            $"A rotation holding 3 carts loaded from an EMPTY table and now holds "
            + $"{firstNight.Count}. On the first night Load has to return before the "
            + "Clear() — week 10's if (loaded.Count == 0), which ships finished.");
    }

    // ── Check 2 — Task 2: a cart can be pulled ──────────────────────────────

    [Fact]
    public void Check2_ACartCanBePulled()
    {
        var rotation = ThreeCarts();
        Song longWayRound = rotation.All()[2];

        bool pulled = Ask("Rotation.Remove(\"Slack Water\")", () => rotation.Remove("Slack Water"));

        Assert.True(pulled,
            "Rotation.Remove(\"Slack Water\") said false, and Slack Water is in the rotation.\n"
            + "Remove hands back true when it took a cart off, and false when there was no "
            + "cart by that title. Your instructor's Switchboard.Remove has the shape:\n"
            + "    Song? found = Find(title);\n"
            + "\n"
            + "    if (found == null)\n"
            + "    {\n"
            + "        return false;\n"
            + "    }\n"
            + "\n"
            + "    _songs.Remove(found);\n"
            + "    return true;\n"
            + "👉 Next: Lab/Rotation.cs, under its // TODO — Task 2 comment.");

        Assert.True(rotation.Count < 3,
            $"Remove said true, and the rotation still holds {Titles(rotation.All())}. "
            + "Saying it took the cart off is not the same as taking it off: "
            + "_songs.Remove(found) is the line that does it.");

        Assert.True(rotation.Find("Slack Water") == null,
            $"Remove was asked for Slack Water, and the rotation now holds {Titles(rotation.All())}. "
            + "A cart came off, and it was the wrong one. Take off the cart Find handed back:\n"
            + "    _songs.Remove(found);");

        Assert.True(Titles(rotation.All()) == "Nightjar, Long Way Round"
                    && ReferenceEquals(rotation.All()[1], longWayRound),
            $"Slack Water came off and the rotation now holds {Titles(rotation.All())}. It "
            + "should hold Nightjar, Long Way Round: the same two carts, in the same order. "
            + "Take off the one cart Find handed back, and nothing else.");

        bool again = Ask("Rotation.Remove(\"Slack Water\"), a second time",
            () => rotation.Remove("Slack Water"));

        Assert.True(!again && rotation.Count == 2,
            "Rotation.Remove was asked for a cart that had already been pulled, and it "
            + $"said {again.ToString().ToLower()} with {rotation.Count} carts left. A title "
            + "that isn't in the rotation is an answer: false, and nothing changes.");
    }

    // ── Check 3 — Task 3: a pulled cart stays pulled ────────────────────────

    [Fact]
    public void Check3_APulledCartStaysPulled()
    {
        var probe = ThreeCarts();
        bool removeWorks = Ask("Rotation.Remove", () => probe.Remove("Slack Water"))
                           && probe.Count == 2 && probe.Find("Slack Water") == null;

        Assert.True(removeWorks,
            "🚧 blocked — this check pulls a cart with Rotation.Remove, and Remove doesn't "
            + "take one off yet.\n"
            + "👉 Next: check 2 first. It is Task 2.");

        string table = Fresh("pulled");

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Save", () => ThreeCarts().Save(db));
        }

        int[] idsBefore;

        using (DeskContext db = Somewhere(table))
        {
            idsBefore = db.Songs.OrderBy(s => s.Id).Select(s => s.Id).ToArray();
        }

        // The next shift: one context, read at the start and written at the
        // end, the same way the desk does it.
        var nextNight = new Rotation();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Load", () => nextNight.Load(db));
            nextNight.Remove("Slack Water");
            Do("Rotation.Save, after a cart was pulled", () => nextNight.Save(db));
        }

        List<Song> rows;

        using (DeskContext db = Somewhere(table))
        {
            rows = db.Songs.OrderBy(s => s.Id).ToList();
        }

        Assert.True(rows.Count <= 3,
            $"Three carts were saved, one was pulled, and the table now holds {rows.Count} rows: "
            + $"{Titles(rows)}.\n"
            + "⚠️ Save put carts in a second time. Task 3's loop only takes rows OUT — it "
            + "never adds any. Emptying the table and adding the list back is not the same "
            + "thing, and it gives every cart a new row.");

        Assert.True(!(rows.Count == 1 && rows[0].Title == "Slack Water"),
            "Slack Water was pulled, and it is the ONLY row left in the table. The two carts "
            + "that were kept are gone.\n"
            + "⚠️ Save took out the rows the list DOES hold, and kept the one it doesn't. "
            + "The ! in front of _songs.Any(...) turns the question round:\n"
            + "    if (!_songs.Any(song => song.Id == row.Id))");

        Assert.True(rows.All(s => s.Title != "Slack Water"),
            $"Slack Water was pulled and the rotation saved, and the table still has its "
            + $"row. The table holds {Titles(rows)}.\n"
            + "⚠️ Save adds and updates the carts in the list. A pulled cart isn't in the "
            + "list any more, so nothing in Save ever mentions it, and its row stays. "
            + "That is why it comes back the next night.\n"
            + "👉 Next: after the loop, and before SaveChanges, read the table and take "
            + "out every row whose Id no cart in the list has:\n"
            + "    foreach (Song row in db.Songs.ToList())\n"
            + "    {\n"
            + "        if (!_songs.Any(song => song.Id == row.Id))\n"
            + "        {\n"
            + "            db.Songs.Remove(row);\n"
            + "        }\n"
            + "    }");

        Assert.True(rows.Count == 2 && Titles(rows) == "Nightjar, Long Way Round",
            $"Slack Water was pulled and the table now holds {Titles(rows)}. It should hold "
            + "exactly the two carts that weren't pulled: Nightjar, Long Way Round.\n"
            + "⚠️ Take out only the rows no cart in the list has. Every other row stays.");

        Assert.True(rows[0].Id == idsBefore[0] && rows[1].Id == idsBefore[2],
            $"The two carts that weren't pulled have new Ids: {rows[0].Id} and {rows[1].Id}, "
            + $"where they had {idsBefore[0]} and {idsBefore[2]}.\n"
            + "⚠️ Emptying the table and putting every cart back in looks the same on the "
            + "desk, and it gives every cart a new row. Take out the one row that isn't "
            + "in the list, and leave the others alone.");

        var reopened = new Rotation();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Load", () => reopened.Load(db));
        }

        Assert.True(reopened.Count == 2 && reopened.Find("Slack Water") == null,
            $"The next night loaded {Titles(reopened.All())}. Slack Water was pulled, so "
            + "it should not be there.");
    }
}
