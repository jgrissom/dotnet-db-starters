// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  Week 1 of the semester project. Nothing here asks what your topic is,
//  what your record is called, or what your program prints. What it asks
//  is whether your class can defend itself — which is the whole of week 4
//  and the reason every week after this one has something to build on.
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
    public void Check1_YouPickedATopic()
    {
        var property = StudentCode.RegistryType()
            .GetProperty("Topic", BindingFlags.Public | BindingFlags.Static);
        var field = StudentCode.RegistryType()
            .GetField("Topic", BindingFlags.Public | BindingFlags.Static);

        Assert.True(property != null || field != null,
            "Registry has no public static Topic.\n"
            + "    public static string Topic => \"Lighthouses of the Outer Banks\";\n"
            + "One line, in words, saying what your program is about. It is the first thing "
            + "I read when I open your repo, and from week 12 it is what reminds you which "
            + "second thing your topic was supposed to grow.\n"
            + "👉 Next: add Topic to Project/Registry.cs.");

        var topic = (property != null
            ? property.GetValue(null)
            : field!.GetValue(null)) as string;

        Assert.False(string.IsNullOrWhiteSpace(topic),
            "Registry.Topic is blank. Say what your project is about in a few words — "
            + "\"Fire lookout towers in the Cascades\", \"Every bus route I've ridden\", "
            + "\"Competitive cheese\". Yours, and it should be obvious from reading it.");

        Assert.False(NotATopic.Contains(topic!.Trim().ToLowerInvariant()),
            $"Registry.Topic still says \"{topic}\" — that's the example, not a topic.\n"
            + "This is the one you get to choose. Pick the odd one; the odd ones are more "
            + "fun to build and much more fun to present in week 16.");
    }

    [Fact]
    public void Check2_YourRecordKeepsItsDataToItself()
    {
        var item = StudentCode.ItemType();
        var fields = StudentCode.PublicFields(item);

        Assert.True(fields.Length == 0,
            $"{item.Name} has {fields.Length} public field(s): "
            + $"{string.Join(", ", fields.Select(f => f.Name))}.\n"
            + "That is the shape your KDXR Call class shipped in last week, and it is the "
            + "shape this week exists to argue with: a public field is a hole in the wall. "
            + "Anything, anywhere in your program, can write anything into it — a blank "
            + "name, a negative price, a date in 1804 — and nothing warns you.\n"
            + "    private string _name = \"(unnamed)\";\n"
            + "    public string Name\n"
            + "    {\n"
            + "        get { return _name; }\n"
            + "        set { ... }          // ← your rule, on the way in\n"
            + "    }\n"
            + "👉 Next: turn the public fields on your record into properties.");

        var properties = StudentCode.Properties(item);
        Assert.True(properties.Length >= 3,
            $"{item.Name} has {properties.Length} public propert(ies), and your record needs "
            + "at least 3 for there to be anything to a record at all.\n"
            + "A thing worth keeping a registry of has a few facts about it: what it's "
            + "called, and at least two more that make it worth looking up.");
    }

    // This check and the next one both need properties to look at. With a class
    // that is still all public fields there are none, and BOTH of them would
    // otherwise report something that reads like a different problem. A check
    // that can't run yet is blocked, not failed — and one line says what to do.
    private static void BlockedIfNoProperties(Type type)
    {
        Assert.True(StudentCode.Properties(type).Length > 0,
            $"Blocked — {type.Name} has no properties at all yet, so there is nothing here "
            + "for me to look at.\n"
            + "This isn't a separate problem from check 2; it's the same one. Turn your "
            + "record's public fields into properties first and come back.\n"
            + "👉 Next: fix check 2, then run these again.");
    }

    [Fact]
    public void Check3_SomethingRefusesABadValue()
    {
        var type = StudentCode.ItemType();
        BlockedIfNoProperties(type);

        var registry = StudentCode.NewRegistry();
        var item = StudentCode.NewItem(registry, "Ordinary Name");

        var testable = new List<string>();
        var refused = new List<string>();

        foreach (var p in StudentCode.Properties(type))
        {
            if (!p.CanRead || !p.CanWrite) continue;
            if (p.GetSetMethod(nonPublic: false) == null) continue;

            object? nonsense = StudentCode.NonsenseFor(p.PropertyType);
            if (nonsense == null) continue;

            testable.Add(p.Name);
            try
            {
                p.SetValue(item, nonsense);
                if (!Equals(p.GetValue(item), nonsense)) refused.Add(p.Name);
            }
            catch (TargetInvocationException)
            {
                refused.Add(p.Name); // throwing it out is also turning it away
            }
        }

        Assert.True(testable.Count > 0,
            $"None of {type.Name}'s properties could be tested for this: every one of them "
            + "is either read-only from outside or of a type where there's no obviously "
            + "silly value to try (I try blank strings and negative numbers).\n"
            + "Your record needs at least one property that the outside world CAN set and "
            + "that checks what it's given — a name that won't go blank, a count that won't "
            + "go negative, a year that has to be a year.\n"
            + "👉 Next: give one settable property a `set` with a rule in it.");

        Assert.True(refused.Count > 0,
            $"Every settable property on {type.Name} stored a nonsense value exactly as "
            + $"given. I tried: {string.Join(", ", testable)}.\n"
            + "At least one of them should refuse. That is what a property is FOR — a field "
            + "and a property with an empty setter behave identically, and the second one "
            + "just takes longer to type:\n"
            + "        set { if (!string.IsNullOrWhiteSpace(value)) { _name = value; } }\n"
            + "Refusing means the old value stays. Nothing crashes, nothing is announced, "
            + "and the bad value simply never happened.");
    }

    [Fact]
    public void Check4_OnlyYourCodeCanChangeIt()
    {
        var type = StudentCode.ItemType();
        BlockedIfNoProperties(type);
        var properties = StudentCode.Properties(type);

        var sealedOnes = properties
            .Where(p => p.CanRead && p.GetSetMethod(nonPublic: false) == null)
            .Select(p => p.Name)
            .ToArray();

        Assert.True(sealedOnes.Length > 0,
            $"Every property on {type.Name} can be written from anywhere. At least one of "
            + "them should be readable by everybody and writable by nobody outside the "
            + "class:\n"
            + "    public int TimesVisited { get; private set; }\n"
            + "    public DateTime Added { get; }\n"
            + "Pick the fact that your record itself is the authority on — a count of "
            + "something that happened, when it was added, an id. If the outside world can "
            + "set it, then it is not a record of anything; it is just a number somebody "
            + "typed.\n"
            + "👉 Next: take the public `set` off one property on your record.");
    }

    [Fact]
    public void Check5_TheRegistryHoldsRecords()
    {
        var registry = StudentCode.NewRegistry();

        Assert.True(StudentCode.Count(registry) == 0,
            $"A brand-new Registry says it holds {StudentCode.Count(registry)}, and it "
            + "should hold 0. Count has to ask the list:\n"
            + "    public int Count => _items.Count;");

        StudentCode.Add(registry, StudentCode.NewItem(registry, "The first one"));
        Assert.True(StudentCode.Count(registry) == 1,
            $"After one Add, Count says {StudentCode.Count(registry)} and it should say 1.\n"
            + "    public void Add(Lighthouse item) { _items.Add(item); }");

        StudentCode.Add(registry, StudentCode.NewItem(registry, "The second one"));
        Assert.True(StudentCode.Count(registry) == 2,
            $"After a second Add, Count says {StudentCode.Count(registry)} and it should say "
            + "2. Every record goes on the END — Add never replaces what's already there.");

        var handedBack = StudentCode.All(registry);
        Assert.True(handedBack.Count == 2,
            $"All() handed back {handedBack.Count} record(s) after two Adds, and it should "
            + "hand back 2.");

        // The one that matters, and the same one your KDXR rotation had.
        handedBack.Clear();
        Assert.True(StudentCode.Count(registry) == 2,
            "Somebody emptied the list All() handed them, and the Registry went from 2 "
            + $"records to {StudentCode.Count(registry)}. So All() gave out the registry's "
            + "OWN list, and the `private` in front of it was never really doing anything:\n"
            + "    public List<Lighthouse> All() { return new List<Lighthouse>(_items); }\n"
            + "A copy. It costs one `new` and it is the difference between a registry and a "
            + "pile of things anybody can kick over.");
    }
}
