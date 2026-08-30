// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-06/Lab.Checks
//
//  ⚠️ Worth noticing what these checks DON'T do. Not one of them looks at
//  what your Cue says word for word, and not one of them looks for a
//  property called TimesAired. They ask whether the promise is kept:
//  can this thing say what kind it is, how long it runs, and does
//  playing it change something about that thing and nothing else.
//
//  That is what an interface is, and it is what a check is. Week 7.
// ═══════════════════════════════════════════════════════════════════
using System.Reflection;

namespace Lab.Checks;

public class HourChecks
{
    // ── plumbing ───────────────────────────────────────────────────────────

    private static Song ACart() => new Song("Nightjar", "The Lamplighters", 227);

    private static WeatherBed ABed() =>
        new WeatherBed("clear, four below, wind out of the northwest");

    // Nothing in Lab/ implements IScheduleItem until you say so, so these
    // checks ask at run time rather than at compile time.
    private static bool Keeps(Type type) => typeof(IScheduleItem).IsAssignableFrom(type);

    private static IScheduleItem AsItem(object thing, string name, int task)
    {
        Assert.True(thing is IScheduleItem,
            $"{name} does not keep IScheduleItem's promise yet, so the hour cannot hold "
            + "it. One phrase, after the class name:\n"
            + $"    public class {name} : IScheduleItem\n"
            + "Build it as soon as you have typed that. The compiler answers with one "
            + "CS0535 per member you still owe, which is the most useful to-do list you "
            + "will get all night.\n"
            + $"👉 Next: Task {task}.");

        return (IScheduleItem)thing;
    }

