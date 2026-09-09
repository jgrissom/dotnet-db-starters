// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  FOUR checks this week.
//
//  ⭐ CHECK 1 IS DOING SOMETHING NEW, AND IT IS THE POINT OF THE WEEK. It
//  asserts everything weeks 4 through 8 built — including the loops you
//  are invited to rewrite tonight. It is green before you start, and its
//  job is to still be green afterwards. That is what makes changing code
//  that already works a safe thing to do rather than a brave one.
//
//  Checks 2, 3 and 4 are the three questions your registry could not
//  answer: just the names, in order, and the ones that match.
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
    public void Check1_WeeksFourToEightStillHold()
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

        // ── week 8: the round trip ─────────────────────────────────────────

        var path = StudentCode.Scratch("carried.json");
        var saved = StudentCode.NewRegistry();
        StudentCode.Add(saved, StudentCode.NewItem(saved, Mango));
        StudentCode.Add(saved, StudentCode.NewItem(saved, Zebra));
        StudentCode.Add(saved, StudentCode.NewItem(saved, Alpha));

        var record = StudentCode.Find(saved, Zebra)!;
        var verb = StudentCode.TheVerb(saved);
        var moved = verb == null ? Array.Empty<string>() : StudentCode.WhatItMoves(record, verb);
        var expected = StudentCode.Snapshot(record);

        StudentCode.Save(saved, path);

        Assert.True(File.Exists(path),
            "Registry.Save() was handed a path and no file appeared there. Week 8, and it "
            + "still has to work.");

        var reopened = StudentCode.NewRegistry();
        StudentCode.Load(reopened, path);

        Assert.True(StudentCode.Count(reopened) == 3,
            $"Three records were saved, and loading them into a fresh registry gave "
            + $"{StudentCode.Count(reopened)}. Week 8's Load.\n"
            + "⚠️ If you swapped the foreach in Load for AddRange, check that the Clear() "
            + "above it went with the loop rather than instead of it.");

        // ⚠️ Loading onto a registry that ALREADY holds records — which is what
        // your own Program.cs does. An empty one cannot tell you whether the
        // Clear() is still there.
        var already = StudentCode.NewRegistry();
        StudentCode.Add(already, StudentCode.NewItem(already, Mango));
        StudentCode.Add(already, StudentCode.NewItem(already, Zebra));
        StudentCode.Add(already, StudentCode.NewItem(already, Alpha));
        StudentCode.Load(already, path);

        Assert.True(StudentCode.Count(already) == 3,
            $"A registry already holding 3 records loaded a file holding 3 and now holds "
            + $"{StudentCode.Count(already)}.\n"
            + "⚠️ Loading is REPLACING, and the Clear() at the top of Load is what makes "
            + "that true. Your own Program.cs seeds records and then loads on top of them, "
            + "so this is the shape that actually bites.\n"
            + "👉 Next: put _items.Clear(); back above the AddRange.");

        foreach (var name in new[] { Zebra, Alpha, Mango })
        {
            Assert.True(StudentCode.Find(reopened, name) != null,
                $"Three records came back and Find(\"{name}\") cannot see one of them. Week "
                + "8 — the name did not survive the trip, which usually means the property "
                + "NewItem puts it into has no public setter. [JsonInclude] above it.");
        }

        if (moved.Length > 0)
        {
            var back = StudentCode.Find(reopened, Zebra)!;
            var after = StudentCode.Snapshot(back);

            foreach (var name in moved)
            {
                Assert.True(Equals(expected[name], after[name]),
                    $"{verb!.Name}() moved {name} to {Show(expected[name])}, and after a "
                    + $"save and a load it says {Show(after[name])}. That was week 8's "
                    + "[JsonInclude], and it ships in your own code already made.");
            }
        }
    }

    // ── Check 2 — just the names ───────────────────────────────────────────

    [Fact]
    public void Check2_TheRegistryHandsBackItsNames()
    {
        var registry = StudentCode.NewRegistry();
        StudentCode.Add(registry, StudentCode.NewItem(registry, Mango));
        StudentCode.Add(registry, StudentCode.NewItem(registry, Zebra));
        StudentCode.Add(registry, StudentCode.NewItem(registry, Alpha));

        var names = StudentCode.Names(registry);

        Assert.True(names.Count == 3,
            $"Registry.Names() handed back {names.Count} names out of a registry holding 3. "
            + $"(It gave: {Show(names)})\n"
            + "One name per record, and Select is the word for \"turn every one of these "
            + "into something else\":\n"
            + "    public List<string> Names()\n"
            + "    {\n"
            + "        return _items.Select(item => item.Name).ToList();\n"
            + "    }\n"
            + "Where keeps SOME of the things. Select keeps all of them and changes what "
            + "each one IS — here, from a record into the one string on it.\n"
            + "👉 Next: swap Name for whatever your own name property is called.");

        Assert.True(names.SequenceEqual(new[] { Mango, Zebra, Alpha }),
            $"Registry.Names() gave {Show(names)} and should give\n"
            + $"    {Show(new List<string> { Mango, Zebra, Alpha })}\n"
            + "Two things this could be. Either it is handing back something other than "
            + "the name — Select takes ONE property off each record, not the record's whole "
            + "line — or it has put them in order, and Names() is not the method that "
            + "does that. It hands them back in the registry's own order, which is the "
            + "order they were added in. Sorting is check 3's job.");

        var empty = StudentCode.NewRegistry();
        Assert.True(StudentCode.Names(empty).Count == 0,
            "Registry.Names() on an empty registry handed back "
            + $"{StudentCode.Names(empty).Count} name(s). An empty registry has no names — "
            + "and Select over nothing is an empty list rather than null or an error.");
    }

    // ── Check 3 — in order ─────────────────────────────────────────────────

    [Fact]
    public void Check3_TheRegistryComesBackInOrder()
    {
        var registry = StudentCode.NewRegistry();
        StudentCode.Add(registry, StudentCode.NewItem(registry, Mango));
        StudentCode.Add(registry, StudentCode.NewItem(registry, Zebra));
        StudentCode.Add(registry, StudentCode.NewItem(registry, Alpha));

        var sorted = StudentCode.Sorted(registry);

        Assert.True(sorted.Count == 3,
            $"Registry.Sorted() handed back {sorted.Count} records out of a registry holding "
            + "3. Sorting does not drop anything — every record comes back, in a different "
            + "order:\n"
            + "    public List<Lighthouse> Sorted()\n"
            + "    {\n"
            + "        return _items.OrderBy(item => item.Name).ToList();\n"
            + "    }\n"
            + "👉 Next: swap Lighthouse and Name for your own record and its name property.");

        // By REFERENCE, through the student's own Find — so this check never
        // has to know what their name property is called.
        var alpha = StudentCode.Find(registry, Alpha);
        var mango = StudentCode.Find(registry, Mango);
        var zebra = StudentCode.Find(registry, Zebra);

        // Which record came back first says exactly what went wrong, which is
        // why the three were added in the order they were.
        string diagnosis =
            ReferenceEquals(sorted[0], mango)
                ? "That is the first record ADDED, so nothing sorted at all — Sorted() is "
                  + "handing back the registry's own order.\n"
                  + "👉 Next: OrderBy(item => item.Name), and ToList() on the end."
            : ReferenceEquals(sorted[0], zebra)
                ? "That is LAST alphabetically, so it sorted backwards — that is "
                  + "OrderByDescending.\n"
                  + "👉 Next: OrderBy is the one that starts at A."
                : "That is not any of the three orders I can account for, so it is sorting "
                  + "by something other than the name.\n"
                  + "👉 Next: read what is inside the lambda — it should be the same property "
                  + "NewItem sets and Find matches on.";

        Assert.True(ReferenceEquals(sorted[0], alpha),
            $"Registry.Sorted() put the wrong record first. The three names were added as "
            + $"\"{Mango}\", \"{Zebra}\", \"{Alpha}\" — so in order they are \"{Alpha}\", "
            + $"\"{Mango}\", \"{Zebra}\", and the first one is \"{Alpha}\".\n"
            + "⚠️ " + diagnosis);

        Assert.True(ReferenceEquals(sorted[1], mango) && ReferenceEquals(sorted[2], zebra),
            $"Registry.Sorted() got the first one right and then went wrong. In order it is "
            + $"\"{Alpha}\", \"{Mango}\", \"{Zebra}\".");

        Assert.True(sorted.All(r => r != null)
                    && sorted.Select(r => r!).Distinct().Count() == 3,
            "Registry.Sorted() handed back the same record twice, or a null. Every record "
            + "comes back exactly once.");

        // ⚠️ The one this check really exists for.
        var stillThere = StudentCode.All(registry).Cast<object?>().ToList();
        Assert.True(ReferenceEquals(stillThere[0], mango)
                    && ReferenceEquals(stillThere[1], zebra)
                    && ReferenceEquals(stillThere[2], alpha),
            "Sorting the registry SORTED THE REGISTRY. All() is handing records back in a "
            + "different order from the one they were added in.\n"
            + "⚠️ OrderBy sorts a COPY and hands the copy back — it never touches the list "
            + "it was asked about. Whatever you did reached _items itself, and it was "
            + "probably List.Sort, which rearranges in place.\n"
            + "That distinction is the one to keep from this week: a query answers a "
            + "question and leaves the thing alone. Your registry's own order is the order "
            + "records arrived in, and Save writes them in that order — so a Sort in here "
            + "quietly rewrites your file too.\n"
            + "👉 Next: OrderBy, and let it build you a new sequence.");

        var empty = StudentCode.NewRegistry();
        Assert.True(StudentCode.Sorted(empty).Count == 0,
            "Registry.Sorted() on an empty registry handed back "
            + $"{StudentCode.Sorted(empty).Count} record(s). Sorting nothing gives you an "
            + "empty list, not null and not an error.");
    }

    // ── Check 4 — the ones that match ──────────────────────────────────────

    [Fact]
    public void Check4_TheRegistryFindsEveryMatch()
    {
        const string Sable = "Sable Point Light";
        const string Pointe = "Pointe Aux Barques";
        const string Round = "The Roundhouse";

        var registry = StudentCode.NewRegistry();
        StudentCode.Add(registry, StudentCode.NewItem(registry, Sable));
        StudentCode.Add(registry, StudentCode.NewItem(registry, Pointe));
        StudentCode.Add(registry, StudentCode.NewItem(registry, Round));

        var matches = StudentCode.Matching(registry, "Point");

        Assert.True(matches.Count == 2,
            $"Registry.Matching(\"Point\") handed back {matches.Count} records and should "
            + "hand back 2.\n"
            + $"The three names are \"{Sable}\", \"{Pointe}\" and \"{Round}\" — two of "
            + "those have \"Point\" somewhere inside them.\n"
            + "    public List<Lighthouse> Matching(string term)\n"
            + "    {\n"
            + "        return _items.Where(item => item.Name.Contains(term)).ToList();\n"
            + "    }\n"
            + "Where keeps the ones the question is TRUE for and drops the rest.\n"
            + $"👉 Next: if you got 1, it is StartsWith rather than Contains — \"{Sable}\" "
            + "has Point in the middle of it. If you got 0, the method is still handing back "
            + "an empty list. If you got 3, the question inside the lambda is true of "
            + "everything.");

        Assert.True(ReferenceEquals(matches[0], StudentCode.Find(registry, Sable))
                    && ReferenceEquals(matches[1], StudentCode.Find(registry, Pointe)),
            $"Registry.Matching(\"Point\") found two records, and they are not the two I "
            + $"expected in the order I expected: \"{Sable}\" then \"{Pointe}\".\n"
            + "Where does not reorder anything — it keeps what is left in the order it "
            + "found it, and those two were added in that order.\n"
            + "⚠️ It also hands back the RECORDS the registry is holding, never copies of "
            + "them. That is week 5's rule, and it is what lets a caller act on what came "
            + "back.");

        Assert.True(StudentCode.Matching(registry, "Roundhouse").Count == 1,
            "Registry.Matching(\"Roundhouse\") found "
            + $"{StudentCode.Matching(registry, "Roundhouse").Count} records and should find "
            + $"1 — \"{Round}\" has it on the end.");

        Assert.True(StudentCode.Matching(registry, "Lighthouse of Alexandria").Count == 0,
            "Registry.Matching() found something for a term no record contains. It found "
            + $"{StudentCode.Matching(registry, "Lighthouse of Alexandria").Count}.\n"
            + "A question that is true of nothing hands back an empty list — not null, and "
            + "not an error.");

        Assert.True(StudentCode.Matching(registry, "").Count == 3,
            "Registry.Matching(\"\") found "
            + $"{StudentCode.Matching(registry, "").Count} records out of 3. Every string "
            + "contains the empty string, so an empty search term matches everything. That "
            + "is not a special case you have to write — it is what Contains already does, "
            + "and it is the sensible answer for a search box nobody has typed in yet.");

        Assert.True(StudentCode.Count(registry) == 3,
            $"Asking Matching changed the registry — it holds {StudentCode.Count(registry)} "
            + "records now and it held 3.\n"
            + "⚠️ A query ASKS. Where builds a new sequence, ToList() copies that into a new "
            + "list, and _items is untouched by both.");
    }

    private static string Show(object? value) =>
        value == null ? "null" : $"{value}";

    private static string Show(List<string> names) =>
        names.Count == 0 ? "(nothing)" : string.Join(", ", names.Select(n => $"\"{n}\""));
}
