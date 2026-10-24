// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  FOUR checks this week.
//
//  CHECK 1 is everything weeks 4 through 11 built — the database and a
//  record that stays removed included — and it is green before you start.
//  Checks 2 to 4 are this week's: each record has many details, the details
//  come back with their record, and the database answers a question.
//
//  ⚠️ ⚠️ NOT ONE OF THESE CHECKS TOUCHES A DATABASE. Every one of them
//  hands your context somewhere else to keep its rows — a table that
//  lives inside the test run and disappears with it. That is why they
//  answer in milliseconds on a machine with no network. It is also why
//  they can say NOTHING about whether your connection string is right.
//  Running your program is what tells you that, and it is the only thing
//  that does.
//
//  ⚠️ Nothing in here knows a single thing about YOUR record's property
//  names. Read StudentCode.cs if you want to see how.
// ═══════════════════════════════════════════════════════════════════
using System.Reflection;

namespace Project.Checks;

public class ProjectChecks
{
    // Answers that mean "I haven't picked yet". Case and spaces ignored.
    private static readonly string[] NotATopic =
    {
        "your topic here", "topic", "todo", "tbd", "changeme", "my topic",
    };

    // Three names with distinct first letters, no ties, and nothing whose
    // order could depend on the machine's language settings.
    //
    // ⚠️ THE ORDER THEY GET ADDED IN IS CHOSEN, NOT ARBITRARY. Added as
    // Mango, Zebra, Alpha, the three ways Sorted() can be wrong all put a
    // DIFFERENT record first:
    //
    //      added        Mango, Zebra, Alpha   ← nothing sorted
    //      OrderBy      Alpha, Mango, Zebra   ← right
    //      Descending   Zebra, Mango, Alpha   ← sorted backwards
    //
    // Add them in sorted-adjacent order and two of those collide, which
    // would make check 3's message name the wrong cause.
    private const string Zebra = "Zebra Crossing";
    private const string Alpha = "Alpha Street";
    private const string Mango = "Mango Lane";

