// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  Week 4 of the semester project, and the week this file stops being
//  magic: it is a test project, the same kind of thing as the
//  Project.Tests you are writing tonight. Open it. Read it. Every line
//  is syntax you now know.
//
//  Only TWO checks this week, because most of tonight's points are in
//  YOUR tests — the ones `dotnet test Project.Tests` runs. Check 1 is
//  everything weeks 4-6 built, still holding. Check 5 is the one new
//  rule of the week, and it starts red on every registry in the room.
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
    public void Check1_WeeksFourToSixStillHold()
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
    }

    // ⚠️ Red until Task 5, and that is the design: your own test in
    // Project.Tests goes red against the same rule first. This is my copy
    // of it — proof the fix holds even for somebody whose test lies.
    [Fact]
    public void Check5_TheSameNameCannotRegisterTwice()
    {
        var type = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        var first = StudentCode.NewItem(registry, "The Roundhouse");
        StudentCode.Add(registry, first);

        var imposter = StudentCode.NewItem(registry, "The Roundhouse");
        StudentCode.Add(registry, imposter);

        Assert.True(StudentCode.Count(registry) == 1,
            $"I registered \"The Roundhouse\", then registered \"The Roundhouse\" again — "
            + $"and the registry now holds {StudentCode.Count(registry)} records. A registry "
            + "with the same thing on file twice can't answer the one question a registry "
            + "exists for: how many are there?\n"
            + "The guard lives in Add, and it is built on the Find you already have:\n"
            + "    public void Add(Lighthouse item)\n"
            + "    {\n"
            + "        if (Find(item.Name) != null) { return; }\n"
            + "        _items.Add(item);\n"
            + "    }\n"
            + "(Name here is whatever property NewItem puts the name into — the same one "
            + "Find compares against. NewItem itself changes nothing: making a record is "
            + "fine, REGISTERING it twice is not.)\n"
            + "👉 Next: Task 5 — and write your own test first, so you watch it fail "
            + "while the door is still open.");

        Assert.True(ReferenceEquals(StudentCode.Find(registry, "The Roundhouse"), first),
            "After a duplicate is refused, Find(\"The Roundhouse\") has to hand back the "
            + "ORIGINAL record — the one that was on the books first. The imposter is "
            + "turned away; the record it imitated is untouched.");

        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));
        Assert.True(StudentCode.Count(registry) == 2,
            $"A different name should still register — the guard is against the SAME name, "
            + $"not against adding at all. \"Sable Point\" went in and Count says "
            + $"{StudentCode.Count(registry)}.");
    }
}
