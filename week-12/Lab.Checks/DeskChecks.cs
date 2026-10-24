// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-12/Lab.Checks
//
//  CHECK 1 is everything the desk already does, the database included. It
//  is green before you start, and its job is to still be green at the end.
//
//  Checks 2 to 4 are tonight's: a cart has many airings, the airings come
//  back with their cart, and the database answers a question.
//
//  ⚠️ NOT ONE OF THESE CHECKS TOUCHES THE COLLEGE'S SERVER. They hand your
//  DeskContext a table that lives inside the test run and nowhere else. So
//  they need no password and no network, and they say nothing about whether
//  YOUR connection string is right. Running the desk is what tells you that.
//
//  ⚠️ Nothing in here reads what the desk prints. Every assert is on a
//  value or on an object's state.
//
//  ⚠️ Your Airing class doesn't exist until Task 1 makes it, so these checks
//  find it by name rather than by writing it down. That's the only reason
//  some of what follows looks roundabout.
// ═══════════════════════════════════════════════════════════════════

using System.Collections;
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

    private static string Words(List<string> words) =>
        words.Count == 0 ? "(nothing)" : string.Join(", ", words);

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

    private static Type[] Migrations() =>
        typeof(Rotation).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && Derives(t, "Microsoft.EntityFrameworkCore.Migrations.Migration"))
            .ToArray();

    // ── tonight's class, found by name ──────────────────────────────────────

    private static Type? AiringType() =>
        typeof(Song).Assembly.GetTypes().FirstOrDefault(t => t.Name == "Airing");

    private static PropertyInfo? AiringsProperty() =>
        typeof(Song).GetProperty("Airings", BindingFlags.Public | BindingFlags.Instance);

    private static IList? AiringsOf(Song song) =>
        AiringsProperty()?.GetValue(song) as IList;

    private static int CountAirings(Song song) => AiringsOf(song)?.Count ?? -1;

    // True when Task 1 is far enough along for the later checks to use it.
    private static bool AiringsWork()
    {
        if (AiringType() == null || AiringsProperty() == null)
        {
            return false;
        }

        var song = new Song("Nightjar", "The Lamplighters", 227);
        song.Play();
        return CountAirings(song) == 1;
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
            $"Song.Seconds accepted -400 and is now {song.Seconds}. The rule in Seconds "
            + "ships finished, and nothing this week touches it.");

        song.Play();
        Assert.True(song.PlaysTonight == 1,
            $"A cart played once says PlaysTonight is {song.PlaysTonight}. Play() still "
            + "counts the play the way it always has — Task 1 ADDS a line to it, and takes "
            + "nothing out.");

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

        // ── week 11: a cart comes off ───────────────────────────────────────

        var pulling = ThreeCarts();
        Assert.True(Ask("Rotation.Remove(\"Slack Water\")", () => pulling.Remove("Slack Water"))
                    && Titles(pulling.All()) == "Nightjar, Long Way Round"
                    && !pulling.Remove("Slack Water"),
            "Rotation.Remove no longer takes one cart off and says so. It ships finished "
            + "from last week: Find(title), then _songs.Remove(found).");

        // ── weeks 10 and 11: the database ───────────────────────────────────

        using (DeskContext shape = Somewhere(Fresh("shape")))
        {
            IEntityType? cart = shape.Model.FindEntityType(typeof(Song));
            Assert.True(cart != null && cart.FindPrimaryKey()?.Properties[0].Name == "Id",
                "DeskContext has no Songs table with an Id any more. Week 10 gave it one:\n"
                + "    public DbSet<Song> Songs { get; set; }\n"
                + "and Song an Id. Both ship in tonight's starter — undo whatever changed.");
        }

        Assert.True(Migrations().Length > 0,
            "The Migrations folder is gone from Lab/. It ships in tonight's starter, and "
            + "`dotnet ef database update` needs it to make your tables. Copy week 12 in "
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
            + "Save and Load ship finished from weeks 10 and 11 — Task 2 changes one line of "
            + "Load, and nothing it changes should stop the three carts going in and coming back.");

        // The next night: one context, read at the start and written at the
        // end, the way the desk does it. One cart is pulled.
        using (DeskContext db = Somewhere(table))
        {
            var nextNight = new Rotation();
            Do("Rotation.Load", () => nextNight.Load(db));
            nextNight.All()[2].Play();
            nextNight.Remove("Slack Water");
            Do("Rotation.Save, the second night", () => nextNight.Save(db));
        }

        using (DeskContext db = Somewhere(table))
        {
            List<Song> rows = db.Songs.OrderBy(s => s.Id).ToList();
            Assert.True(rows.Count == 2 && Titles(rows) == "Nightjar, Long Way Round",
                $"Saved, loaded, one cart pulled and saved again, and the table holds "
                + $"{Titles(rows)}. It should hold Nightjar, Long Way Round.\n"
                + "Save's two loops ship finished from weeks 10 and 11: Add the new carts, "
                + "Update the rest, then take out every row no cart in the list has.");
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

    // ── Check 2 — Task 1: a cart has many airings ───────────────────────────

    [Fact]
    public void Check2_ACartHasManyAirings()
    {
        Type? airing = AiringType();

        Assert.True(airing != null,
            "There is no class called Airing in Lab/. One airing is one time a cart went "
            + "out on air, and each one gets a row of its own:\n"
            + "    public class Airing\n"
            + "    {\n"
            + "        public int Id { get; set; }\n"
            + "        public int SongId { get; set; }\n"
            + "        public DateTime At { get; private set; }\n"
            + "\n"
            + "        public Airing(DateTime at)\n"
            + "        {\n"
            + "            At = at;\n"
            + "        }\n"
            + "    }\n"
            + "👉 Next: make Lab/Airing.cs. Your instructor's Request.cs has the same shape.");

        PropertyInfo? list = AiringsProperty();

        Assert.True(list != null && list.PropertyType == typeof(List<>).MakeGenericType(airing!),
            "Song has no list of its airings. A cart has many airings, so Song holds a list "
            + "of them, the way your instructor's Caller holds a list of Requests:\n"
            + "    public List<Airing> Airings { get; private set; } = new List<Airing>();\n"
            + "👉 Next: Lab/Song.cs, under its // TODO — Task 1 comment.");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        Assert.True(CountAirings(song) == 0,
            "A cart that has never been on air should have no airings. Start the list "
            + "empty: = new List<Airing>();");

        DateTime before = DateTime.Now.AddSeconds(-1);
        song.Play();
        song.Play();

        Assert.True(CountAirings(song) == 2,
            $"A cart played twice has {CountAirings(song)} airings in its list, and should "
            + "have 2. Play() adds one every time the cart goes out on air:\n"
            + "    Airings.Add(new Airing(DateTime.Now));");

        object first = AiringsOf(song)![0]!;
        object? at = airing!.GetProperty("At")?.GetValue(first);
        Assert.True(at is DateTime when && when >= before && when <= DateTime.Now.AddSeconds(1),
            $"The airing Play() added says it went out at {at ?? "(no At)"}. It should say "
            + "now: new Airing(DateTime.Now).");

        Assert.True(song.LastAired() != "-",
            "A cart that has been on air still says \"-\" for LAST ON AIR. Task 1 gives "
            + "LastAired() a body that reads the newest airing.");

        using DeskContext db = Somewhere(Fresh("airings-shape"));

        IEntityType? table = db.Model.FindEntityType(airing!);
        Assert.True(table != null && table.FindPrimaryKey()?.Properties[0].Name == "Id",
            "Airing isn't a table in your DeskContext. Add one line beside the others:\n"
            + "    public DbSet<Airing> Airings { get; set; }");

        // ⚠️ Not GetTableName(): the table lives in this test run, and only a real
        // SQL Server names a table after its DbSet. So ask DeskContext directly.
        Assert.True(typeof(DeskContext).GetProperties()
                .Any(p => p.PropertyType == typeof(DbSet<>).MakeGenericType(airing!)),
            "DeskContext has no DbSet for your airings, so on the server the table would be "
            + "called Airing — every other table at the desk is named after its DbSet. Add "
            + "one line beside the others:\n"
            + "    public DbSet<Airing> Airings { get; set; }");

        IForeignKey? toSong = table.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Song));
        Assert.True(toSong != null && toSong.Properties.Count == 1 && toSong.Properties[0].Name == "SongId",
            "An airing has to say which cart it belongs to, and the column that says so is "
            + "SongId — the Id of that cart's row in Songs:\n"
            + "    public int SongId { get; set; }");

        Assert.True(Migrations().Any(MakesAirings),
            "No migration in Lab/Migrations knows about airings, so `dotnet ef database "
            + "update` has nothing that makes the table.\n"
            + "👉 Next: from the top of your repo,\n"
            + "    dotnet ef migrations add RequestsAndAirings --project week-12/Lab\n"
            + "then read the new file in Lab/Migrations before you update.");
    }

    // A migration knows about airings when the model it leaves behind has them.
    private static bool MakesAirings(Type migration)
    {
        object? made = Activator.CreateInstance(migration);
        var model = migration.GetProperty("TargetModel")?.GetValue(made) as IReadOnlyModel;
        return model?.FindEntityType(AiringType()!.FullName!) != null;
    }

    // ── Check 3 — Task 2: the airings come back with their cart ─────────────

    [Fact]
    public void Check3_TheAiringsComeBack()
    {
        Assert.True(AiringsWork(),
            "🚧 blocked — this check needs a cart that keeps its airings, and Song doesn't "
            + "have them yet.\n"
            + "👉 Next: check 2 first. It is Task 1.");

        string table = Fresh("airings-back");
        var rotation = ThreeCarts();
        rotation.All()[0].Play();
        rotation.All()[0].Play();
        rotation.All()[1].Play();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Save", () => rotation.Save(db));
        }

        var reopened = new Rotation();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Load", () => reopened.Load(db));
        }

        Song? nightjar = reopened.Find("Nightjar");
        Assert.True(nightjar != null && reopened.Count == 3,
            $"Three carts were saved and the next night loaded {Titles(reopened.All())}. "
            + "Check 1 covers Save and Load — make it green first.");

        int n = CountAirings(nightjar!);
        Assert.True(n != 0,
            "Nightjar went out on air twice and was saved, and the next night it has no "
            + "airings at all. The airings are in their table. Load never asked for them.\n"
            + "⚠️ Load reads the Songs table, and only the Songs table. EF Core brings "
            + "each cart's airings along only when Load asks it to:\n"
            + "    db.Songs.Include(song => song.Airings).OrderBy(song => song.Id).ToList()\n"
            + "Include comes from EF Core, so Rotation.cs needs its using line at the top:\n"
            + "    using Microsoft.EntityFrameworkCore;\n"
            + "👉 Next: Lab/Rotation.cs, the line in Load under its // TODO — Task 2 comment.");

        Assert.True(n == 2 && CountAirings(reopened.Find("Slack Water")!) == 1
                    && CountAirings(reopened.Find("Long Way Round")!) == 0,
            $"Nightjar was on air twice, Slack Water once and Long Way Round never, and the "
            + $"next night they have {n}, {CountAirings(reopened.Find("Slack Water")!)} and "
            + $"{CountAirings(reopened.Find("Long Way Round")!)} airings. Each cart should "
            + "come back with exactly its own.");
    }

    // ── Check 4 — Task 3: the database answers ──────────────────────────────

    [Fact]
    public void Check4_TheDatabaseAnswers()
    {
        Assert.True(AiringsWork(),
            "🚧 blocked — this check asks which carts have been on air, and Song doesn't "
            + "keep its airings yet.\n"
            + "👉 Next: check 2 first. It is Task 1.");

        string table = Fresh("aired");
        var rotation = ThreeCarts();
        rotation.All()[1].Play();
        rotation.All()[2].Play();

        using (DeskContext db = Somewhere(table))
        {
            Do("Rotation.Save", () => rotation.Save(db));
        }

        // A rotation holding NOTHING asks the question. The answer has to come
        // from the table, because there is nothing in its list to look at.
        var empty = new Rotation();
        List<string> aired;

        using (DeskContext db = Somewhere(table))
        {
            aired = Ask("Rotation.Aired", () => empty.Aired(db));
        }

        Assert.True(aired.Count != 0,
            "Slack Water and Long Way Round have been on air, and Aired handed back nothing.\n"
            + "⚠️ This check asks a rotation with NOTHING in its list. If Aired looks at "
            + "_songs, there is nothing there. The question goes to the database:\n"
            + "    return db.Songs\n"
            + "        .Where(song => song.Airings.Any())\n"
            + "        .OrderBy(song => song.Title)\n"
            + "        .Select(song => song.Title)\n"
            + "        .ToList();\n"
            + "👉 Next: Lab/Rotation.cs, under its // TODO — Task 3 comment.");

        Assert.True(!aired.Contains("Nightjar"),
            $"Aired handed back {Words(aired)}, and Nightjar has never been on air. Keep only "
            + "the carts that have at least one airing:\n"
            + "    .Where(song => song.Airings.Any())");

        Assert.True(aired.Count == 2,
            $"Aired handed back {Words(aired)}. It should hand back the two carts that have "
            + "been on air, once each: Long Way Round, Slack Water.");

        Assert.True(aired.SequenceEqual(new[] { "Long Way Round", "Slack Water" }),
            $"Aired handed back {Words(aired)}, and it should be in order by title: Long Way "
            + "Round, Slack Water.\n"
            + "    .OrderBy(song => song.Title)");
    }
}