    [Fact] // green before you start, and it has to STAY green
    public void Check1_WeeksFourToElevenStillHold()
    {
        var type = StudentCode.ItemType();

        var property = StudentCode.RegistryType()
            .GetProperty("Topic", BindingFlags.Public | BindingFlags.Static);
        var field = StudentCode.RegistryType()
            .GetField("Topic", BindingFlags.Public | BindingFlags.Static);

        Assert.True(property != null || field != null,
            "Registry has no public static Topic any more. It has been there since week 4 "
            + "— put it back:\n"
            + "    public static string Topic => \"Lighthouses of the Outer Banks\";");

        var topic = (property != null ? property.GetValue(null) : field!.GetValue(null)) as string;

        Assert.True(!string.IsNullOrWhiteSpace(topic)
                    && !NotATopic.Contains(topic!.Trim().ToLowerInvariant()),
            $"Registry.Topic says \"{topic}\" — say what your project is about, in words.");

        var fields = StudentCode.PublicFields(type);
        Assert.True(fields.Length == 0,
            $"{type.Name} has grown {fields.Length} public field(s) back: "
            + $"{string.Join(", ", fields.Select(f => f.Name))}.\n"
            + "That door was shut in week 4 and it stays shut every week after.");

        var registry = StudentCode.NewRegistry();
        var first = StudentCode.NewItem(registry, "The Roundhouse");
        StudentCode.Add(registry, first);
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));

        var handedBack = StudentCode.All(registry);
        handedBack.Clear();

        Assert.True(StudentCode.Count(registry) == 2,
            "Somebody emptied the list All() handed them and the Registry went from 2 "
            + $"records to {StudentCode.Count(registry)}. All() hands back a COPY.\n"
            + "⚠️ Every query you add this week hands back a new list for the same reason. "
            + "That is what the ToList() on the end of them is for.");

        // Week 5 — and this is the one a rewrite breaks. Find has to be able to
        // come back empty-handed.
        Assert.True(ReferenceEquals(StudentCode.Find(registry, "The Roundhouse"), first),
            "Registry.Find no longer hands back the record the registry is holding. That "
            + "was week 5's check 2 and it is still the deal — Find never builds a new one.\n"
            + "⚠️ If you rewrote Find with FirstOrDefault, that is right and this is not "
            + "why it broke. Read the comparison inside the lambda.");

        Assert.True(StudentCode.Find(registry, "Somewhere I Never Added") == null,
            "Registry.Find handed something back for a name nobody has. Week 5's check 3.\n"
            + "⚠️ If you rewrote Find this week: it is FirstOrDefault, never First. First "
            + "OBJECTS to finding nothing — it throws InvalidOperationException — and "
            + "coming back empty-handed is half of what this method is for.");

        Assert.True(StudentCode.Remove(registry, "The Roundhouse")
                    && StudentCode.Count(registry) == 1,
            "Registry.Remove no longer takes a record off and says it did. Week 5's check 4.");

        Assert.True(!StudentCode.Remove(registry, "Somewhere I Never Added"),
            "Registry.Remove said true for a name nobody has. Week 5's check 4.");

        // Week 6: the promise, kept by the record AND by the registry.
        Assert.True(StudentCode.Keeps(type) && StudentCode.Keeps(StudentCode.RegistryType()),
            $"{(StudentCode.Keeps(type) ? "Registry" : type.Name)} no longer keeps IListed's "
            + "promise. That was week 6, and the listing still depends on it:\n"
            + $"    public class {(StudentCode.Keeps(type) ? "Registry" : type.Name)} : IListed");

        var listing = StudentCode.Everything(registry).Cast<object?>().ToList();
        Assert.True(listing.Any(t => ReferenceEquals(t, registry))
                    && listing.Count == StudentCode.Count(registry) + 1
                    && listing.All(t => t != null),
            "Registry.Everything() no longer hands back the registry's own line plus one "
            + "per record. Week 6's check 5, still the deal.");

        // Week 7: the guard in Add.
        var before = StudentCode.Count(registry);
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));
        Assert.True(StudentCode.Count(registry) == before,
            $"\"Sable Point\" was already on the books and registering it again took the "
            + $"count from {before} to {StudentCode.Count(registry)}. That was week 7's "
            + "guard in Add, and it is still the deal:\n"
            + "    if (Find(item.Name) != null) { return; }");

        // ⚠️ WEEK 8'S ROUND TRIP IS NOT IN HERE, and that is deliberate. It is
        // the one thing your registry did last week that this week MOVES, so it
        // is check 4 rather than a came-forward fact.

        // ── week 9: the three questions, which are now just things it does ──

        var asked = StudentCode.NewRegistry();
        StudentCode.Add(asked, StudentCode.NewItem(asked, Mango));
        StudentCode.Add(asked, StudentCode.NewItem(asked, Zebra));
        StudentCode.Add(asked, StudentCode.NewItem(asked, Alpha));

        var names = StudentCode.Names(asked);
        Assert.True(names.Count == 3 && names[0] == Mango && names[2] == Alpha,
            $"Registry.Names() handed back [{string.Join(", ", names)}] and should hand back "
            + $"[{Mango}, {Zebra}, {Alpha}] — every name, in the order the records were "
            + "added. Week 9, and nothing this week goes near it.");

        var sorted = StudentCode.Sorted(asked);
        Assert.True(sorted.Count == 3
                    && ReferenceEquals(sorted[0], StudentCode.Find(asked, Alpha)),
            "Registry.Sorted() no longer starts with the record that is first "
            + "alphabetically. Week 9.");

        Assert.True(ReferenceEquals(StudentCode.All(asked)[0], StudentCode.Find(asked, Mango)),
            "⚠️ Sorted() SORTED THE REGISTRY. It answers a question about the records and "
            + "leaves them exactly as it found them — OrderBy sorts a copy, List.Sort "
            + "rearranges the list itself. Week 9, and it matters more now than it did "
            + "then: the order your records come out of Load is the order they went into "
            + "the table.");

        var matching = StudentCode.Matching(asked, "a");
        Assert.True(matching.Count == 3,
            $"Registry.Matching(\"a\") handed back {matching.Count} records and should hand "
            + $"back 3 — every one of {Mango}, {Zebra} and {Alpha} has an \"a\" in it. "
            + "Week 9.");

        // ── week 10: the database, which is now just where the records live ──

        Still("check 2 — a record shaped for a table", Week10_TheRecordIsShapedForATable);
        Still("check 3 — a migration", Week10_ThereIsAMigration);
        Still("check 4 — the registry survives a restart", Week10_TheRegistrySurvivesARestart);
        Still("check 5 — a second save changes the row", Week10_ASecondSaveChangesTheRow);

        // ── week 11: a removed record stays gone ──

        Still11("check 2 — a removed record stays gone", Week11_ARemovedRecordStaysGone);
    }

    // A week-11 check, run again. If it fails, say whose it was.
    private static void Still11(string which, Action check)
    {
        try
        {
            check();
        }
        catch (Xunit.Sdk.XunitException e)
        {
            throw new Xunit.Sdk.XunitException(
                $"This was week 11's {which}, and it still has to hold:\n{e.Message}");
        }
    }

    // A week-10 check, run again. If it fails, say whose it was.
    private static void Still(string which, Action check)
    {
        try
        {
            check();
        }
        catch (Xunit.Sdk.XunitException e)
        {
            throw new Xunit.Sdk.XunitException(
                $"This was week 10's {which}, and it still has to hold:\n{e.Message}");
        }
    }

    // ── Week 10's check 2 — a record the database can keep ──────────────────────────

    private static void Week10_TheRecordIsShapedForATable()
    {
        using var db = StudentCode.NewContext(StudentCode.SomewhereElse("shape"));

        var table = StudentCode.TheTable(db);
        var item = StudentCode.ItemType();

        var key = table.FindPrimaryKey();
        Assert.True(key != null && key.Properties.Count == 1
                    && key.Properties[0].Name == "Id",
            $"{item.Name} has no Id for the database to tell its rows apart by"
            + (key == null ? "" : $" — the key it found was {string.Join(", ", key.Properties.Select(k => k.Name))}")
            + ".\n"
            + "⚠️ A table needs one column that is different on every row, and EF Core "
            + "looks for a property called Id. Nothing about your records has one — this "
            + "is the database's own name for the row, and it is the only property this "
            + "week asks you to add:\n"
            + "    public int Id { get; set; }\n"
            + $"👉 Next: put it at the top of your {item.Name} class.");

        // Everything NewItem sets has to survive, so the name has to be a column.
        var probe = StudentCode.NewItem(StudentCode.NewRegistry(), Mango);
        var named = StudentCode.Properties(item)
            .FirstOrDefault(pr => pr.PropertyType == typeof(string)
                                  && Equals(pr.GetValue(probe), Mango));

        Assert.True(named != null,
            "Nothing on your record holds the name NewItem was given, so nothing can be "
            + "stored. This is week 4's property and it should already be there.");

        Assert.True(table.FindProperty(named!.Name) != null,
            $"{item.Name}.{named.Name} holds the name, and it is not a column — so a record "
            + "would go into the table without its name and come back nameless.\n"
            + "⚠️ Check you have not put [NotMapped] on it.");
    }

    // ── Week 10's check 3 — the instructions for making the table ───────────────────

    private static void Week10_ThereIsAMigration()
    {
        var migrations = StudentCode.ItemType().Assembly
            .GetTypes()
            .Where(type => typeof(Microsoft.EntityFrameworkCore.Migrations.Migration)
                               .IsAssignableFrom(type)
                           && !type.IsAbstract)
            .ToArray();

        Assert.True(migrations.Length > 0,
            "There is no migration in your project.\n"
            + "⚠️ A migration is the instructions for making your table. It is a C# class "
            + "that `dotnet ef` writes for you, by comparing what your context says now to "
            + "what it said last time:\n"
            + "    dotnet ef migrations add TheRegistryMovesIn\n"
            + "👉 Next: run that from the top of your project repo, then open "
            + "Project/Migrations/ and read the Up() method — it is your own record class "
            + "with the column types filled in.\n"
            + "⚠️ Making the migration does not make the table. `dotnet ef database update` "
            + "is the one that talks to the server, and this check cannot see whether you "
            + "ran it. Running your program is what tells you that.");
    }

    // ── Week 10's check 4 — and the registry survives a restart ─────────────────────

    private static void Week10_TheRegistrySurvivesARestart()
    {
        var somewhere = StudentCode.SomewhereElse("restart");

        var saved = StudentCode.NewRegistry();
        StudentCode.Add(saved, StudentCode.NewItem(saved, Mango));
        StudentCode.Add(saved, StudentCode.NewItem(saved, Zebra));
        StudentCode.Add(saved, StudentCode.NewItem(saved, Alpha));

        // If the record keeps a count or a state of its own, move it — so the
        // check can say whether it came back too.
        var record = StudentCode.Find(saved, Zebra)!;
        var verb = StudentCode.TheVerb(saved);
        var moved = verb == null ? Array.Empty<string>() : StudentCode.WhatItMoves(record, verb);
        var expected = StudentCode.Snapshot(record);

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Save(saved, db);
        }

        // A second registry, holding nothing, and a second context — reading
        // the same table. Loading into the one that just saved proves nothing.
        var reopened = StudentCode.NewRegistry();

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(reopened, db);
        }

        Assert.True(StudentCode.Count(reopened) == 3,
            $"Three records were saved and {StudentCode.Count(reopened)} came back.\n"
            + (StudentCode.Count(reopened) == 0
                ? "⚠️ Nothing came back at all. Either Save never called SaveChanges() — "
                  + "Add and AddRange only write down what you INTEND to do — or Load is "
                  + "not asking the DbSet for what is in it.\n"
                : "")
            + "👉 Next: read Save and Load side by side. Save: Add each record, then "
            + "SaveChanges(). Load: read the rows into a list, then Clear() and AddRange.");

        foreach (var name in new[] { Zebra, Alpha, Mango })
        {
            Assert.True(StudentCode.Find(reopened, name) != null,
                $"Three records came back and Find(\"{name}\") cannot see one of them. The "
                + "name did not survive the trip, which usually means the property NewItem "
                + "puts it into is not being stored — check 2 covers the shape.");
        }

        if (moved.Length > 0)
        {
            var back = StudentCode.Find(reopened, Zebra)!;
            var after = StudentCode.Snapshot(back);

            foreach (var name in moved)
            {
                Assert.True(Equals(expected[name], after[name]),
                    $"{verb!.Name}() moved {name} to {Show(expected[name])}, and after a "
                    + $"save and a load it says {Show(after[name])}.\n"
                    + "⚠️ Week 8 needed [JsonInclude] to get a private setter back out of a "
                    + "file. EF Core does not — it sets private setters the same way your "
                    + "class does. If this is at its default, the property is probably "
                    + "[NotMapped], or it is worked out rather than kept.");
            }
        }

        // ⚠️ THE FIRST RUN: an empty table must leave the registry alone.
        var firstRun = StudentCode.NewRegistry();
        StudentCode.Add(firstRun, StudentCode.NewItem(firstRun, Mango));

        using (var db = StudentCode.NewContext(StudentCode.SomewhereElse("first-run")))
        {
            StudentCode.Load(firstRun, db);
        }

        Assert.True(StudentCode.Count(firstRun) == 1,
            $"A registry holding 1 record loaded from an EMPTY table and now holds "
            + $"{StudentCode.Count(firstRun)}.\n"
            + "⚠️ This is the first run, and it is the one that bites: your Program.cs "
            + "seeds records and THEN calls Load. On a table nobody has saved to yet, a "
            + "Load that clears first throws your seeds away.\n"
            + "⚠️ Week 8 had this too — it is what File.Exists at the top of Load was for. "
            + "The table is always there, but on the first run it is empty.\n"
            + "👉 Next: read the rows into a list FIRST, and return early when there are "
            + "none of them.");

        // And loading twice does not stack them up.
        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(reopened, db);
        }

        Assert.True(StudentCode.Count(reopened) == 3,
            $"Loaded twice and the registry holds {StudentCode.Count(reopened)} records.\n"
            + "⚠️ Loading is REPLACING. _items.Clear() before the AddRange.");
    }

    // ── Week 10's check 5 — the second save changes the rows it already has ─────────

    private static void Week10_ASecondSaveChangesTheRow()
    {
        var somewhere = StudentCode.SomewhereElse("second-save");

        var first = StudentCode.NewRegistry();
        StudentCode.Add(first, StudentCode.NewItem(first, Mango));
        StudentCode.Add(first, StudentCode.NewItem(first, Zebra));

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Save(first, db);
        }

        // The next run: one context, read at the start and written at the end,
        // the way a Program.cs does it.
        var nextRun = StudentCode.NewRegistry();
        string[] moved = Array.Empty<string>();
        Dictionary<string, object?> expected = new();
        MethodInfo? verb = null;

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(nextRun, db);

            var record = StudentCode.Find(nextRun, Zebra);
            Assert.True(record != null,
                "Two records were saved and Find cannot see one of them after a Load. "
                + "Check 4 covers this — make it green first.");

            verb = StudentCode.TheVerb(nextRun);
            if (verb != null)
            {
                moved = StudentCode.WhatItMoves(record!, verb);
                expected = StudentCode.Snapshot(record!);
            }

            StudentCode.Save(nextRun, db);
        }

        var reopened = StudentCode.NewRegistry();

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(reopened, db);
        }

        Assert.True(StudentCode.Count(reopened) != 0,
            "Two records were saved, loaded and saved again, and NONE came back. Nothing was "
            + "removed.\n"
            + "⚠️ Save took out the rows your list DOES hold. Look at this week's loop in Save: "
            + "the ! in front of _yourList.Any(...) is what makes it take out only the rows "
            + "your list doesn't hold.");

        Assert.True(StudentCode.Count(reopened) == 2,
            $"Two records were saved, loaded and saved again, and {StudentCode.Count(reopened)} "
            + "came back.\n"
            + "⚠️ A record that came out of the table already has a row. Saving it again "
            + "has to CHANGE that row, not add a second one:\n"
            + "    if (item.Id == 0)\n"
            + "    {\n"
            + "        db.YourRecords.Add(item);\n"
            + "    }\n"
            + "    else\n"
            + "    {\n"
            + "        db.YourRecords.Update(item);\n"
            + "    }");

        if (moved.Length > 0)
        {
            var after = StudentCode.Snapshot(StudentCode.Find(reopened, Zebra)!);

            foreach (var name in moved)
            {
                Assert.True(Equals(expected[name], after[name]),
                    $"{verb!.Name}() moved {name} to {Show(expected[name])} on the second run, "
                    + $"and after saving it came back as {Show(after[name])}. The second save "
                    + "has to change the row the table already has.");
            }
        }
    }

    // ── Week 11's check 2 — a record you remove stays gone ──────────────────

    private static void Week11_ARemovedRecordStaysGone()
    {
        var somewhere = StudentCode.SomewhereElse("removed");

        var first = StudentCode.NewRegistry();
        StudentCode.Add(first, StudentCode.NewItem(first, Mango));
        StudentCode.Add(first, StudentCode.NewItem(first, Zebra));
        StudentCode.Add(first, StudentCode.NewItem(first, Alpha));

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Save(first, db);
        }

        var before = new Dictionary<string, object?>();
        var saved = StudentCode.NewRegistry();

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(saved, db);
        }

        foreach (var name in new[] { Mango, Alpha })
        {
            var record = StudentCode.Find(saved, name);
            Assert.True(record != null,
                "Three records were saved and Find cannot see one of them after a Load. "
                + "Check 1 covers the round trip — make it green first.");
            before[name] = StudentCode.IdOf(record!);
        }

        // The next run: one context, read at the start and written at the end,
        // the way a Program.cs does it. Zebra is taken off in between.
        var nextRun = StudentCode.NewRegistry();

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(nextRun, db);

            Assert.True(StudentCode.Remove(nextRun, Zebra),
                $"Remove(\"{Zebra}\") said false on a registry that had just loaded it. "
                + "Check 1 covers Remove — make it green first.");

            StudentCode.Save(nextRun, db);
        }

        var reopened = StudentCode.NewRegistry();

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(reopened, db);
        }

        Assert.True(StudentCode.Count(reopened) <= 3,
            $"Three records were saved, one was removed, and the next Load found "
            + $"{StudentCode.Count(reopened)}.\n"
            + "⚠️ Save put records in a second time. This week's loop only takes rows OUT — "
            + "it never adds any. Emptying the table and adding your list back is not the "
            + "same thing, and it gives every record a new row.");

        Assert.True(!(StudentCode.Count(reopened) == 1 && StudentCode.Find(reopened, Zebra) != null),
            $"\"{Zebra}\" was removed, and it is the ONLY record left. The two that were kept "
            + "are gone.\n"
            + "⚠️ Save took out the rows your list DOES hold, and kept the one it doesn't. The "
            + "! in front of _yourList.Any(...) turns the question round:\n"
            + "    if (!_yourList.Any(item => item.Id == row.Id))");

        Assert.True(StudentCode.Find(reopened, Zebra) == null,
            $"\"{Zebra}\" was removed and the registry saved, and the next Load put it "
            + "straight back.\n"
            + "⚠️ Save adds and updates the records in your list. A removed record isn't in "
            + "the list any more, so nothing in Save ever mentions it, and its row stays in "
            + "the table. That is why it comes back on the next run.\n"
            + "👉 Next: in Save, after the loop and before SaveChanges, read the table and "
            + "take out every row whose Id no record in your list has:\n"
            + "    foreach (YourRecord row in db.YourRecords.ToList())\n"
            + "    {\n"
            + "        if (!_yourList.Any(item => item.Id == row.Id))\n"
            + "        {\n"
            + "            db.YourRecords.Remove(row);\n"
            + "        }\n"
            + "    }");

        Assert.True(StudentCode.Count(reopened) == 2
                    && StudentCode.Find(reopened, Mango) != null
                    && StudentCode.Find(reopened, Alpha) != null,
            $"One of three records was removed and the next Load found "
            + $"{StudentCode.Count(reopened)}. It should find the two that weren't removed, "
            + $"{Mango} and {Alpha}.\n"
            + "⚠️ Take out only the rows no record in your list has. Every other row stays.");

        foreach (var name in new[] { Mango, Alpha })
        {
            var now = StudentCode.IdOf(StudentCode.Find(reopened, name)!);
            Assert.True(Equals(before[name], now),
                $"\"{name}\" wasn't removed, and its Id went from {Show(before[name])} to "
                + $"{Show(now)}.\n"
                + "⚠️ Emptying the table and putting every record back in looks the same "
                + "when you run your program, and it gives every record a new row. Take out "
                + "the one row that isn't in your list, and leave the others alone.");
        }
    }

    // ── Check 2 — each record has many details ───────────────────────────────

    [Fact]
    public void Check2_EachRecordHasManyDetails()
    {
        var registry = StudentCode.NewRegistry();
        var detail = StudentCode.DetailType();

        using var db = StudentCode.NewContext(StudentCode.SomewhereElse("details-shape"));
        var nav = StudentCode.DetailsNavigation(db);

        var record = StudentCode.NewItem(registry, Mango);
        var list = StudentCode.DetailsOf(record, nav);

        Assert.True(list.Count == 0,
            $"A brand-new {record.GetType().Name} already has {list.Count} {detail.Name}s in its "
            + "list. Start the list empty.");

        var made = StudentCode.NewDetail(registry, "First one");
        Assert.True(detail.IsInstanceOfType(made),
            $"NewDetail handed back a {made.GetType().Name}, not a {detail.Name}.");

        list.Add(made);
        Assert.True(StudentCode.DetailsOf(record, nav).Count == 1,
            $"A {detail.Name} was added to {record.GetType().Name}.{nav.Name} and the list "
            + "still holds none. The property has to hand back the list the record keeps, "
            + "not a copy of it.");

        Assert.True(StudentCode.AMigrationMakes(detail),
            $"No migration in your project knows about {detail.Name}, so `dotnet ef database "
            + "update` has nothing that makes its table.\n"
            + "👉 Next: from the top of your project repo,\n"
            + "    dotnet ef migrations add ASecondTable --project Project\n"
            + "then read the new file in Project/Migrations before you run "
            + "`dotnet ef database update --project Project`.");
    }

    // ── Check 3 — the details come back with their record ───────────────────

    [Fact]
    public void Check3_TheDetailsComeBack()
    {
        var somewhere = StudentCode.SomewhereElse("details-back");
        var nav = Shape(somewhere);

        var first = StudentCode.NewRegistry();
        foreach (var name in new[] { Mango, Zebra, Alpha })
        {
            StudentCode.Add(first, StudentCode.NewItem(first, name));
        }

        var mango = StudentCode.Find(first, Mango)!;
        StudentCode.DetailsOf(mango, nav).Add(StudentCode.NewDetail(first, "First one"));
        StudentCode.DetailsOf(mango, nav).Add(StudentCode.NewDetail(first, "Second one"));
        StudentCode.DetailsOf(StudentCode.Find(first, Alpha)!, nav)
            .Add(StudentCode.NewDetail(first, "Only one"));

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Save(first, db);
        }

        var reopened = StudentCode.NewRegistry();

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Load(reopened, db);
        }

        var back = StudentCode.Find(reopened, Mango);
        Assert.True(back != null && StudentCode.Count(reopened) == 3,
            $"Three records were saved and {StudentCode.Count(reopened)} came back. Check 1 "
            + "covers Save and Load — make it green first.");

        int n = StudentCode.DetailsOf(back!, nav).Count;
        Assert.True(n != 0,
            $"\"{Mango}\" was saved with two details, and the next run it has none. The "
            + "details are in their table. Load never asked for them.\n"
            + "⚠️ Load reads your record's table, and only that table. EF Core brings each "
            + "record's details along only when Load asks it to:\n"
            + $"    db.YourRecords.Include(item => item.{nav.Name}).OrderBy(item => item.Id).ToList()\n"
            + "Include comes from EF Core, so Registry.cs needs its using line at the top:\n"
            + "    using Microsoft.EntityFrameworkCore;");

        int alpha = StudentCode.DetailsOf(StudentCode.Find(reopened, Alpha)!, nav).Count;
        int zebra = StudentCode.DetailsOf(StudentCode.Find(reopened, Zebra)!, nav).Count;
        Assert.True(n == 2 && alpha == 1 && zebra == 0,
            $"\"{Mango}\" was saved with 2 details, \"{Alpha}\" with 1 and \"{Zebra}\" with "
            + $"none, and they came back with {n}, {alpha} and {zebra}. Each record should "
            + "come back with exactly its own.");
    }

    // ── Check 4 — the database answers ──────────────────────────────────────

    [Fact]
    public void Check4_TheDatabaseAnswers()
    {
        var somewhere = StudentCode.SomewhereElse("with-details");
        var nav = Shape(somewhere);

        var first = StudentCode.NewRegistry();
        foreach (var name in new[] { Mango, Zebra, Alpha })
        {
            StudentCode.Add(first, StudentCode.NewItem(first, name));
        }

        StudentCode.DetailsOf(StudentCode.Find(first, Mango)!, nav)
            .Add(StudentCode.NewDetail(first, "First one"));
        StudentCode.DetailsOf(StudentCode.Find(first, Alpha)!, nav)
            .Add(StudentCode.NewDetail(first, "Only one"));

        using (var db = StudentCode.NewContext(somewhere))
        {
            StudentCode.Save(first, db);
        }

        // A registry holding NOTHING asks the question, so the answer can only
        // come from the table.
        var empty = StudentCode.NewRegistry();
        List<string> answer;

        using (var db = StudentCode.NewContext(somewhere))
        {
            answer = StudentCode.WithDetails(empty, db);
        }

        Assert.True(answer.Count != 0,
            $"\"{Mango}\" and \"{Alpha}\" both have details, and WithDetails handed back "
            + "nothing.\n"
            + "⚠️ This check asks a registry with NOTHING in its list. If WithDetails looks at "
            + "your list, there is nothing there. The question goes to the database:\n"
            + "    return db.YourRecords\n"
            + $"        .Where(item => item.{nav.Name}.Any())\n"
            + "        .OrderBy(item => item.Name)\n"
            + "        .Select(item => item.Name)\n"
            + "        .ToList();");

        Assert.True(!answer.Contains(Zebra),
            $"WithDetails handed back {string.Join(", ", answer)}, and \"{Zebra}\" has no "
            + "details. Keep only the records that have at least one:\n"
            + $"    .Where(item => item.{nav.Name}.Any())");

        Assert.True(answer.Count == 2 && answer.Contains(Mango) && answer.Contains(Alpha),
            $"WithDetails handed back {string.Join(", ", answer)}. It should hand back the "
            + $"names of the two records with details: {Alpha}, {Mango}. Select the property "
            + "that holds the name NewItem was given.");

        Assert.True(answer[0] == Alpha,
            $"WithDetails handed back {string.Join(", ", answer)}, and it should be in order "
            + $"by name: {Alpha}, {Mango}.\n"
            + "    .OrderBy(item => item.Name)");
    }

    // Check 2's shape, or a blocked message pointing at it.
    private static Microsoft.EntityFrameworkCore.Metadata.INavigation Shape(object somewhere)
    {
        try
        {
            using var db = StudentCode.NewContext(somewhere);
            return StudentCode.DetailsNavigation(db);
        }
        catch (Xunit.Sdk.XunitException)
        {
            throw new Xunit.Sdk.XunitException(
                "🚧 blocked — this check needs records that hold a list of details, and "
                + "your project doesn't have that yet.\n"
                + "👉 Next: check 2 first. It is Task 1.");
        }
    }

    private static string Show(object? value) =>
        value == null ? "null" : $"{value}";
}
