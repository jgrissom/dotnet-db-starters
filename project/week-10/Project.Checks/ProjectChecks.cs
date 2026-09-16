// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  FOUR checks this week.
//
//  CHECK 1 is everything weeks 4 through 9 built, and it is green before
//  you start. Checks 2, 3 and 4 are this week's: a record shaped for a
//  table, a migration, and the registry surviving a restart in its new
//  home.
//
//  ⚠️ ⚠️ NOT ONE OF THESE CHECKS TOUCHES A DATABASE. Every one of them
//  hands your context somewhere else to keep its rows — a table that
//  lives inside the test run and disappears with it. That is why they
//  answer in milliseconds on a machine with no network. It is also why
//  they can say NOTHING about whether your connection string is right,
//  whether your database exists, or whether you ever ran
//  `dotnet ef database update`. Running your program is what tells you
//  that, and it is the only thing that does.
//
//  ⚠️ Nothing in here knows a single thing about YOUR record's property
//  names. Sorted() is checked by REFERENCE — three records go in, and I
//  ask your own Find for each one and compare. Read StudentCode.cs if you
//  want to see how.
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
    public void Check1_WeeksFourToNineStillHold()
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
    }

    // ── Check 2 — a record the database can keep ──────────────────────────

    [Fact]
    public void Check2_TheRecordIsShapedForATable()
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

    // ── Check 3 — the instructions for making the table ───────────────────

    [Fact]
    public void Check3_ThereIsAMigration()
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

    // ── Check 4 — and the registry survives a restart ─────────────────────

    [Fact]
    public void Check4_TheRegistrySurvivesARestart()
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
            + "👉 Next: read Save and Load side by side. Save: take out what is there, put "
            + "yours in, SaveChanges(). Load: read the rows, and replace what the registry "
            + "is holding.");

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
            + "The table is always there; on the first run it is empty, which is the same "
            + "situation wearing different clothes.\n"
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

    private static string Show(object? value) =>
        value == null ? "null" : $"{value}";
}
