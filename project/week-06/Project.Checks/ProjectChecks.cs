// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  Week 3 of the semester project. Your record can defend itself (week 4)
//  and do something (week 5). This week it makes a PROMISE — and so does
//  the registry holding it, which is not a kind of record and never will
//  be. One list holds them both anyway.
//
//  ⚠️ Not one check below reads a word of what your Kind or your Line()
//  actually says. They ask whether the promise is kept.
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

    // What the homework's shape-first step leaves behind on purpose.
    private static readonly string[] NotAKind =
    {
        "?", "kind", "todo", "tbd", "-", "(kind)",
    };

    private static bool Placeholder(string? text) =>
        string.IsNullOrWhiteSpace(text)
        || NotAKind.Contains(text!.Trim().ToLowerInvariant());

    [Fact]
    public void Check1_WeeksFourAndFiveStillHold()
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
    }
    [Fact]
    public void Check2_YourRecordKeepsThePromise()
    {
        var listed = StudentCode.ListedType();
        var type = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        Assert.True(StudentCode.Keeps(type),
            $"{type.Name} does not keep IListed's promise, so nothing can put it on a "
            + "listing. One phrase, after the class name:\n"
            + $"    public class {type.Name} : IListed\n"
            + "Build it the moment you have typed that. The compiler answers with one "
            + "CS0535 for each member you still owe, which is the most useful to-do list "
            + "you will get tonight.\n"
            + "👉 Next: the colon, then the two members.");

        var item = StudentCode.NewItem(registry, "The Roundhouse");

        var kind = StudentCode.KindOf(item);
        Assert.True(!Placeholder(kind),
            $"{type.Name}.Kind says \"{kind}\", which is a placeholder rather than an "
            + "answer. It is the one word in the left-hand column of your listing — what "
            + $"a {type.Name} IS, in a word:\n"
            + "    public string Kind => \"LIGHTHOUSE\";\n"
            + "👉 Next: one line, in your record.");

        var line = StudentCode.LineOf(item);
        Assert.True(!string.IsNullOrWhiteSpace(line),
            $"{type.Name}.Line() came back empty. It is the rest of the row — the facts "
            + "this record is the authority on, in whatever words and whatever order you "
            + "like:\n"
            + "    public string Line() => $\"{Name} - {Height}ft - visited {TimesVisited}x\";\n"
            + "The wording is entirely yours. Coming back with something is not.");

        Assert.True(line.Contains("The Roundhouse"),
            $"I made a record called \"The Roundhouse\" and its Line() said:\n"
            + $"    {line}\n"
            + "A line on a listing that doesn't name the thing it is about is a line "
            + "nobody can use. Read the property NewItem put that name into — whatever "
            + "you called it — the same one Find compares against.");
    }

    [Fact]
    public void Check3_EachRecordWritesItsOwnLine()
    {
        var type = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        var first = StudentCode.NewItem(registry, "The Roundhouse");
        var second = StudentCode.NewItem(registry, "Sable Point");

        var one = StudentCode.LineOf(first);
        var other = StudentCode.LineOf(second);

        Assert.True(one != other,
            $"Two different records wrote the same line:\n    {one}\n    {other}\n"
            + "Line() is being built out of something fixed rather than out of the record "
            + "it is running on. Read the record's own properties — the ones the object "
            + "you are inside is holding.");

        Assert.True(StudentCode.KindOf(first) == StudentCode.KindOf(second),
            $"Two records of the same type gave two different Kinds — "
            + $"\"{StudentCode.KindOf(first)}\" and \"{StudentCode.KindOf(second)}\". "
            + $"Kind says what sort of thing this is, and both of these are a {type.Name}. "
            + "It is the same word for every one of them:\n"
            + "    public string Kind => \"LIGHTHOUSE\";");
    }

    [Fact]
    public void Check4_TheRegistryKeepsItToo()
    {
        var registryType = StudentCode.RegistryType();
        var itemType = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        Assert.True(StudentCode.Keeps(registryType),
            "Registry does not keep IListed's promise.\n"
            + "    public class Registry : IListed\n"
            + "⚠️ This is the whole point of the week, so it is worth being clear about "
            + $"what it is NOT saying. A Registry is not a {itemType.Name}. It is not a "
            + $"kind of {itemType.Name} and it never will be — one of them holds the "
            + "things and the other one IS one of the things. It goes on the same listing "
            + "anyway, because the "
            + "listing only ever asks two questions and this class can answer both.\n"
            + "👉 Next: the colon, then Kind and Line() on Registry.");

        var kind = StudentCode.KindOf(registry);
        Assert.True(!Placeholder(kind),
            $"Registry.Kind says \"{kind}\", which is a placeholder rather than an answer.\n"
            + "    public string Kind => \"REGISTRY\";");

        StudentCode.Add(registry, StudentCode.NewItem(registry, "The Roundhouse"));
        var item = StudentCode.NewItem(registry, "Sable Point");
        StudentCode.Add(registry, item);

        Assert.True(kind != StudentCode.KindOf(item),
            $"Registry and {itemType.Name} both call themselves \"{kind}\". Kind is how "
            + "somebody reading the listing tells one row from another — two different "
            + "sorts of things need two different words.");

        var line = StudentCode.LineOf(registry);
        Assert.True(!string.IsNullOrWhiteSpace(line),
            "Registry.Line() came back empty. It is the heading of your listing, and the "
            + "registry is the one object in the program that knows what to put in it:\n"
            + "    public string Line() => $\"{Topic} - {Count} on file\";");

        Assert.True(line != StudentCode.LineOf(item),
            $"Registry.Line() and {itemType.Name}.Line() said the same thing:\n"
            + $"    {line}\n"
            + "Same promise, two completely different answers — that is the only reason "
            + "putting them on one list is worth anything.");
    }

    [Fact]
    public void Check5_OneListHoldsThemBoth()
    {
        var itemType = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        var first = StudentCode.NewItem(registry, "The Roundhouse");
        var second = StudentCode.NewItem(registry, "Sable Point");
        StudentCode.Add(registry, first);
        StudentCode.Add(registry, second);

        var listing = StudentCode.Everything(registry);
        var things = listing.Cast<object?>().ToList();

        Assert.True(things.All(t => t != null),
            "Registry.Everything() handed back a list with a null in it. Everything on a "
            + "listing has to be able to answer — a null cannot, and the first loop over "
            + "it crashes.");

        Assert.True(things.Any(t => ReferenceEquals(t, first))
                    && things.Any(t => ReferenceEquals(t, second)),
            $"Registry.Everything() handed back {things.Count} thing(s), and the two "
            + $"records on the registry are not both in it — or they are copies rather "
            + "than the records themselves. Walk the private list and add the records you "
            + "are actually holding:\n"
            + $"        foreach ({itemType.Name} item in _items) {{ listing.Add(item); }}");

        Assert.True(things.Any(t => ReferenceEquals(t, registry)),
            "Registry.Everything() handed back the records and nothing else. The heading "
            + "is a line on the listing too, and the registry is what writes it:\n"
            + "    public List<IListed> Everything()\n"
            + "    {\n"
            + "        List<IListed> listing = new List<IListed>();\n"
            + "        listing.Add(this);\n"
            + $"        foreach ({itemType.Name} item in _items) {{ listing.Add(item); }}\n"
            + "        return listing;\n"
            + "    }\n"
            + "`this` is the registry the method is running on — the same word you watched "
            + "in the Variables pane last week.");

        Assert.True(things.Count == 3,
            $"Registry.Everything() handed back {things.Count} things for a registry "
            + "holding two records. It should be the registry's own line plus one per "
            + "record, and each of them exactly once.");

        // And every one of them answers, which is the only thing the list ever asked.
        var kinds = things.Select(t => StudentCode.KindOf(t!)).ToList();
        var lines = things.Select(t => StudentCode.LineOf(t!)).ToList();

        Assert.True(kinds.Distinct().Count() == 2,
            $"The listing came back with these kinds on it: {string.Join(", ", kinds)}. "
            + "Two sorts of things are on that list — the registry and your records — so "
            + "there should be exactly two different words in that column.");

        Assert.True(lines.All(l => !string.IsNullOrWhiteSpace(l)),
            "Something on the listing came back with an empty Line(). Every row has to say "
            + "something; that is the deal the promise makes.");
    }

}
