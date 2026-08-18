// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your project repo; I don't have a second, secret set.
//
//  Run them from the top of your PROJECT repo — the other window:
//      dotnet test Project.Checks
//
//  Week 2 of the semester project. Last week your record learned to
//  defend itself. This week it learns to DO something — and the two
//  questions underneath every check here are the two the whole session
//  was about:
//
//      how many of this thing are there?      (one each, or one shared)
//      and what happens when there isn't one?  (null, and living with it)
//
//  Nothing here asks what your topic is, what your record is called, or
//  what you named the method that does the doing.
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
    public void Check1_YourRecordDoesSomething()
    {
        var type = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        var sealedOnes = StudentCode.SealedProperties(type);
        Assert.True(sealedOnes.Length > 0,
            $"Every property on {type.Name} can still be written from anywhere outside it, "
            + "so there is nothing for a method of yours to be the only way into.\n"
            + "That was last week's check 4 and it is this week's starting line:\n"
            + "    public int TimesVisited { get; private set; }\n"
            + "👉 Next: take the public `set` off the fact your record is the authority on.");

        var verb = StudentCode.TheVerb(registry);

        Assert.True(verb != null,
            $"{type.Name} has a property nothing can write from outside — and nothing INSIDE "
            + "writes it either, so it never changes at all.\n"
            + "A sealed property with no method to move it is decoration: it promises \"only "
            + "I change this\" and then nothing ever does. Give it the verb that moves it:\n"
            + "    public int TimesVisited { get; private set; }\n"
            + "\n"
            + "    public void Visit()\n"
            + "    {\n"
            + "        TimesVisited++;\n"
            + "    }\n"
            + $"I tried every public method on {type.Name} ({StudentCode.VerbNames(type)}) "
            + "and none of them changed a sealed property. Call it whatever suits your topic "
            + "— Visit, Play, Check, Ride, Sighted. I never look at the name.\n"
            + "👉 Next: add the method, and make it the only way that fact moves.");

        // ...and it does it to ITSELF. One record's business is its own.
        var mine = StudentCode.NewItem(registry, "The First One");
        var yours = StudentCode.NewItem(registry, "The Second One");
        var yoursBefore = StudentCode.Snapshot(yours);
        StudentCode.WhatItMoves(mine, verb!);

        var alsoMoved = StudentCode.Moved(yoursBefore, yours);
        Assert.True(alsoMoved.Length == 0,
            $"I called {verb!.Name}() on one of your records and a DIFFERENT record changed: "
            + $"{string.Join(", ", alsoMoved)}.\n"
            + "There is one copy of that fact shared by every record in your program, which "
            + "is what `static` means — the field belongs to the CLASS, not to any one "
            + $"{type.Name}. One copy, made once, alive as long as the program runs, and "
            + "every record reporting the same number.\n"
            + "    private static int _timesVisited;              // ← one for everything\n"
            + "    public int TimesVisited { get; private set; }   // ← one per record\n"
            + "A fact ABOUT one of your records is never static.");
    }

    [Fact]
    public void Check2_TheRegistryCanFindOne()
    {
        var type = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        var first = StudentCode.NewItem(registry, "The Roundhouse");
        var second = StudentCode.NewItem(registry, "Sable Point");
        StudentCode.Add(registry, first);
        StudentCode.Add(registry, second);

        var found = StudentCode.Find(registry, "Sable Point");

        Assert.True(found != null,
            "Registry.Find(\"Sable Point\") handed back nothing, and a record made with that "
            + "exact name is on the registry.\n"
            + $"    public {type.Name}? Find(string name)\n"
            + "    {\n"
            + $"        foreach ({type.Name} item in _items)\n"
            + "        {\n"
            + "            if (item.Name == name) { return item; }\n"
            + "        }\n"
            + "        return null;\n"
            + "    }\n"
            + "⚠️ Find looks for the same fact NewItem is handed. NewItem takes a name and "
            + "puts it somewhere on the record; Find has to compare against that same "
            + "property, whatever you called it.\n"
            + "👉 Next: write the loop in Registry.Find.");

        // The one that matters this week: THE record, not a record like it.
        Assert.True(ReferenceEquals(found, second),
            $"Find(\"Sable Point\") handed back a {type.Name} — but not the one on the "
            + "registry. It built a new one with the same name.\n"
            + "That is a copy, and a copy is a dead end: every change made through it lands "
            + "on a record nothing else is looking at, and the one in the registry never "
            + "moves.\n"
            + "Find hands back the record it found. It never makes one:\n"
            + "        if (item.Name == name) { return item; }\n"
            + "Two names for one object — the thing the registry is holding and the thing "
            + "you were handed have to be the same object.");

        Assert.True(ReferenceEquals(StudentCode.Find(registry, "The Roundhouse"), first),
            "Find works for one record and not the other, so the loop is giving up on the "
            + "first record instead of walking all of them. The `if` goes INSIDE the "
            + "foreach; the `return null` goes after it.");
    }

    [Fact]
    public void Check3_AndNothingWhenThereIsnt()
    {
        var type = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        // The worst case first: nothing on the registry at all.
        Assert.True(StudentCode.Find(registry, "Nobody Here") == null,
            "Registry.Find handed something back from an EMPTY registry.\n"
            + "There was nothing to find, so the only honest answer was `null`. A Find that "
            + "reaches into the list for a record that isn't there — `return _items[0];` — "
            + "throws the moment the list is empty, which on a real registry is the first "
            + "run of the program.\n"
            + "👉 Next: `return null;` after the loop.");

        StudentCode.Add(registry, StudentCode.NewItem(registry, "The Roundhouse"));
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));

        var nothing = StudentCode.Find(registry, "Somewhere I Never Added");

        Assert.True(nothing == null,
            $"Find(\"Somewhere I Never Added\") handed back a {type.Name}, and nothing by "
            + "that name is on the registry. It is handing back the first record, or the "
            + "last one it looked at, rather than admitting there wasn't one.\n"
            + $"    public {type.Name}? Find(string name)\n"
            + "The `?` is what makes `null` a legal answer here — and `null` is the right "
            + "answer, not an embarrassing one. It means exactly what happened: I looked, "
            + "and there is no such record.\n"
            + "👉 Next: `return null;` goes AFTER the loop, not inside it.");

        Assert.True(StudentCode.Count(registry) == 2,
            $"Looking for a record that wasn't there changed the registry — it holds "
            + $"{StudentCode.Count(registry)} records instead of 2. Find looks. It never "
            + "adds, and it never removes.");
    }

    [Fact]
    public void Check4_AndCanTakeOneOffTheBooks()
    {
        var type = StudentCode.ItemType();
        var registry = StudentCode.NewRegistry();

        StudentCode.Add(registry, StudentCode.NewItem(registry, "The Roundhouse"));
        StudentCode.Add(registry, StudentCode.NewItem(registry, "Sable Point"));

        // The one that isn't there. Nothing should happen, and it should say so.
        bool removedNothing = StudentCode.Remove(registry, "Somewhere I Never Added");

        Assert.False(removedNothing,
            "Remove(\"Somewhere I Never Added\") said true, and nothing by that name was on "
            + "the registry. It only reports true when it actually took a record off:\n"
            + $"        {type.Name}? found = Find(name);\n"
            + "        if (found == null) { return false; }\n"
            + "        _items.Remove(found);\n"
            + "        return true;");

        Assert.True(StudentCode.Count(registry) == 2,
            $"Removing a record that wasn't there left the registry holding "
            + $"{StudentCode.Count(registry)} records instead of 2. When Find comes back "
            + "null there is nothing to do — that is the whole job of the `if`.");

        bool removed = StudentCode.Remove(registry, "The Roundhouse");

        Assert.True(removed,
            "Remove(\"The Roundhouse\") said false, and a record made with that exact name "
            + "was on the registry. If check 2 is also red, fix Find first — Remove is built "
            + "on it.\n"
            + "👉 Next: `return true;` after the record comes off the list.");

        Assert.True(StudentCode.Count(registry) == 1,
            $"Remove said true and the registry still holds {StudentCode.Count(registry)} "
            + "records. Saying so is not the same as doing it — the record has to come off "
            + "the private list:\n"
            + "        _items.Remove(found);\n"
            + "⚠️ Removing from the list All() handed you does nothing to the registry: that "
            + "is a copy of the list, and emptying it empties the copy.");

        Assert.True(StudentCode.Find(registry, "The Roundhouse") == null,
            "Remove said true and the registry says it holds one fewer — but Find still "
            + "turns the record up, so it came off something other than the list Find walks. "
            + "Both of them work on _items.");

        Assert.True(StudentCode.Find(registry, "Sable Point") != null,
            "Removing The Roundhouse took Sable Point off too. `_items.Remove(found)` takes "
            + "off the ONE record it was handed — if the whole list went, something called "
            + "Clear() instead.");
    }

    [Fact]
    public void Check5_LastWeeksDoorsStillHold()
    {
        var type = StudentCode.ItemType();

        var property = StudentCode.RegistryType()
            .GetProperty("Topic", BindingFlags.Public | BindingFlags.Static);
        var field = StudentCode.RegistryType()
            .GetField("Topic", BindingFlags.Public | BindingFlags.Static);

        Assert.True(property != null || field != null,
            "Registry has no public static Topic any more. It was there last week — put it "
            + "back:\n"
            + "    public static string Topic => \"Lighthouses of the Outer Banks\";");

        var topic = (property != null ? property.GetValue(null) : field!.GetValue(null)) as string;

        Assert.True(!string.IsNullOrWhiteSpace(topic)
                    && !NotATopic.Contains(topic!.Trim().ToLowerInvariant()),
            $"Registry.Topic says \"{topic}\" — say what your project is about, in words.");

        var fields = StudentCode.PublicFields(type);
        Assert.True(fields.Length == 0,
            $"{type.Name} has grown {fields.Length} public field(s) back: "
            + $"{string.Join(", ", fields.Select(f => f.Name))}.\n"
            + "A public field is a hole in the wall — anything, anywhere, can write anything "
            + "into it. That door was shut last week and it stays shut every week after.");

        // And the copy, which is now only half the story — see check 3.
        var registry = StudentCode.NewRegistry();
        StudentCode.Add(registry, StudentCode.NewItem(registry, "The First One"));
        StudentCode.Add(registry, StudentCode.NewItem(registry, "The Second One"));

        var handedBack = StudentCode.All(registry);
        handedBack.Clear();

        Assert.True(StudentCode.Count(registry) == 2,
            "Somebody emptied the list All() handed them and the Registry went from 2 "
            + $"records to {StudentCode.Count(registry)}. All() hands back a COPY of the "
            + "list:\n"
            + $"    public List<{type.Name}> All() {{ return new List<{type.Name}>(_items); }}\n"
            + "⚠️ Worth knowing this week: that copies the LIST, not the records in it. The "
            + "records are the same records — which is exactly what makes Find useful, and "
            + "exactly what makes handing your list out dangerous.");
    }
}
