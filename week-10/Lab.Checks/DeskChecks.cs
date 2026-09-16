// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-10/Lab.Checks
//
//  CHECK 1 is the came-forward check: everything the desk has done since
//  week 4, including every answer week 9's queries give. It is green
//  before you start and its job is to still be green at the end.
//
//  Checks 2 to 5 are tonight's: a cart shaped for a table, a migration, and
//  the rotation going into the database and coming back out of it.
//
//  ⚠️ ⚠️ NOT ONE OF THESE CHECKS TOUCHES THE COLLEGE'S SERVER. They hand
//  the context somewhere else to keep the rows — a table that lives in the
//  test run and nowhere else. So they need no password, no network and no
//  database, they answer in milliseconds, and they say nothing at all about
//  whether YOUR connection string is right. Running the desk is what tells
//  you that.
//
//  ⚠️ Nothing in here greps output. Every assert is on a value or an
//  object's state — which is why you can print whatever you like.
//
//  ⚠️ Every value is fetched through Ask() below, because two of tonight's
//  likely rewrites THROW where the loop simply answered. A stack trace is
//  not a hint.
// ═══════════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;

namespace Lab.Checks;

public class DeskChecks
{
    // A scratch file of this check's own, in the folder the operating system
    // keeps for exactly this. Deleted first, so a previous run cannot pass a
    // check for you.
    private static string Scratch(string name)
    {
        string path = Path.Combine(Path.GetTempPath(), $"kdxr-check-{name}");
        File.Delete(path);
        return path;
    }

    // Call one of tonight's methods and turn a crash into something you can
    // read. Both of the traps below are ordinary first attempts, not blunders.
    private static T Ask<T>(string what, Func<T> question)
    {
        try
        {
            return question();
        }
        catch (Exception e)
        {
            throw new Xunit.Sdk.XunitException(
                $"{what} threw {e.GetType().Name} instead of answering:\n"
                + $"    {e.Message}\n"
                + "⚠️ Two rewrites throw where the loop you replaced did not:\n"
                + "  • First(), Last() and Single() OBJECT to an empty sequence. The\n"
                + "    ...OrDefault versions hand back null instead — which is what your\n"
                + "    loop did when it walked a list and found nothing.\n"
                + "  • MaxBy and MinBy hand back NULL for an empty list, so asking their\n"
                + "    answer for a .Name or a .Seconds straight off dies right here.\n"
                + "    Put a ?. in front of it, and say with ?? what to answer instead.\n"
                + "👉 Next: run the program and try the same thing by hand — the desk with "
                + "an empty hour, or a switchboard nobody has rung.");
        }
    }

    // Somewhere to keep rows that is not the college's server: a table named
    // after this call and living only inside this test run. Every check below
    // that needs a database gets one of these.
    private static DbContextOptions<DeskContext> SomewhereElse(string name) =>
        new DbContextOptionsBuilder<DeskContext>()
            .UseInMemoryDatabase($"kdxr-check-{name}-{Guid.NewGuid()}")
            .Options;

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

    // ── Check 1 — everything the desk already did, and still has to ────────

    [Fact] // green before you start, and it has to STAY green
    public void Check1_TheDeskStillAnswers()
    {
        // ── the parts nothing this week touches ────────────────────────────

        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — that line ships finished and "
            + "nothing this week touches it. If this check is red, something in "
            + $"Broadcast.cs got changed by hand. (It returned: \"{Broadcast.CallSign()}\")");

