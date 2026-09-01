// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  FOUR checks this week. Check 1 is everything weeks 4-7 built, still
//  holding, and it is green before you start. Checks 2, 3 and 4 are one
//  question in three parts, and all three start red on every registry in
//  the room: is it still there after the program has stopped?
//
//  The fifth item on the grading table is not in here at all. It is a
//  fact in YOUR suite, and it is worth reading the homework's Task 5 for
//  why it is named the way it is.
//
//  ⚠️ Every check that touches a file uses a scratch path of its own, in
//  the system's temp folder. Yours never gets read and never gets
//  written — which is only possible because Save and Load take the path
//  rather than knowing one.
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

    [Fact]
    public void Check1_WeeksFourToSevenStillHold()
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
            + $"records to {StudentCode.Count(registry)}. All() hands back a COPY.");

        Assert.True(ReferenceEquals(StudentCode.Find(registry, "The Roundhouse"), first),
            "Registry.Find no longer hands back the record the registry is holding. That "
            + "was week 5's check 2 and it is still the deal — Find never builds a new one.");

        Assert.True(StudentCode.Find(registry, "Somewhere I Never Added") == null,
            "Registry.Find handed something back for a name nobody has. Week 5's check 3.");

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

        // Week 7: the guard in Add. You wrote it last week, and it stays.
        // "Sable Point" is the one still on the books here — The Roundhouse was
        // taken off two asserts ago, so re-registering IT would be legitimate.
        var before = StudentCode.Count(registry);
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));
        Assert.True(StudentCode.Count(registry) == before,
            $"\"Sable Point\" was already on the books and registering it again took the "
            + $"count from {before} to {StudentCode.Count(registry)}. That was last week's "
            + "guard in Add, and it is still the deal:\n"
            + "    if (Find(item.Name) != null) { return; }");
    }

    // ── Check 2 — Task 2: the registry writes itself down ──────────────────

    [Fact]
    public void Check2_TheRegistryWritesItselfDown()
    {
        var path = StudentCode.Scratch("save.json");

        var registry = StudentCode.NewRegistry();
        StudentCode.Add(registry, StudentCode.NewItem(registry, "The Roundhouse"));
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));

        StudentCode.Save(registry, path);

        Assert.True(File.Exists(path),
            "Registry.Save() was handed a path and no file appeared there. Nothing was "
            + "written down, so nothing can come back.\n"
            + "Two moves, whatever format you pick: turn the records into text, and put "
            + "the text in the file you were handed.\n"
            + "    string json = JsonSerializer.Serialize(_items);\n"
            + "    File.WriteAllText(path, json);\n"
            + "⚠️ The path is the one Save was HANDED. A file name written inside the "
            + "method means one thing when your program runs and something else when a "
            + "test runs — they do not stand in the same folder.\n"
            + "👉 Next: Task 2 — and write your own test first, so you watch it fail "
            + "while there is genuinely nothing on disk.");

        var written = File.ReadAllText(path);

        Assert.True(written.Length > 0,
            "Registry.Save() made the file and left it empty. Whatever went in, it was "
            + "not the records — serialize the LIST the registry is holding.");

        Assert.True(written.Contains("The Roundhouse") && written.Contains("Sable Point"),
            "A file appeared and neither record's name is in it. Whatever got written, it "
            + $"was not the registry. The file says:\n{Excerpt(written)}\n"
            + "Serialize the list of records — not a count of them, and not a line you "
            + "built out of them.");
    }

    // ── Check 3 — Task 3: it survives a restart ────────────────────────────

    [Fact]
    public void Check3_TheRegistrySurvivesARestart()
    {
        var path = StudentCode.Scratch("survives.json");

        var registry = StudentCode.NewRegistry();
        StudentCode.Add(registry, StudentCode.NewItem(registry, "The Roundhouse"));
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Cape Fear River Light"));
        StudentCode.Save(registry, path);

        // A second registry, holding nothing, reading the same file. This is
        // the whole of "quit it and start it again", without quitting.
        var reopened = StudentCode.NewRegistry();
        StudentCode.Load(reopened, path);

        Assert.True(StudentCode.Count(reopened) == 3,
            $"Three records were saved, and loading them into a fresh registry gave "
            + $"{StudentCode.Count(reopened)}.\n"
            + "Load reads the file, turns the text back into your records, and puts them "
            + "in the list:\n"
            + "    List<Lighthouse>? loaded =\n"
            + "        JsonSerializer.Deserialize<List<Lighthouse>>(File.ReadAllText(path));\n"
            + "⚠️ Clear the list before you fill it, or a Load on top of a registry that "
            + "already holds records adds to them instead of replacing them.\n"
            + "👉 Next: Task 3.");

        foreach (var name in new[] { "The Roundhouse", "Sable Point", "Cape Fear River Light" })
        {
            Assert.True(StudentCode.Find(reopened, name) != null,
                $"Three records came back and Find(\"{name}\") cannot see one of them. The "
                + "records are in the list but the NAME did not survive the trip — which "
                + "usually means the property NewItem puts the name into has no public "
                + "setter, so it was written to the file and had no way back in.\n"
                + "A `{ get; private set; }` property goes out and never comes home. "
                + "[JsonInclude] above it is the sentence \"yes, this one too\".");
        }

        // The first run of a program that has never saved anything: no file,
        // and that is a first run rather than a failure.
        var firstRun = StudentCode.NewRegistry();
        StudentCode.Load(firstRun, StudentCode.Scratch("no-such-file.json"));

        Assert.True(StudentCode.Count(firstRun) == 0,
            $"Loading a path with no file at it left the registry holding "
            + $"{StudentCode.Count(firstRun)} record(s). Nothing was read, so nothing should "
            + "have arrived — ask File.Exists(path) FIRST and simply return when the answer "
            + "is no. The very first time anybody runs your program there is no file.");
    }

    // ── Check 4 — Task 4: a record keeps its own facts ─────────────────────

    [Fact]
    public void Check4_ARecordKeepsItsOwnFacts()
    {
        var path = StudentCode.Scratch("facts.json");
        var registry = StudentCode.NewRegistry();

        var verb = StudentCode.TheVerb(registry);

        Assert.True(verb != null,
            $"Your {StudentCode.ItemType().Name} has no method that moves something the "
            + "outside world cannot write — a count or a state the record owns, moved by a "
            + $"verb. That was week 5's job and it is what this check saves and reloads. "
            + $"(The public methods I can see on it: {StudentCode.VerbNames(StudentCode.ItemType())}.)");

        var record = StudentCode.NewItem(registry, "The Roundhouse");
        StudentCode.Add(registry, record);

        // Call their verb until something sealed actually moves, and remember
        // which properties those were.
        var moved = StudentCode.WhatItMoves(record, verb!);

        Assert.True(moved.Length > 0,
            $"{verb!.Name}() ran and nothing the outside world cannot write moved. Week 5's "
            + "check 3 asked for a property the record is the authority on; this week asks "
            + "whether it is still there after a save.");

        var expected = StudentCode.Snapshot(record);

        StudentCode.Save(registry, path);

        var reopened = StudentCode.NewRegistry();
        StudentCode.Load(reopened, path);

        var back = StudentCode.Find(reopened, "The Roundhouse");

        Assert.True(back != null,
            "Check 3 covers this one — the record did not come back at all. Finish Load "
            + "first, then come back to this check.");

        var after = StudentCode.Snapshot(back!);

        foreach (var name in moved)
        {
            Assert.True(Equals(expected[name], after[name]),
                $"{verb.Name}() moved {name} to {Show(expected[name])}, and after a save and "
                + $"a load it says {Show(after[name])}.\n"
                + "Open the file and look: the value IS in there. A serializer writes every "
                + "property it can READ and reads back only the ones it can WRITE — and "
                + $"{name} has a private setter, so it goes out and never comes home. A "
                + "count that resets every restart is not a count.\n"
                + "One line above the property says otherwise:\n"
                + "    [JsonInclude]\n"
                + $"    public int {name} {{ get; private set; }}\n"
                + "It needs `using System.Text.Json.Serialization;` at the top of the file.\n"
                + "👉 Next: Task 4 — and write the fact FIRST, so you watch it go red.");
        }
    }

    private static string Show(object? value) =>
        value == null ? "null" : $"{value}";

    // Enough of the file to recognize, never the whole thing — a registry with
    // fifty records in it would bury the message that matters.
    private static string Excerpt(string text) =>
        text.Length <= 300 ? text : text.Substring(0, 300) + "…";
}
