// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing
//  Lab/Caller.cs and Lab/Switchboard.cs, never this file.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-05/Lab.Checks
//
//  ⚠️ Something new in here, and it is tonight's subject looking back at
//  you. Most of these checks measure a DIFFERENCE — "her count went up by
//  one" — instead of a fixed number. That is not style. While CallsTonight
//  is still sitting on a `static` field there is one count for the whole
//  test run, so it carries over from one check to the next, and a check
//  that demanded "exactly 1" would fail for a reason that had nothing to
//  do with what it was asking about.
//
//  A shared number leaking between two checks that never met is the same
//  bug as Bex's calls showing up on Teodoro's row. Fix Task 2 and it
//  stops happening in both places at once.
// ═══════════════════════════════════════════════════════════════════
using System.Reflection;

namespace Lab.Checks;

public class SwitchboardChecks
{
    // ── plumbing ───────────────────────────────────────────────────────────

    private static PropertyInfo? PublicProperty(Type type, string name) =>
        type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

    private static string[] PublicFieldNames(Type type) =>
        type.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f => f.Name)
            .ToArray();

    private static Song ACart() => new Song("Nightjar", "The Lamplighters", 227);

    // ── Check 1 — weeks 1-4, shipped finished ──────────────────────────────

    [Fact] // passes out of the box
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — Broadcast.cs ships finished, so "
            + $"if this check is red something in it got changed. (It returned: "
            + $"\"{Broadcast.CallSign()}\")\n"
            + "Tonight's work is all in Lab/Caller.cs and Lab/Switchboard.cs.");

        var song = ACart();
        Assert.True(song.Length == "3:47",
            $"Song.Length says \"{song.Length}\" for a 227-second cart and it should say "
            + "\"3:47\". Song.cs ships finished this week — it is the file you closed hole "
            + "by hole last week. Undo whatever changed in it.");

        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. That door was shut last "
            + "week and Song.cs ships with it shut. Nothing tonight needs it opened.");

        var rotation = new Rotation();
        rotation.Add(ACart());
        rotation.All().Clear();
        Assert.True(rotation.Count == 1,
            $"Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. Rotation.cs ships finished — All() hands back a copy of "
            + "the list. Undo whatever changed in it.");
    }

    // ── Check 2 — Task 2: the count belongs to the caller ──────────────────

    [Fact]
    public void Check2_EveryCallerKeepsTheirOwnCount()
    {
        var calls = PublicProperty(typeof(Caller), "CallsTonight");

        Assert.True(calls != null && calls.PropertyType == typeof(int),
            "Caller has no public int property called CallsTonight — check the spelling and "
            + "the capital letter. The switchboard reads it by that exact name.");

        Assert.True(calls!.GetSetMethod(nonPublic: false) == null,
            "CallsTonight has a public setter, so any line anywhere in the program can "
            + "claim Dorothy rang forty times.\n"
            + "    public int CallsTonight { get; private set; }\n"
            + "                                   ^^^^^^^\n"
            + "Same shape as PlaysTonight last week: read it anywhere, write it nowhere "
            + "except inside Caller.");

        var dorothy = new Caller("Dorothy");
        var bex = new Caller("Bex");

        int dorothyBefore = dorothy.CallsTonight;
        int bexBefore = bex.CallsTonight;

        dorothy.Calls();
        dorothy.Calls();
        dorothy.Calls();

        Assert.True(dorothy.CallsTonight - dorothyBefore == 3,
            $"Dorothy rang three times and her count moved by "
            + $"{dorothy.CallsTonight - dorothyBefore}. Calls() adds one:\n"
            + "    public void Calls() { CallsTonight++; }\n"
            + "If it moved by 0, Calls() is still empty. If it moved by 1, it is setting "
            + "rather than adding.");

        // The night's actual claim, and the reason this file exists.
        Assert.True(bex.CallsTonight == bexBefore,
            $"Dorothy rang three times and BEX's count went up by "
            + $"{bex.CallsTonight - bexBefore}. Bex was not on the phone.\n"
            + "There is still one number shared by every caller in the program, which is "
            + "what `static` means: the field belongs to the CLASS, not to any one caller. "
            + "One copy, made once, alive as long as the program runs.\n"
            + "    private static int _calls;          // ← delete this line\n"
            + "    public int CallsTonight => _calls;  // ← and this one\n"
            + "    public int CallsTonight { get; private set; }   // ← this instead\n"
            + "Every caller then gets a count of their own, because every caller is a "
            + "separate object and an instance field is one per object.");

        var stranger = new Caller("Ray");
        Assert.True(stranger.CallsTonight == 0,
            $"A caller who has only just rung says they have called {stranger.CallsTonight} "
            + "times tonight. A brand-new Caller starts at 0 — if it doesn't, the count is "
            + "still coming from somewhere shared.");
    }

    // ── Check 3 — Task 3: Find, and what it says about a stranger ──────────

    [Fact]
    public void Check3_TheSwitchboardFindsARegular()
    {
        var switchboard = new Switchboard();
        var dorothy = new Caller("Dorothy");
        var teodoro = new Caller("Teodoro");
        switchboard.Add(dorothy);
        switchboard.Add(teodoro);

        Caller? found = switchboard.Find("Dorothy");

        Assert.True(found != null,
            "Find(\"Dorothy\") handed back null, and Dorothy is on the board.\n"
            + "It ships answering null to everything — walk the private list and hand back "
            + "the caller whose Name matches:\n"
            + "    foreach (Caller caller in _callers)\n"
            + "    {\n"
            + "        if (caller.Name == name) { return caller; }\n"
            + "    }\n"
            + "    return null;\n"
            + "👉 Next: write the loop in Switchboard.Find.");

        // The one that matters tonight: THE caller, not a caller like her.
        Assert.True(ReferenceEquals(found, dorothy),
            "Find(\"Dorothy\") handed back a Caller called Dorothy — but not the one on the "
            + "board. It is a second Dorothy, with her own count, and every call taken "
            + "through it lands on a record nobody is looking at.\n"
            + "Find hands back the caller it found. It never makes one:\n"
            + "    if (caller.Name == name) { return caller; }\n"
            + "Two names for one object is the whole of tonight — `found` and `dorothy` "
            + "have to be two names for the same person.");

        Assert.True(ReferenceEquals(switchboard.Find("Teodoro"), teodoro),
            "Find(\"Dorothy\") works but Find(\"Teodoro\") doesn't, so the loop is stopping "
            + "at the first caller instead of comparing every one of them. The `if` goes "
            + "INSIDE the foreach and the `return null` goes after it.");

        Assert.True(switchboard.Find("Halvorsen") == null,
            "Find(\"Halvorsen\") handed back a caller, and nobody by that name has ever rung "
            + "the desk.\n"
            + "When the loop finds nobody, the honest answer is `null` — which is exactly "
            + "why the return type is written `Caller?` and not `Caller`. The question mark "
            + "is you telling the compiler that this method is allowed to come back "
            + "empty-handed.\n"
            + "    return null;      // ← after the loop, not inside it");
    }

    // ── Check 4 — Task 4: the desk takes a call from anybody ───────────────

    [Fact]
    public void Check4_TheDeskTakesACallFromAnybody()
    {
        var switchboard = new Switchboard();
        var dorothy = new Caller("Dorothy");
        switchboard.Add(dorothy);

        int hers = dorothy.CallsTonight;

        Caller again = switchboard.Take("Dorothy");

        Assert.True(switchboard.Count == 1,
            $"Dorothy rang, and the switchboard now holds {switchboard.Count} callers. She "
            + "was already on it.\n"
            + "Take asks Find first. If Find hands back a caller, that is the one — use it "
            + "and do not add anybody:\n"
            + "    Caller? caller = Find(name);\n"
            + "    if (caller == null) { caller = new Caller(name); Add(caller); }\n"
            + "👉 Next: if this is red and check 3 is too, fix Find first — Take is built "
            + "on it.");

        Assert.True(ReferenceEquals(again, dorothy),
            "Take(\"Dorothy\") handed back a different Dorothy from the one on the board. "
            + "Every request she makes tonight is being written on a caller nobody can see, "
            + "and the board will never move.\n"
            + "It ships as `return new Caller(name);` — a brand-new stranger every single "
            + "time. Ask Find first, and only make one when Find comes back null.");

        Assert.True(again.CallsTonight - hers == 1,
            $"Dorothy rang once and her count moved by {again.CallsTonight - hers}. Taking a "
            + "call is a call, whoever it was.\n"
            + "Both roads through Take end in the same place, so Calls() goes AFTER the `if` "
            + "and not inside it — a regular who was already on the board never enters that "
            + "branch:\n"
            + "    if (caller == null) { caller = new Caller(name); Add(caller); }\n"
            + "    caller.Calls();        // ← outside the if\n"
            + "    return caller;");

        // Now somebody who has never rung.
        Caller ray = switchboard.Take("Ray");

        Assert.True(switchboard.Count == 2,
            $"Ray has never rung the desk before, and after his call the switchboard holds "
            + $"{switchboard.Count} caller(s) instead of 2. When Find comes back null this "
            + "really is somebody new — make a Caller and Add them to the board, or they "
            + "vanish the moment the method returns.");

        Assert.True(ray.Name == "Ray",
            $"Take(\"Ray\") handed back a caller called \"{ray.Name}\". Pass the name "
            + "straight through: new Caller(name).");

        Assert.True(ray.CallsTonight == 1,
            $"Ray's first ever call left him on {ray.CallsTonight}. Both roads through Take "
            + "end at the same place — the call gets counted whether they were already on "
            + "the board or not, so Calls() belongs after the `if`, not inside it.");

        Assert.True(ReferenceEquals(switchboard.Find("Ray"), ray),
            "Ray is on the board, but Find can't get him back out again.\n"
            + "Two things do this, so check them in this order:\n"
            + "  1. Find gives up too early — if check 3 is also red, that is the one, and "
            + "Ray is the SECOND caller Find has to walk past. Fix Find first; Take is "
            + "built on it.\n"
            + "  2. Take made two callers — one to Add and a different one to return. Make "
            + "it once, Add that one, return that same one.");
    }

    // ── Check 5 — Task 5: and remembers what they asked for ────────────────

    [Fact]
    public void Check5_AndRemembersWhatTheyAskedFor()
    {
        var favourite = PublicProperty(typeof(Caller), "Favourite");

        Assert.True(favourite != null,
            "Caller has no public property called Favourite. The switchboard's ASKED FOR "
            + "column reads it by that name.");

        Assert.True(favourite!.GetSetMethod(nonPublic: false) == null,
            "Favourite has a public setter, so anything anywhere can claim Bex asked for "
            + "something she never asked for.\n"
            + "    public Song? Favourite { get; private set; }\n"
            + "Asks() is the only way it moves.");

        var bex = new Caller("Bex");

        Assert.True(bex.Favourite == null,
            "A caller who has only just rung already has a Favourite. Before they ask for "
            + "anything there is nothing to report, and `null` is the right answer rather "
            + "than a made-up song:\n"
            + "    public Song? Favourite { get; private set; }\n"
            + "The `?` is what makes null a legal answer for this property.");

        var cart = ACart();
        int before = bex.CallsTonight;
        bex.Asks(cart);

        Assert.True(bex.Favourite != null,
            "Bex asked for something and Favourite is still null. Asks() stores the song it "
            + "was handed:\n"
            + "    Favourite = song;");

        Assert.True(ReferenceEquals(bex.Favourite, cart),
            "Bex's Favourite is a Song, but not the one she asked for — Asks() built a new "
            + "one instead of keeping the one it was handed.\n"
            + "    Favourite = song;\n"
            + "That matters more than it looks: the song she asked for is the cart in the "
            + "rotation, so when it plays, the play count she is looking at is the real "
            + "one. Copy it and she is looking at a photograph of it.");

        Assert.True(bex.CallsTonight == before,
            $"Bex asked for one song and her call count moved by "
            + $"{bex.CallsTonight - before}. It should not have moved at all in here.\n"
            + "Switchboard.Take already counted the call when it put her on the line, so "
            + "counting it again in Asks() puts every regular up by two for one phone call. "
            + "Asks() stores the song and nothing else:\n"
            + "    public void Asks(Song song) { Favourite = song; }\n"
            + "One rule, one place — the same reason Count asks the list instead of keeping "
            + "a number beside it.");

        // And the last public field in the class, if there is one left.
        Assert.True(PublicFieldNames(typeof(Caller)).Length == 0,
            $"Caller still has {PublicFieldNames(typeof(Caller)).Length} public field(s): "
            + $"{string.Join(", ", PublicFieldNames(typeof(Caller)))}.\n"
            + "That was last week's lesson and it still holds — a public field is a way in "
            + "that nothing is watching.");
    }
}