        Assert.True(Broadcast.Clock(605) == "10:05",
            $"Broadcast.Clock(605) says \"{Broadcast.Clock(605)}\" and should say \"10:05\". "
            + "That is week 7's repair and it ships already made.");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. That door was shut in "
            + "week 4 and nothing this week goes near it.");

        var ad = new Ad("Pham's Bakery", "open at five", 1);
        ad.Play();
        ad.Play();
        Assert.True(ad.Remaining == 0,
            $"A one-run buy aired twice and Remaining is {ad.Remaining}. Week 7's guard, "
            + "and it ships already made.");

        var rotation = ThreeCarts();
        rotation.All().Clear();
        Assert.True(rotation.Count == 3,
            $"Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. All() hands back a COPY — week 4, and still the deal.\n"
            + "⚠️ Every query you write tonight hands back a new list for the same reason. "
            + "That is what the ToList() on the end of them is for.");

        // ⚠️ WEEK 8'S ROUND TRIP IS NOT IN HERE, and that is deliberate. It is
        // the one thing the desk did last week that tonight MOVES, so it is
        // checks 4 and 5 rather than a came-forward fact. The air log below
        // stays exactly where it was — a file, and still a file at the end of
        // the night.

        string noLog = Ask("Broadcast.LastShift", () => Broadcast.LastShift(Scratch("no-such.txt")));
        Assert.True(noLog == "",
            "Broadcast.LastShift() should hand back an empty string for a log that isn't "
            + $"there, and it gave \"{noLog}\".\n"
            + "⚠️ Task 1 rewrites this method. The File.Exists guard at the top STAYS — "
            + "File.ReadAllLines throws on a missing file, so LINQ never gets asked "
            + "anything at all.");

        // ── the seven Task 1 rewrites, asserted ───────────────────────────
        // Every one of these was green before you touched it. If one has gone
        // red, the rewrite changed the answer — which is exactly what this
        // check exists to tell you.

        var hour = new Hour();
        hour.Add(new StationId("KDXR 88.1, The Owl"));           // 12
        hour.Add(new Song("Nightjar", "The Lamplighters", 227)); // 227
        hour.Add(new Ad("Pham's Bakery", "open at five", 3));    // 30
        hour.Add(new WeatherBed("clear, four below"));           // 45

        int hourSeconds = Ask("Hour.TotalSeconds", () => hour.TotalSeconds);
        Assert.True(hourSeconds == 314,
            $"Hour.TotalSeconds says {hourSeconds} for an hour holding 12 + 227 + 30 + 45 "
            + "seconds, and it should say 314.\n"
            + "This was a running total, a foreach and a += — and it was RIGHT. If it went "
            + "red just now, the one-liner is adding up something other than Seconds:\n"
            + "    public int TotalSeconds => _items.Sum(item => item.Seconds);\n"
            + "👉 Next: read the lambda. The name before the => is each item in turn; what "
            + "comes after it is the thing to add up about that item.");

        int rotationSeconds = Ask("Rotation.TotalSeconds", () => ThreeCarts().TotalSeconds);
        Assert.True(rotationSeconds == 810,
            $"Rotation.TotalSeconds says {rotationSeconds} for 227 + 252 + 331 seconds, and "
            + "it should say 810. Same line as Hour's, different nouns.");

        var empty = new Hour();
        int emptySeconds = Ask("Hour.TotalSeconds on an empty hour", () => empty.TotalSeconds);
        Assert.True(emptySeconds == 0,
            $"An empty hour says {emptySeconds} seconds long and should say 0. Sum over "
            + "nothing is 0 — you do not have to guard for it.");

        IScheduleItem? longest = Ask("Hour.LongestItem", () => hour.LongestItem());
        Assert.True(longest != null && longest.Seconds == 227,
            "Hour.LongestItem() should hand back the 227-second song out of an hour holding "
            + $"12, 227, 30 and 45 — it gave "
            + $"{(longest == null ? "null" : longest.Seconds + " seconds")}.\n"
            + "    public IScheduleItem? LongestItem() => _items.MaxBy(item => item.Seconds);\n"
            + "👉 Next: MaxBy hands back the ITEM. Max would hand back the number, and then "
            + "you could not ask it for its Cue.");

        IScheduleItem? nothingLongest = Ask("Hour.LongestItem on an empty hour",
            () => empty.LongestItem());
        Assert.True(nothingLongest == null,
            "The longest thing in an empty hour is null, and LongestItem() handed back "
            + "something instead.\n"
            + "MaxBy on an empty list answers null rather than throwing — which is why this "
            + "method's return type has a ? on the end of it, and why OrderByDescending "
            + "followed by First() is the wrong shape here.");

        var board = new Switchboard();
        board.Take("Dorothy");
        board.Take("Bex");
        board.Take("Dorothy");

        int calls = Ask("Switchboard.TotalCalls", () => board.TotalCalls);
        Assert.True(calls == 3,
            $"Switchboard.TotalCalls says {calls} after three calls and should say 3. Third "
            + "Sum in the program, same line as the other two.");

        string regular = Ask("Switchboard.TheRegular", () => board.TheRegular());
        Assert.True(regular == "Dorothy",
            $"Switchboard.TheRegular() says \"{regular}\" after Dorothy rang twice and Bex "
            + "once. It should say \"Dorothy\".\n"
            + "This is the method you wrote in WEEK 3, over a dictionary. Same question:\n"
            + "    _callers.MaxBy(caller => caller.CallsTonight)?.Name ?? \"nobody yet\"");

        var quiet = new Switchboard();
        string nobody = Ask("Switchboard.TheRegular on a board nobody has rung",
            () => quiet.TheRegular());
        Assert.True(nobody == "nobody yet",
            $"Switchboard.TheRegular() says \"{nobody}\" on a board nobody has rung, and it "
            + "has to say \"nobody yet\" — exactly that, the same string it has said since "
            + "week 3.\n"
            + "⚠️ MaxBy on an empty list hands back NULL, so asking it for a .Name straight "
            + "off throws. Two operators do the whole job:\n"
            + "    _callers.MaxBy(caller => caller.CallsTonight)?.Name ?? \"nobody yet\"\n"
            + "  ?.  don't ask a nothing for its Name\n"
            + "  ??  and when there is nothing, say this instead\n"
            + "👉 Next: that is what `string best = \"nobody yet\";` was doing before the "
            + "loop, and it still has to be said somewhere.");

        Assert.True(board.Find("Dorothy") != null
                    && ReferenceEquals(board.Find("Dorothy"), board.All()[0]),
            "Switchboard.Find no longer hands back the caller the board is holding. Week 5, "
            + "and nothing this week touches Find.");

        // ── week 9's four answers, which are now things the desk simply does ──

        var asked = ThreeCarts();

        List<Song> longOnes = Ask("Rotation.LongerThan", () => asked.LongerThan(240));
        Assert.True(longOnes.Count == 2,
            $"Rotation.LongerThan(240) handed back {longOnes.Count} carts and should hand "
            + $"back 2 — Slack Water at 252 and Long Way Round at 331. It gave: "
            + $"{Titles(longOnes)}. Week 9, and nothing tonight goes near it.");

        asked.All()[0].Play();
        List<Song> unplayed = Ask("Rotation.NeverPlayed", () => asked.NeverPlayed());
        Assert.True(unplayed.Count == 2 && unplayed.All(s => s.PlaysTonight == 0),
            $"Rotation.NeverPlayed() handed back {Titles(unplayed)} after one cart aired "
            + "once. It should hand back the other two. Week 9.");

        var order = ThreeCarts();
        List<Song> before = order.All();
        order.All()[2].Play();
        order.All()[2].Play();
        order.All()[1].Play();

        List<Song> top = Ask("Rotation.TopPlayed", () => order.TopPlayed(2));
        Assert.True(top.Count == 2 && top[0].Title == "Long Way Round",
            $"Rotation.TopPlayed(2) handed back {Titles(top)}, hardest-worked first. It "
            + "should start with Long Way Round, which aired twice. Week 9.");

        Assert.True(order.All()[0].Title == before[0].Title
                    && order.All()[2].Title == before[2].Title,
            "⚠️ TopPlayed SORTED THE ROTATION. It is supposed to answer a question about "
            + "the rotation and leave it exactly as it found it — OrderByDescending sorts "
            + "a copy, List.Sort rearranges the list itself. Week 9, and it matters more "
            + "tonight than it did then: the order the carts come out of Load is the order "
            + "they went into the table.");

        var airtime = new Hour();
        var cart = new Song("Nightjar", "The Lamplighters", 227);
        airtime.Add(cart);
        List<string> running = Ask("Hour.RunningOrder", () => airtime.RunningOrder());
        Assert.True(running.Count == 1 && cart.PlaysTonight == 0,
            $"Hour.RunningOrder() put the hour ON THE AIR — Nightjar has aired "
            + $"{cart.PlaysTonight} times and should have aired 0. A query asks; Run() "
            + "does. Week 9.");
    }

    // ── Check 2 — Task 2: a cart the database can keep ────────────────────

    [Fact]
    public void Check2_ACartIsShapedForATable()
    {
        // Ask EF Core what it thinks this class is. Building the model reads
        // the class and the attributes on it; it opens no connection.
        using var db = new DeskContext(SomewhereElse("shape"));

        Microsoft.EntityFrameworkCore.Metadata.IEntityType? cart =
            Ask("the model EF Core builds for Song", () => db.Model.FindEntityType(typeof(Song)));

        Assert.True(cart != null,
            "EF Core does not know about Song at all.\n"
            + "⚠️ A class becomes a table by being named in the context. DeskContext needs:\n"
            + "    public DbSet<Song> Carts { get; set; }\n"
            + "👉 Next: open Lab/DeskContext.cs and look for that line.");

        var key = cart!.FindPrimaryKey();
        Assert.True(key != null && key.Properties.Count == 1
                    && key.Properties[0].Name == "Id",
            "Song has no Id for the database to tell its rows apart by"
            + (key == null ? "" : $" — the key it found was {string.Join(", ", key.Properties.Select(k => k.Name))}")
            + ".\n"
            + "⚠️ A table needs one column that is different on every row, and EF Core "
            + "looks for a property called Id. Nothing at the station has one; this is the "
            + "database's own name for the row.\n"
            + "    public int Id { get; set; }\n"
            + "👉 Next: add it at the top of Song.cs.");

        foreach (string worked in new[] { "Length", "Kind", "Cue" })
        {
            Assert.True(cart.FindProperty(worked) == null,
                $"Song.{worked} is being stored as a column, and it should not be.\n"
                + "⚠️ A table gets a column for everything the class can tell you, "
                + "including the things it WORKS OUT. Length is arithmetic on Seconds; "
                + "Kind is the same word on every row; Cue is Title and Artist stuck "
                + "together. Storing any of them means keeping the same fact twice and "
                + "letting the two drift apart.\n"
                + $"    [NotMapped]\n    public string {worked} => ...\n"
                + "👉 Next: put [NotMapped] on it, and add\n"
                + "    using System.ComponentModel.DataAnnotations.Schema;\n"
                + "   at the top of Song.cs if it is not there yet.");
        }

        // Seconds IS a fact the cart keeps, so it had better be a column.
        Assert.True(cart.FindProperty("Seconds") != null,
            "Song.Seconds is not a column, and it has to be — it is the one number the "
            + "desk cannot work out for itself. Check you have not put [NotMapped] on it "
            + "by mistake.");
    }

    // ── Check 3 — Task 3: the table actually got made ─────────────────────

    [Fact]
    public void Check3_ThereIsAMigration()
    {
        // A migration is a CLASS. `dotnet ef migrations add` writes it into
        // your project, so it is in the compiled program and can be found.
        Type[] migrations = typeof(Rotation).Assembly
            .GetTypes()
            .Where(type => typeof(Microsoft.EntityFrameworkCore.Migrations.Migration)
                               .IsAssignableFrom(type)
                           && !type.IsAbstract)
            .ToArray();

        Assert.True(migrations.Length > 0,
            "There is no migration in this project.\n"
            + "⚠️ A migration is the instructions for making the table. It is a C# class "
            + "that `dotnet ef` writes for you, by comparing what your context says to "
            + "what it made last time:\n"
            + "    dotnet ef migrations add TheCartsMoveIn --project week-10/Lab\n"
            + "👉 Next: run that, then look in Lab/Migrations/ and read the Up() method — "
            + "it is your Song class with the column types filled in.\n"
            + "⚠️ Making the migration does not make the table. `dotnet ef database update` "
            + "is the one that talks to the server, and this check cannot see whether you "
            + "ran it — running the desk is what tells you that.");
    }

    // ── Check 4 — Task 4: the carts go into the table ──────────────────────

    [Fact]
    public void Check4_TheCartsGoIntoTheTable()
    {
        var somewhere = SomewhereElse("save");
        var rotation = ThreeCarts();

        using (var db = new DeskContext(somewhere))
        {
            Ask("Rotation.Save", () => { rotation.Save(db); return 0; });
        }

        using (var db = new DeskContext(somewhere))
        {
            // db.Set<Song>() rather than db.Carts — the checks do not care
            // what you called the DbSet property.
            int rows = db.Set<Song>().Count();

            Assert.True(rows == 3,
                $"Three carts were saved and the table holds {rows} of them.\n"
                + (rows == 0
                    ? "⚠️ Nothing arrived. Add() only writes down what you INTEND to do — "
                      + "db.SaveChanges() is the line that actually does it, and every Save "
                      + "needs one at the end.\n"
                    : "⚠️ Too many. Every cart went in as a NEW row. A cart that came out "
                      + "of the table is carrying an Id, and that one is not new — it is a "
                      + "row that CHANGES.\n")
                + "👉 Next: read Rotation.Save. Each cart is one of two things, and its Id "
                + "says which: 0 means new (Add), anything else means the table already has "
                + "a row for it (Update).");
        }

        // Saving twice must not double anything up — the desk signs off every
        // night, and the file it replaced was rewritten whole every time.
        using (var db = new DeskContext(somewhere))
        {
            rotation.Save(db);
        }

        using (var db = new DeskContext(somewhere))
        {
            // db.Set<Song>() rather than db.Carts — the checks do not care
            // what you called the DbSet property.
            int rows = db.Set<Song>().Count();

            Assert.True(rows == 3,
                $"Saved the same three carts a second time and the table now holds {rows}.\n"
                + "⚠️ Every sign-off writes the whole rotation back, so saving twice must "
                + "not double anything up. A cart that is already on file is an UPDATE, not "
                + "an Add — and its Id is how you can tell.\n"
                + "👉 Next: if (cart.Id == 0) Add, else Update.");
        }
    }

    // ── Check 5 — Task 5: and they come back out ──────────────────────────

    [Fact]
    public void Check5_TheCartsComeBackOut()
    {
        var somewhere = SomewhereElse("load");
        var rotation = ThreeCarts();
        rotation.All()[0].Play();

        using (var db = new DeskContext(somewhere))
        {
            rotation.Save(db);
        }

        var reopened = new Rotation();

        using (var db = new DeskContext(somewhere))
        {
            Ask("Rotation.Load", () => { reopened.Load(db); return 0; });
        }

        Assert.True(reopened.Count == 3,
            $"Three carts went into the table and {reopened.Count} came back out.\n"
            + (reopened.Count == 0
                ? "⚠️ Nothing came back. db.Carts is the table; .ToList() is what asks it "
                  + "for everything in it.\n"
                : "")
            + "👉 Next: read Rotation.Load — Clear(), then AddRange over db.Carts.ToList().");

        Assert.True(reopened.All()[0].Title == "Nightjar",
            $"The first cart back is \"{reopened.All()[0].Title}\" and should be "
            + "\"Nightjar\". The carts have to come back in the order they went in — the "
            + "hour and the rotation both count on it.");

        int plays = reopened.All().Sum(s => s.PlaysTonight);
        Assert.True(plays == 1,
            $"A cart that aired once came back having aired {plays} times. The plays are "
            + "part of the cart, and they survive the night with it.");

        // ⚠️ THE FIRST NIGHT: an empty table must leave the rotation alone.
        var firstNight = ThreeCarts();

        using (var db = new DeskContext(SomewhereElse("first-night")))
        {
            Ask("Rotation.Load from an empty table", () => { firstNight.Load(db); return 0; });
        }

        Assert.True(firstNight.Count == 3,
            $"A rotation holding 3 carts loaded from an EMPTY table and now holds "
            + $"{firstNight.Count}.\n"
            + "⚠️ This is the first night, and it is the one case that crashes the desk. "
            + "Program.cs adds three carts and THEN calls Load. On a table nobody has saved "
            + "to yet, a Load that clears first throws those three away, and the next line "
            + "of Program.cs reaches for carts[0] and finds nothing.\n"
            + "⚠️ Week 8 had this too — it is what the File.Exists guard at the top of Load "
            + "was for. The table is always there; on the first night it is empty, which is "
            + "the same situation wearing different clothes.\n"
            + "👉 Next: read the rows into a list FIRST, and return early when there are "
            + "none of them.");

        // And loading twice does not stack them up.
        using (var db = new DeskContext(somewhere))
        {
            reopened.Load(db);
        }

        Assert.True(reopened.Count == 3,
            $"Loaded twice and the rotation holds {reopened.Count} carts.\n"
            + "⚠️ Loading is REPLACING. _songs.Clear() at the top of Load.");
    }
}
