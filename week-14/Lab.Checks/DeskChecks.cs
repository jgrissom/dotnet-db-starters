// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-14/Lab.Checks
//
//  CHECK 1 is everything the desk already does, the database included. It
//  is green before you start, and its job is to still be green at the end.
//
//  Checks 2 and 3 are tonight's: a column nobody knows, and a column
//  everybody knows.
//
//  ⚠️ NOT ONE OF THESE CHECKS TOUCHES THE COLLEGE'S SERVER. They read your
//  classes, and the migrations you made, straight out of the project. So
//  they say nothing about whether you ran `database update`. Running the
//  desk is what tells you that.
// ═══════════════════════════════════════════════════════════════════

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

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

    // ── tonight's helpers ───────────────────────────────────────────────────

    // Runs a desk method with these lines typed at it. A loop that never ends
    // on the end of the input would hang the run, so it gets ten seconds.
    private static void TypeAt(string what, string[] lines, Action desk)
    {
        TextReader keyboard = Console.In;
        Console.SetIn(new StringReader(string.Join("\n", lines) + "\n"));

        try
        {
            Exception? crash = null;
            Task run = Task.Run(() =>
            {
                try { desk(); }
                catch (Exception e) { crash = e; }
            });

            Assert.True(run.Wait(TimeSpan.FromSeconds(10)),
                $"{what} was still going after ten seconds, with its input used up.\n"
                + "⚠️ When the input ends, Console.ReadLine() hands back null. A loop has to "
                + "leave on that, the same as on Enter:\n"
                + "    if (string.IsNullOrWhiteSpace(typed))\n"
                + "    {\n"
                + "        return;\n"
                + "    }");

            if (crash != null)
            {
                Exception inner = crash is TargetInvocationException && crash.InnerException != null
                    ? crash.InnerException : crash;
                throw new Xunit.Sdk.XunitException(
                    $"{what} crashed with {inner.GetType().Name}:\n"
                    + $"    {inner.Message}\n"
                    + "That is the stack dump the person at the desk would have seen. A desk "
                    + "method catches what it can answer.\n"
                    + "👉 Next: run the desk and do the same thing by hand.");
            }
        }
        finally
        {
            Console.SetIn(keyboard);
        }
    }

    private static string FileHolding(string json)
    {
        string path = Path.Combine(Path.GetTempPath(), $"kdxr-check-{Guid.NewGuid()}.json");
        File.WriteAllText(path, json);
        return path;
    }

    private const string GoodFile =
        "[{\"Title\":\"Harbor Lights\",\"Artist\":\"The Ferrymen\",\"Seconds\":244},"
        + "{\"Title\":\"Night Freight\",\"Artist\":\"Marguerite Vance\",\"Seconds\":198}]";

    private const string DamagedFile =
        "[{\"Title\":\"Harbor Lights\",\"Artist\":\"The Ferrymen\",\"Seconds\":244},{\"Tit";

    private const string BadFile =
        "[{\"Title\":\"Harbor Lights\",\"Artist\":\"The Ferrymen\",\"Seconds\":244},"
        + "{\"Title\":\"Night Freight\",\"Artist\":\"Marguerite Vance\",\"Seconds\":198},"
        + "{\"Title\":\"Dead Air\",\"Artist\":\"Nobody At All\",\"Seconds\":-198}]";

    // Program.cs with its comments taken out, found by walking up from here.
    private static string ProgramSource()
    {
        DirectoryInfo? folder = new DirectoryInfo(AppContext.BaseDirectory);

        while (folder != null && !File.Exists(Path.Combine(folder.FullName, "Lab", "Program.cs")))
        {
            folder = folder.Parent;
        }

        Assert.True(folder != null,
            "I can't find Lab/Program.cs. Run this from the top of your repo, with "
            + "week-14/Lab and week-14/Lab.Checks side by side.");

        string[] lines = File.ReadAllLines(Path.Combine(folder!.FullName, "Lab", "Program.cs"));
        return string.Join("\n", lines.Where(line => !line.TrimStart().StartsWith("//")));
    }

    // ── Check 1 — everything the desk already does, and still has to ───────

    [Fact] // green before you start, and it has to STAY green
    public void Check1_TheDeskStillAnswers()
    {
        TheDeskAnswers();
        AWrongTitleAsksAgain();
        TheDeskCatchesABadFile();
        ABadFileAddsNothing();
        TheDeskStopsInASentence();
    }

    // Weeks 3 to 12.
    private static void TheDeskAnswers()
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
            + "counts the play the way it always has, and it ships finished.");

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
            + "`dotnet ef database update` needs it to make your tables. Copy week 13 in "
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
            + "Save and Load ship finished, and nothing tonight touches them.");

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

        // ── week 12: a cart has many airings ────────────────────────────────

        string aired = Fresh("airings");
        var onAir = ThreeCarts();
        onAir.All()[0].Play();

        using (DeskContext db = Somewhere(aired))
        {
            Do("Rotation.Save", () => onAir.Save(db));
        }

        var nextDay = new Rotation();

        using (DeskContext db = Somewhere(aired))
        {
            Do("Rotation.Load", () => nextDay.Load(db));
            Assert.True(nextDay.Find("Nightjar")?.Airings.Count == 1,
                "A cart played once came back from the table with no airings. Load's "
                + ".Include(song => song.Airings) ships finished. Undo whatever changed.");

            List<string> answer = Ask("Rotation.Aired", () => new Rotation().Aired(db));
            Assert.True(Words(answer) == "Nightjar",
                $"Rotation.Aired gave {Words(answer)} and should give Nightjar. It ships "
                + "finished from last week, and it asks the database.");
        }
    }

    // ── week 13, Task 1: ask again 

    private static void AWrongTitleAsksAgain()
    {
        var rotation = ThreeCarts();

        using (DeskContext db = Somewhere(Fresh("ask-again")))
        {
            Do("Rotation.Save", () => rotation.Save(db));

            TypeAt("RotationDesk.PullACart", new[] { "No Such Cart", "Slack Water", "Long Way Round" },
                () => RotationDesk.PullACart(rotation, db));

            Assert.True(rotation.Find("Slack Water") == null,
                "I typed a title nobody has, then Slack Water, and Slack Water is still in "
                + "the rotation. The desk stopped listening after the first answer.\n"
                + "A wrong title should ask again: a while (true) around the prompt, leaving "
                + "with return when a cart comes off.\n"
                + "This shipped finished from last week; undo whatever changed in RotationDesk.cs.");

            Assert.True(rotation.Find("Long Way Round") != null,
                "I typed a wrong title, then Slack Water, then Long Way Round, and both came "
                + "off. Once a cart is pulled the desk is done: return right after the Pulled "
                + "line, or the loop asks again.");

            int before = rotation.Count;

            TypeAt("RotationDesk.PullACart", new[] { "", "Nightjar" },
                () => RotationDesk.PullACart(rotation, db));

            Assert.True(rotation.Count == before,
                "I pressed Enter at the prompt, and a cart came off anyway. Enter means give "
                + "up: leave the loop before anything is removed.\n"
                + "    if (string.IsNullOrWhiteSpace(typed))\n"
                + "    {\n"
                + "        return;\n"
                + "    }");
        }
    }

    // ── week 13, Task 2: catch it at the desk 

    private static void TheDeskCatchesABadFile()
    {
        string good = FileHolding(GoodFile);
        string damaged = FileHolding(DamagedFile);
        string nowhere = Path.Combine(Path.GetTempPath(), $"kdxr-check-{Guid.NewGuid()}.json");

        var rotation = ThreeCarts();

        using (DeskContext db = Somewhere(Fresh("catch")))
        {
            Do("Rotation.Save", () => rotation.Save(db));

            TypeAt("RotationDesk.TakeDelivery", new[] { nowhere, good },
                () => RotationDesk.TakeDelivery(rotation, db));

            Assert.True(rotation.Count == 5,
                $"I typed a path with no file at it, then a good file, and the rotation holds "
                + $"{rotation.Count} carts. It should hold 5.\n"
                + "A wrong path is the person's to fix, so it should ask again: the prompt "
                + "and the try both go inside a while (true).\n"
                + "This shipped finished from last week; undo whatever changed in RotationDesk.cs.");

            var second = ThreeCarts();
            string good2 = FileHolding(GoodFile);

            TypeAt("RotationDesk.TakeDelivery", new[] { damaged, good2 },
                () => RotationDesk.TakeDelivery(second, db));

            Assert.True(second.Count == 3,
                $"I typed a damaged file, then a good one, and the rotation holds {second.Count} "
                + "carts. It should hold 3.\n"
                + "Typing a damaged file's name again can't fix it, so the desk says so and goes "
                + "back to the menu, with return in the catch for JsonException.\n"
                + "This shipped finished from last week; undo whatever changed in RotationDesk.cs.");
        }
    }

    // ── week 13, Task 3: a bad file adds nothing 

    private static void ABadFileAddsNothing()
    {
        string bad = FileHolding(BadFile);
        var rotation = ThreeCarts();
        Exception? thrown = null;

        try
        {
            rotation.Import(bad);
        }
        catch (Exception e)
        {
            thrown = e;
        }

        Assert.True(thrown != null,
            "Rotation.Import read a file with a cart that is -198 seconds long and threw "
            + "nothing. The Seconds property refused the number quietly, so the cart came in "
            + "with no length at all.\n"
            + "A file has nobody to ask, so Import refuses out loud:\n"
            + "    throw new InvalidDataException($\"\\\"{song.Title}\\\" has no length.\");\n"
            + "This shipped finished from last week; undo whatever changed in Rotation.cs.");

        Assert.True(thrown is InvalidDataException,
            $"Rotation.Import threw {thrown!.GetType().Name} for a cart with no length, and "
            + "it should throw InvalidDataException.");

        Assert.True(rotation.Count == 3,
            $"Rotation.Import threw, and the rotation went from 3 carts to {rotation.Count}. "
            + "The two good carts ahead of the bad one got in. A bad file adds NOTHING: one "
            + "loop checks every cart, and only then does a second loop add them.");

        using (DeskContext db = Somewhere(Fresh("bad-file")))
        {
            string again = FileHolding(BadFile);

            TypeAt("RotationDesk.TakeDelivery", new[] { again },
                () => RotationDesk.TakeDelivery(rotation, db));
        }
    }

    // ── week 13, Task 4: stop in a sentence 

    private static void TheDeskStopsInASentence()
    {
        string program = ProgramSource();

        Assert.True(Regex.IsMatch(program, @"CanConnect\s*\("),
            "Program.cs never asks whether the database is there, so with the wifi off the "
            + "first thing the person sees is a stack dump.\n"
            + "    if (!db.Database.CanConnect())\n"
            + "This shipped finished from last week; undo whatever changed in Program.cs.");

        Assert.True(Regex.IsMatch(program, @"catch\s*\(\s*(Microsoft\.EntityFrameworkCore\.)?(DbUpdateException|DbUpdateConcurrencyException)\b"),
            "Program.cs has no catch for a row that has gone from its table.\n"
            + "    catch (DbUpdateException)\n"
            + "around the while loop, saying what happened, then return. I read the file for "
            + "this one, because nothing can call the top of a program.\n"
            + "This shipped finished from last week; undo whatever changed in Program.cs.");
    }

    // ── tonight's helpers ───────────────────────────────────────────────────

    // Every column any migration in Lab/Migrations adds to the Songs table.
    private static List<AddColumnOperation> AddedToSongs(string column) =>
        Migrations()
            .Select(t => (Migration)Activator.CreateInstance(t)!)
            .SelectMany(m => m.UpOperations.OfType<AddColumnOperation>())
            .Where(op => op.Table == "Songs" && op.Name == column)
            .ToList();

    // A context pointed at SQL Server, which never connects: it is only asked
    // whether your classes and your migrations agree.
    private static DeskContext Unconnected() =>
        new DeskContext(new DbContextOptionsBuilder<DeskContext>()
            .UseSqlServer("Server=nowhere;Database=nothing;TrustServerCertificate=True").Options);

    // ── Check 2 — Task 1: a column nobody knows ─────────────────────────────

    [Fact]
    public void Check2_ACartHasAnAddedDate()
    {
        PropertyInfo? added = typeof(Song).GetProperty("Added");

        Assert.True(added != null,
            "Song has no property called Added yet.\n"
            + "    public DateTime? Added { get; private set; }\n"
            + "👉 Next: Task 1, in week-14/Lab/Song.cs.");

        Assert.True(added!.PropertyType == typeof(DateTime?),
            $"Song.Added is a {added.PropertyType.Name}, and it should be DateTime? with the ?.\n"
            + "Nobody wrote down when the carts already in your table were added, so the "
            + "column has to be able to hold nothing at all. Without the ? every one of them "
            + "gets the year 1.");

        var song = new Song("Lantern Hours", "The Lamplighters", 214);
        object? stamped = added.GetValue(song);

        Assert.True(stamped is DateTime when && Math.Abs((DateTime.Now - when).TotalMinutes) < 5,
            "A cart made just now has no Added date. The constructor stamps it:\n"
            + "    Added = DateTime.Now;\n"
            + "That way every cart made from tonight on has one, and only the old ones show a dash.");

        MethodInfo? addedOn = typeof(Song).GetMethod("AddedOn", Type.EmptyTypes);

        Assert.True(addedOn != null && addedOn.ReturnType == typeof(string),
            "Song has no AddedOn() method that hands back a string. It goes beside "
            + "LastAired(), and it works the same way: a dash when there is nothing to show.\n"
            + "👉 Next: Task 1.");

        string today = Ask("AddedOn() on a cart made just now", () => (string)addedOn!.Invoke(song, null)!);
        Assert.True(!string.IsNullOrWhiteSpace(today) && today != "-",
            $"AddedOn() on a cart made just now handed back \"{today}\". It has a date, so it "
            + "should show it.");

        added.GetSetMethod(true)!.Invoke(song, new object?[] { null });
        string old = Ask("AddedOn() on a cart with no date", () => (string)addedOn!.Invoke(song, null)!);
        Assert.True(old == "-",
            $"AddedOn() on a cart with no date handed back \"{old}\", and should hand back \"-\".\n"
            + "    if (Added == null)\n"
            + "    {\n"
            + "        return \"-\";\n"
            + "    }");

        List<AddColumnOperation> made = AddedToSongs("Added");

        Assert.True(made.Count > 0,
            "No migration in Lab/Migrations adds an Added column to Songs, so the table "
            + "still has no place to keep it, and the desk crashes with Invalid column name 'Added'.\n"
            + "    dotnet ef migrations add ACartHasAnAddedDate --project week-14/Lab\n"
            + "👉 Next: Task 1.");

        Assert.True(made.All(op => op.IsNullable),
            "Your migration adds Added to Songs as a column that can't be empty. It should "
            + "say nullable: true. Song.Added needs its ?, then remove that migration and add "
            + "it again.");
    }

    // ── Check 3 — Task 2: a column everybody knows ──────────────────────────

    [Fact]
    public void Check3_TheOldCartsGetThreeStars()
    {
        PropertyInfo? stars = typeof(Song).GetProperty("Stars");

        Assert.True(stars != null && stars.PropertyType == typeof(int),
            "Song has no int property called Stars yet.\n"
            + "    public int Stars { get; set; } = 3;\n"
            + "👉 Next: Task 2, in week-14/Lab/Song.cs.");

        var song = new Song("Lantern Hours", "The Lamplighters", 214);
        Assert.True((int)stars!.GetValue(song)! == 3,
            $"A cart made just now has {stars.GetValue(song)} stars, and every cart starts at three.\n"
            + "    public int Stars { get; set; } = 3;");

        List<AddColumnOperation> made = AddedToSongs("Stars");

        Assert.True(made.Count > 0,
            "No migration in Lab/Migrations adds a Stars column to Songs yet.\n"
            + "    dotnet ef migrations add CartsGetStars --project week-14/Lab\n"
            + "👉 Next: Task 2.");

        object? given = made.Last().DefaultValue;
        Assert.True(Equals(given, 3),
            $"Your migration gives every cart already in the table {given ?? "nothing"} stars, "
            + "and it should give them 3.\n"
            + "The = 3 in Song.cs only reaches carts made from now on. The carts already in "
            + "the table get whatever the migration says, and migrations add wrote the type's "
            + "zero. Open the migration file ending _CartsGetStars.cs and change\n"
            + "    defaultValue: 0\n"
            + "to\n"
            + "    defaultValue: 3\n"
            + "⚠️ Already ran database update with the 0 in it? Task 2's box says how to take it back.");

        using DeskContext db = Unconnected();
        Assert.True(!db.Database.HasPendingModelChanges(),
            "Your classes have changed since your last migration: something in a class "
            + "isn't in Lab/Migrations yet, so database update wouldn't add it.\n"
            + "👉 Next: dotnet ef migrations add, with the name the task gives.");
    }
}