    private static PropertyInfo[] IntProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(int) && p.GetIndexParameters().Length == 0)
            .ToArray();

    // Which public int properties moved, and by how much, when Play() ran.
    private static Dictionary<string, int> Moved(object thing)
    {
        var type = thing.GetType();
        var before = IntProperties(type).ToDictionary(p => p.Name, p => (int)p.GetValue(thing)!);

        ((IScheduleItem)thing).Play();

        var moved = new Dictionary<string, int>();
        foreach (var p in IntProperties(type))
        {
            int now = (int)p.GetValue(thing)!;
            if (now != before[p.Name]) { moved[p.Name] = now - before[p.Name]; }
        }

        return moved;
    }

    // ── Check 1 — weeks 1-5, shipped finished ──────────────────────────────

    [Fact] // passes out of the box
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — Broadcast.cs ships finished, so "
            + $"if this check is red something in it got changed. (It returned: "
            + $"\"{Broadcast.CallSign()}\")");

        Assert.True(Broadcast.Clock(893) == "14:53",
            $"Broadcast.Clock(893) says \"{Broadcast.Clock(893)}\" and should say \"14:53\". "
            + "It ships finished — it is Song.Length's old body, moved somewhere four "
            + "different kinds of thing can reach it.");

        var song = ACart();
        Assert.True(song.Length == "3:47",
            $"Song.Length says \"{song.Length}\" for a 227-second cart and it should say "
            + "\"3:47\". Everything above Task 2's two members in Song.cs is week 4's and "
            + "ships untouched — undo whatever changed in it.");

        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. That door was shut in "
            + "week 4 and it stays shut.");

        var rotation = new Rotation();
        rotation.Add(ACart());
        rotation.All().Clear();
        Assert.True(rotation.Count == 1,
            $"Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. Rotation.cs ships finished — All() hands back a copy.");

        var switchboard = new Switchboard();
        var dorothy = new Caller("Dorothy");
        switchboard.Add(dorothy);
        Assert.True(ReferenceEquals(switchboard.Take("Dorothy"), dorothy)
                    && switchboard.Count == 1,
            "Switchboard.Take(\"Dorothy\") no longer hands back the Dorothy already on the "
            + "board. That is last week's work and it ships finished this week — the lab "
            + "folder carries your switchboard forward rather than starting it again.");

        // The worked example, and the reason you have one to read.
        var bed = ABed();
        Assert.True(Keeps(typeof(WeatherBed)) && bed.Seconds == 45 && bed.Kind == "WEATHER",
            "WeatherBed.cs ships finished and is the worked example for Tasks 2, 3 and 4 — "
            + "the whole shape, done once. Undo whatever changed in it and read it instead.");
    }

    // ── Check 2 — Task 2: a song can be scheduled ──────────────────────────

    [Fact]
    public void Check2_ASongKnowsHowToBeScheduled()
    {
        var song = ACart();

        Assert.True(Keeps(typeof(Song)),
            "Song no longer says `: IScheduleItem` after the class name. It ships that way "
            + "— the two members underneath are Task 2, not the phrase itself.");

        Assert.True(song.Kind != "?" && !string.IsNullOrWhiteSpace(song.Kind),
            $"Song.Kind still says \"{song.Kind}\", which is what it ships saying. The KIND "
            + "column on the hour reads this, and every kind of thing needs its own word:\n"
            + "    public string Kind => \"SONG\";\n"
            + "👉 Next: one line, in Song.cs.");

        Assert.True(song.Cue.Contains("Nightjar") && song.Cue.Contains("The Lamplighters"),
            $"Song.Cue says \"{song.Cue}\". The DJ reads that off the screen at four in the "
            + "morning, so it has to name the track AND who it is by — the wording and the "
            + "punctuation between them are yours:\n"
            + "    public string Cue => $\"{Title} - {Artist}\";\n"
            + "⚠️ Read Title and Artist rather than the fields behind them. They are the "
            + "properties for a reason and it is still week 4's reason.");

        Assert.True(song.Seconds == 227,
            $"Song.Seconds says {song.Seconds} for a 227-second cart. The promise asks for a "
            + "number of seconds and Song has had one since week 4 — this member needs "
            + "nothing done to it at all.");

        int before = song.PlaysTonight;
        song.Play();
        Assert.True(song.PlaysTonight - before == 1,
            $"Playing a song moved PlaysTonight by {song.PlaysTonight - before}. Play() was "
            + "already here and already right in week 4 — the promise asked for a method "
            + "called Play that takes nothing and returns nothing, and this was already it.");
    }

    // ── Check 3 — Task 3: the ident is an item too ─────────────────────────

    [Fact]
    public void Check3_TheIdentIsAnItemToo()
    {
        object thing = new StationId("KDXR 88.1, The Owl");
        IScheduleItem ident = AsItem(thing, "StationId", 3);

        Assert.True(!string.IsNullOrWhiteSpace(ident.Kind) && ident.Kind != "?",
            $"StationId.Kind says \"{ident.Kind}\". One word for the KIND column, and it "
            + "cannot be the same word a song uses — telling them apart on the screen is "
            + "the entire reason that column exists.");

        Assert.True(ident.Kind != ACart().Kind,
            $"StationId and Song both call themselves \"{ident.Kind}\". Four kinds of thing "
            + "go through one loop tonight and the KIND column is how the DJ tells them "
            + "apart at four in the morning.");

        Assert.True(ident.Cue.Contains("KDXR"),
            $"StationId.Cue says \"{ident.Cue}\" — and the words it was handed were "
            + "\"KDXR 88.1, The Owl\". The cue is what the DJ reads out, so it has to be "
            + "the words:\n"
            + "    public string Cue => Words;");

        Assert.True(ident.Seconds == 12,
            $"StationId.Seconds says {ident.Seconds} and an ident takes 12. Nothing was "
            + "handed in to the constructor for it, and nothing needs to be — it is worked "
            + "out rather than stored, exactly like Song.Length:\n"
            + "    public int Seconds => 12;\n"
            + "The hour never asks where the number came from. That is the promise doing "
            + "its job.");

        var moved = Moved(thing);

        Assert.True(moved.Count > 0,
            "Playing the ident changed nothing about it. Every kind of thing on the hour "
            + "counts an airing in its own way, and an ident counts up — one number, "
            + "readable anywhere, moved by Play() and by nothing else:\n"
            + "    public int TimesAired { get; private set; }\n"
            + "\n"
            + "    public void Play() { TimesAired++; }\n"
            + "Call it whatever you like; I never look at the name.");

        Assert.True(moved.Count == 1 && moved.Values.First() == 1,
            "Playing the ident once should move exactly one number, by exactly one. It "
            + $"moved: {string.Join(", ", moved.Select(m => $"{m.Key} by {m.Value}"))}.");
    }

    // ── Check 4 — Task 4: and so is the ad ─────────────────────────────────

    [Fact]
    public void Check4_AndSoIsTheAd()
    {
        object thing = new Ad("Pham's Bakery", "open at five", 3);
        IScheduleItem ad = AsItem(thing, "Ad", 4);

        Assert.True(!string.IsNullOrWhiteSpace(ad.Kind)
                    && ad.Kind != "?"
                    && ad.Kind != ACart().Kind
                    && ad.Kind != ABed().Kind,
            $"Ad.Kind says \"{ad.Kind}\", which is either the shipped placeholder or a word "
            + "another kind of item is already using. Four kinds, four words.");

        Assert.True(ad.Cue.Contains("Pham's Bakery"),
            $"Ad.Cue says \"{ad.Cue}\" and it has to name the sponsor — an ad the DJ can't "
            + "attribute is an ad the station gets to run again for free.");

        Assert.True(ad.Seconds == 30,
            $"Ad.Seconds says {ad.Seconds}. A spot is thirty seconds, and has been since "
            + "radio began:\n"
            + "    public int Seconds => 30;");

        var moved = Moved(thing);

        Assert.True(moved.ContainsKey("Remaining"),
            "Airing the ad left Remaining exactly where it was. A buy is three runs and "
            + "every airing spends one:\n"
            + "    public void Play()\n"
            + "    {\n"
            + "        if (Remaining > 0) { Remaining--; }\n"
            + "    }\n"
            + $"(What did move: {(moved.Count == 0 ? "nothing" : string.Join(", ", moved.Keys))})");

        Assert.True(moved["Remaining"] == -1,
            $"Airing the ad once moved Remaining by {moved["Remaining"]}. This is the one "
            + "that counts the other way — the song's count goes up, the buy comes down. "
            + "The hour calls the same Play() on both and never finds out.");

        var spent = new Ad("Pham's Bakery", "open at five", 1);
        var item = (IScheduleItem)spent;
        item.Play();
        item.Play();
        item.Play();

        Assert.True(spent.Remaining == 0,
            $"A one-run buy aired three times and Remaining is now {spent.Remaining}. A "
            + "station that owes minus two spots has a bug:\n"
            + "        if (Remaining > 0) { Remaining--; }\n"
            + "The guard is the whole difference between a counter and a promise you keep.");
    }

    // ── Check 5 — Task 5: the hour runs itself ─────────────────────────────

    [Fact]
    public void Check5_TheHourRunsItself()
    {
        Assert.True(Keeps(typeof(StationId)) && Keeps(typeof(Ad)),
            "The hour needs all four kinds before it can prove anything, and "
            + $"{(Keeps(typeof(StationId)) ? "Ad" : Keeps(typeof(Ad)) ? "StationId" : "StationId and Ad")} "
            + "still keeps no promise.\n"
            + "👉 Next: Tasks 3 and 4. This check isn't failing so much as waiting.");

        var song = ACart();
        var bed = ABed();
        object ident = new StationId("KDXR 88.1, The Owl");
        object ad = new Ad("Pham's Bakery", "open at five", 3);

        var hour = new Hour();
        hour.Add((IScheduleItem)ident);
        hour.Add(song);
        hour.Add((IScheduleItem)ad);
        hour.Add(bed);

        Assert.True(hour.TotalSeconds == 12 + 227 + 30 + 45,
            $"The hour holds an ident, a song, an ad and a weather bed — 12 + 227 + 30 + 45 "
            + $"seconds of radio — and TotalSeconds says {hour.TotalSeconds}. It ships "
            + "saying 0:\n"
            + "    int total = 0;\n"
            + "    foreach (IScheduleItem item in _items) { total += item.Seconds; }\n"
            + "    return total;\n"
            + "👉 Next: TotalSeconds, in Hour.cs. Nothing in that loop knows what a song is.");

        var aired = hour.Run();

        Assert.True(aired.Count == 4,
            $"Running an hour of four items handed back {aired.Count} line(s). One line per "
            + "item, in the order they are scheduled — that is what the desk prints.");

        Assert.True(song.PlaysTonight == 1,
            $"The hour ran and the song's PlaysTonight is {song.PlaysTonight}. Run() has to "
            + "actually Play() each item, not just describe it. Every kind counts that "
            + "differently and this loop never finds out how.");

        Assert.True(bed.Aired && ((Ad)ad).Remaining == 2,
            "The hour ran and the song moved, but the weather bed and the ad did not — so "
            + "the loop is doing something to songs specifically instead of asking every "
            + "item the same question. One foreach over IScheduleItem, one Play() call.");

        Assert.True(aired[1].StartsWith(song.Kind) && aired[1].Contains(song.Cue),
            $"The second line back was \"{aired[1]}\" and the second item on the hour was a "
            + $"song whose Kind is \"{song.Kind}\" and whose Cue is \"{song.Cue}\". Each "
            + "line is the kind, then the cue:\n"
            + "        aired.Add($\"{item.Kind} - {item.Cue}\");");

        // The ad's cue has to move when it airs — the lab asks for that outright,
        // because it is the only thing on screen that proves the read order.
        // ⚠️ Compared against the student's OWN wording, never against mine.
        var unaired = (IScheduleItem)new Ad("Pham's Bakery", "open at five", 3);
        string before = unaired.Cue;
        string after = ((IScheduleItem)ad).Cue;

        Assert.True(before != after,
            $"Ad.Cue says \"{after}\" both before and after the spot airs. It has to show "
            + "how many runs are left on the buy — that is what the desk prints, and it is "
            + "the only thing on the screen that says an airing actually happened:\n"
            + "    public string Cue => $\"{Sponsor} - \\\"{Copy}\\\" ({Remaining} left)\";\n"
            + "The wording is yours. The count isn't optional.");

        Assert.True(aired[2].Contains(after) && !aired[2].Contains(before),
            $"The third line back was \"{aired[2]}\" — the ad, just after it aired. Its cue "
            + $"reads \"{before}\" before an airing and \"{after}\" after one, and the line "
            + "the desk printed is the BEFORE one.\n"
            + "Play() the item, then read its Cue. The desk prints what happened, not what "
            + "was about to:\n"
            + "        item.Play();\n"
            + "        aired.Add($\"{item.Kind} - {item.Cue}\");");

        hour.All().Clear();
        Assert.True(hour.Count == 4,
            $"Somebody emptied the list Hour.All() handed them and the hour went to "
            + $"{hour.Count}. All() ships finished and hands back a copy — same as Rotation, "
            + "same as Switchboard, same reason as week 4.");
    }
}
