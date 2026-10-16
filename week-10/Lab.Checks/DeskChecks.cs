// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-10/Lab.Checks
//
//  CHECK 1 is everything the desk already does. It is green before you
//  start, and its job is to still be green at the end.
//
//  Checks 2 to 5 are tonight's: a cart shaped for a table, a migration,
//  the carts going into the table and coming back out, and a second save
//  that changes the rows it already has.
//
//  ⚠️ NOT ONE OF THESE CHECKS TOUCHES THE COLLEGE'S SERVER. They hand your
//  DeskContext a table that lives inside the test run and nowhere else. So
//  they need no password and no network, and they say nothing about whether
//  YOUR connection string is right. Running the desk is what tells you that.
//
//  ⚠️ They find DeskContext, Save and Load BY NAME, because none of them
//  exists in the starter — that is what lets this project build before you
//  have written them. It is also why the names are dictated.
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

    // ── finding tonight's code, by name ─────────────────────────────────────

    private const string ContextHint =
        "    public class DeskContext : DbContext\n"
        + "    {\n"
        + "        public DbSet<Caller> Callers { get; set; }\n"
        + "        public DbSet<Song> Carts { get; set; }\n"
        + "        public DeskContext() { }\n"
        + "        public DeskContext(DbContextOptions<DeskContext> options) : base(options) { }\n"
        + "        protected override void OnConfiguring(DbContextOptionsBuilder options) { ... }\n"
        + "    }\n";

    private static Type Context()
    {
        Type? type = typeof(Rotation).Assembly.GetType("DeskContext");

        Assert.True(type != null,
            "There is no class called DeskContext in your Lab project yet.\n"
            + "⚠️ That is Task 2: a new file, Lab/DeskContext.cs, holding the desk's "
            + "database. The name is dictated, because your instructor's Switchboard.cs "
            + "and these checks both reach it by name:\n"
            + ContextHint
            + "👉 Next: make Lab/DeskContext.cs. Task 2 in the lab README has the syntax.");

        Assert.True(typeof(DbContext).IsAssignableFrom(type),
            "DeskContext exists, but it isn't a DbContext. The class line needs the "
            + "colon and the base class:\n"
            + "    public class DeskContext : DbContext");

        return type!;
    }

    // A context pointed at a table that lives in this test run and nowhere else.
    private static DbContext Somewhere(string name)
    {
        Type type = Context();

        ConstructorInfo? withOptions = type.GetConstructors()
            .FirstOrDefault(c => c.GetParameters().Length == 1
                && c.GetParameters()[0].ParameterType == typeof(DbContextOptions<>).MakeGenericType(type));

        Assert.True(withOptions != null,
            "DeskContext has no constructor that takes DbContextOptions<DeskContext>, so "
            + "nothing can hand it a table to use for a test — including these checks.\n"
            + "    public DeskContext(DbContextOptions<DeskContext> options)\n"
            + "        : base(options)\n"
            + "    {\n"
            + "    }\n"
            + "⚠️ Keep the empty constructor as well. `dotnet ef` and the desk use that one.");

        var builder = (DbContextOptionsBuilder)Activator.CreateInstance(
            typeof(DbContextOptionsBuilder<>).MakeGenericType(type))!;
        builder.UseInMemoryDatabase(name);

        return (DbContext)withOptions!.Invoke(new object[] { builder.Options });
    }

    private static string Fresh(string what) => $"kdxr-check-{what}-{Guid.NewGuid()}";

    private static MethodInfo RotationMethod(string name)
    {
        Type type = Context();
        MethodInfo? method = typeof(Rotation).GetMethod(name, new[] { type });

        MethodInfo? old = typeof(Rotation).GetMethod(name, new[] { typeof(string) });

        Assert.True(method != null,
            $"Rotation has no {name}(DeskContext db) yet"
            + (old != null ? $" — it still has week 9's {name}(string path)" : "")
            + ".\n"
            + "⚠️ That is Task 4: Save and Load take the desk's database instead of a "
            + "file path. The parameter is the whole change:\n"
            + "    public void Save(DeskContext db)\n"
            + "    public void Load(DeskContext db)\n"
            + "👉 Next: Lab/Rotation.cs. Your instructor's Switchboard.cs has the same two.");

        return method!;
    }

    private static void Save(Rotation rotation, DbContext db) =>
        Ask("Rotation.Save", () => RotationMethod("Save").Invoke(rotation, new object[] { db }));

    private static void Load(Rotation rotation, DbContext db) =>
        Ask("Rotation.Load", () => RotationMethod("Load").Invoke(rotation, new object[] { db }));

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

        // ── Find, which week 9 rewrote ─────────────────────────────────────

        Song? found = Ask("Rotation.Find(\"Slack Water\")", () => rotation.Find("Slack Water"));

        Assert.True(found != null && ReferenceEquals(found, rotation.All()[1]),
            "Rotation.Find(\"Slack Water\") did not hand back the Slack Water the rotation "
            + $"is holding. It handed back: {(found == null ? "nothing" : found.Title)}.\n"
            + "Find hands back the cart ITSELF — the same object, never a copy — for the "
            + "title it was asked for:\n"
            + "    return _songs.FirstOrDefault(song => song.Title == title);\n"
            + "👉 Next: read the question inside the brackets. It compares the song's Title "
            + "with the title Find was handed.");

        Song? missing = Ask("Rotation.Find(\"No Such Cart\")", () => rotation.Find("No Such Cart"));

        Assert.True(missing == null,
            "Rotation.Find was asked for a title no cart has, and it handed back "
            + $"\"{missing?.Title}\" instead of nothing. A title that is not in the "
            + "rotation gets null.");

        var empty = new Rotation();
        Song? fromEmpty = Ask("Rotation.Find on an empty rotation", () => empty.Find("Nightjar"));

        Assert.True(fromEmpty == null,
            "Rotation.Find on an empty rotation should hand back nothing.");

        // ── the switchboard and the hour ────────────────────────────────────

        var board = new Switchboard();
        var dorothy = new Caller("Dorothy");
        board.Add(dorothy);
        Assert.True(ReferenceEquals(board.Take("Dorothy"), dorothy),
            "Switchboard.Take handed back a new Caller instead of the Dorothy on the board. "
            + "Switchboard.cs is your instructor's file — copy it in again from the "
            + "starters repo rather than editing it.");

        var hour = new Hour();
        var spot = new Ad("Pham's Bakery", "open at five", 3);
        hour.Add(spot);
        hour.Add(new Song("Nightjar", "The Lamplighters", 227));

        Assert.True(hour.TotalSeconds == 257,
            $"An hour holding a 30-second spot and a 227-second song adds up to "
            + $"{hour.TotalSeconds} and should add up to 257. Hour.cs is your instructor's "
            + "file — copy it in again from the starters repo rather than editing it.");

        Assert.True(hour.Run()[0].Contains("(2 left)"),
            "The hour printed the buy as it stood BEFORE the spot aired. Hour.cs is your "
            + "instructor's file — copy it in again from the starters repo.");

        // ── week 9's four questions, which are now things the desk does ────

        var asked = ThreeCarts();

        List<Song> longOnes = Ask("Rotation.LongerThan(240)", () => asked.LongerThan(240));
        Assert.True(longOnes.Select(s => s.Title).SequenceEqual(new[] { "Slack Water", "Long Way Round" }),
            $"Rotation.LongerThan(240) gave {Titles(longOnes)} and should give Slack Water, "
            + "Long Way Round. That is week 9's method and nothing tonight goes near it.");

        List<string> titles = Ask("Rotation.Titles()", () => asked.Titles());
        Assert.True(titles.SequenceEqual(new[] { "Nightjar", "Slack Water", "Long Way Round" }),
            $"Rotation.Titles() gave {string.Join(", ", titles)} and should give Nightjar, "
            + "Slack Water, Long Way Round. Week 9's method, and nothing tonight touches it.");

        List<Song> sorted = Ask("Rotation.ByTitle()", () => asked.ByTitle());
        Assert.True(sorted.Select(s => s.Title).SequenceEqual(new[] { "Long Way Round", "Nightjar", "Slack Water" })
                    && asked.All()[0].Title == "Nightjar",
            $"Rotation.ByTitle() gave {Titles(sorted)}, and afterwards the rotation itself "
            + $"starts with {asked.All()[0].Title}. It should give Long Way Round, Nightjar, "
            + "Slack Water, and leave the rotation starting with Nightjar. Week 9's method.");

        List<Song> water = Ask("Rotation.Matching(\"Water\")", () => asked.Matching("Water"));
        Assert.True(water.Count == 1 && water[0].Title == "Slack Water",
            $"Rotation.Matching(\"Water\") gave {Titles(water)} and should give Slack Water. "
            + "Week 9's method, and nothing tonight goes near it.");
    }


    // ── Check 2 — Task 2: a cart the database can keep ──────────────────────

    [Fact]
    public void Check2_ACartIsShapedForATable()
    {
        using DbContext db = Somewhere(Fresh("shape"));

        // Building the model reads the classes; it opens no connection.
        IModel model;
        try
        {
            model = db.Model;
        }
        catch (Exception e)
        {
            string message = e.Message;
            throw new Xunit.Sdk.XunitException(
                "EF Core could not work out a table for one of your classes:\n"
                + $"    {message}\n"
                + (message.Contains("'Caller'") || message.Contains("Song' requires")
                    ? "⚠️ If that names Caller, or names Song while you have not given Song an "
                      + "Id yet, copy your instructor's Caller.cs in first — Task 2 starts with "
                      + "it. The Caller in the starter cannot be kept in a table.\n"
                    : "")
                + "👉 Next: if it says 'requires a primary key', the class it names needs\n"
                + "    public int Id { get; set; }");
        }

        IEntityType? cart = model.FindEntityType(typeof(Song));

        Assert.True(cart != null,
            "DeskContext does not have a table for Song.\n"
            + "A class becomes a table by having a DbSet in the context:\n"
            + "    public DbSet<Song> Carts { get; set; }\n"
            + "👉 Next: add that line to Lab/DeskContext.cs, under the Callers one.");

        IKey? key = cart!.FindPrimaryKey();
        Assert.True(key != null && key.Properties.Count == 1 && key.Properties[0].Name == "Id"
                    && key.Properties[0].ClrType == typeof(int),
            "Song has no Id for the database to tell its rows apart by.\n"
            + "    public int Id { get; set; }\n"
            + "👉 Next: add it at the top of the Song class in Lab/Song.cs.");

        foreach (string kept in new[] { "Title", "Artist", "Seconds", "PlaysTonight" })
        {
            Assert.True(cart.FindProperty(kept) != null,
                $"Song.{kept} is not a column in the Carts table, and it has to be: it is a "
                + "fact the cart keeps. Check you have not put [NotMapped] on it.");
        }

        Assert.True(model.FindEntityType(typeof(Caller)) != null,
            "DeskContext has no table for the callers. Your instructor's Switchboard.cs "
            + "saves to db.Callers, so the context needs both:\n"
            + "    public DbSet<Caller> Callers { get; set; }\n"
            + "    public DbSet<Song> Carts { get; set; }");

        Assert.True(Context().GetConstructor(Type.EmptyTypes) != null,
            "DeskContext has no constructor that takes nothing. `dotnet ef` and the desk "
            + "both make a DeskContext that way, so it has to be there:\n"
            + "    public DeskContext()\n"
            + "    {\n"
            + "    }");
    }

    // ── Check 3 — Task 3: the instructions that made the tables ─────────────

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

    [Fact]
    public void Check3_ThereIsAMigration()
    {
        Type[] types = typeof(Rotation).Assembly.GetTypes();

        Type[] migrations = types
            .Where(t => !t.IsAbstract && Derives(t, "Microsoft.EntityFrameworkCore.Migrations.Migration"))
            .ToArray();

        Assert.True(migrations.Length > 0,
            "There is no migration in your Lab project.\n"
            + "A migration is the instructions for making the tables. `dotnet ef` writes it "
            + "for you, from your DeskContext:\n"
            + "    dotnet ef migrations add TheDeskMovesIn --project week-10/Lab\n"
            + "👉 Next: run that, then open Lab/Migrations/ and read Up().\n"
            + "⚠️ Writing the migration does not make the tables. `dotnet ef database update` "
            + "is the command that talks to the server, and this check cannot see whether you "
            + "ran it. Running the desk is what tells you that.");

        Type? snapshot = types.FirstOrDefault(t => Derives(t, "Microsoft.EntityFrameworkCore.Infrastructure.ModelSnapshot"));
        IModel? model = snapshot == null
            ? null
            : snapshot.GetProperty("Model")!.GetValue(Activator.CreateInstance(snapshot, true)) as IModel;

        Assert.True(model != null && model.FindEntityType("Song") != null,
            "Your migration makes no Carts table. It was written before DeskContext had a "
            + "DbSet<Song> in it, so it only knows about the callers.\n"
            + "👉 Next: one more migration picks up what changed since the last one:\n"
            + "    dotnet ef migrations add TheCartsMoveIn --project week-10/Lab\n"
            + "   then `dotnet ef database update --project week-10/Lab`.");
    }

    // ── Check 4 — Task 4: the carts go into the table and come back out ─────

    [Fact]
    public void Check4_TheCartsSurviveARestart()
    {
        string table = Fresh("restart");

        var rotation = ThreeCarts();
        rotation.All()[0].Play();

        using (DbContext db = Somewhere(table))
        {
            Save(rotation, db);
        }

        using (DbContext db = Somewhere(table))
        {
            int rows = db.Set<Song>().Count();

            Assert.True(rows == 3,
                $"Three carts were saved and the table holds {rows}.\n"
                + (rows == 0
                    ? "⚠️ Nothing arrived. Add only writes down what you INTEND. "
                      + "db.SaveChanges() is the line that sends it, and Save needs it at the "
                      + "end, after the loop.\n"
                    : "")
                + "    foreach (Song song in _songs)\n"
                + "    {\n"
                + "        db.Carts.Add(song);\n"
                + "    }\n"
                + "\n"
                + "    db.SaveChanges();");
        }

        var reopened = new Rotation();

        using (DbContext db = Somewhere(table))
        {
            Load(reopened, db);
        }

        Assert.True(reopened.Count == 3,
            $"Three carts went into the table and the rotation holds {reopened.Count} after "
            + "Load.\n"
            + "    List<Song> loaded = db.Carts.OrderBy(song => song.Id).ToList();\n"
            + "👉 Next: Load reads the table into a list, then Clear() and AddRange(loaded).");

        Assert.True(reopened.All().Select(s => s.Title)
                        .SequenceEqual(new[] { "Nightjar", "Slack Water", "Long Way Round" }),
            $"The carts came back as {Titles(reopened.All())}, and they went in as Nightjar, "
            + "Slack Water, Long Way Round. The hour is built from the first three, so the "
            + "order matters.\n"
            + "⚠️ A table keeps no order of its own. Ask for it:\n"
            + "    db.Carts.OrderBy(song => song.Id).ToList()");

        Assert.True(reopened.All()[0].PlaysTonight == 1,
            $"Nightjar aired once before it was saved and came back saying "
            + $"{reopened.All()[0].PlaysTonight}. PlaysTonight is a column — check it has no "
            + "[NotMapped] on it.");

        // Loading twice must not stack the carts up.
        using (DbContext db = Somewhere(table))
        {
            Load(reopened, db);
        }

        Assert.True(reopened.Count == 3,
            $"Loaded twice, and the rotation holds {reopened.Count} carts.\n"
            + "⚠️ Loading is REPLACING. _songs.Clear() goes before the AddRange.");

        // ⚠️ The first night: a table nobody has saved to yet.
        var firstNight = ThreeCarts();

        using (DbContext db = Somewhere(Fresh("first-night")))
        {
            Load(firstNight, db);
        }

        Assert.True(firstNight.Count == 3,
            $"A rotation holding 3 carts loaded from an EMPTY table and now holds "
            + $"{firstNight.Count}.\n"
            + "⚠️ That is the first night. The table is always there, but nobody has saved "
            + "to it yet. A Load that clears first throws away the three carts Program.cs "
            + "just added, and the desk crashes building the hour.\n"
            + "👉 Next: read the rows into a list FIRST, and return before the Clear() when "
            + "there are none:\n"
            + "    if (loaded.Count == 0)\n"
            + "    {\n"
            + "        return;\n"
            + "    }");
    }

    // ── Check 5 — Task 5: the second save changes the rows it already has ───

    [Fact]
    public void Check5_ASecondSaveUpdatesTheRows()
    {
        string table = Fresh("second-save");

        using (DbContext db = Somewhere(table))
        {
            Save(ThreeCarts(), db);
        }

        // The next shift: one context, read at the start and written at the
        // end, the same way the desk does it.
        var nextNight = new Rotation();

        using (DbContext db = Somewhere(table))
        {
            Load(nextNight, db);
            nextNight.All()[2].Play();

            try
            {
                RotationMethod("Save").Invoke(nextNight, new object[] { db });
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                throw new Xunit.Sdk.XunitException(
                    "The second save threw instead of saving:\n"
                    + $"    {e.InnerException.GetType().Name}: {e.InnerException.Message}\n"
                    + "⚠️ Every cart that came out of the table already has a row, and its Id "
                    + "is that row's number. Add tries to put it in AGAIN. On the college's "
                    + "server the same mistake says 'Cannot insert explicit value for identity "
                    + "column'.\n"
                    + "👉 Next: a cart with Id 0 is new. Any other Id is a row that changes:\n"
                    + "    if (song.Id == 0)\n"
                    + "    {\n"
                    + "        db.Carts.Add(song);\n"
                    + "    }\n"
                    + "    else\n"
                    + "    {\n"
                    + "        db.Carts.Update(song);\n"
                    + "    }");
            }
        }

        var reopened = new Rotation();

        using (DbContext db = Somewhere(table))
        {
            Assert.True(db.Set<Song>().Count() == 3,
                $"Saved, loaded and saved again, and the table holds {db.Set<Song>().Count()} "
                + "carts. It should still hold 3: a cart that came out of the table is a row "
                + "that changes, not a new row.");

            Load(reopened, db);
        }

        Assert.True(reopened.All()[2].PlaysTonight == 1,
            $"Long Way Round aired on the second night and came back saying "
            + $"{reopened.All()[2].PlaysTonight}. The second save has to change the row "
            + "it already has.");
    }
}
