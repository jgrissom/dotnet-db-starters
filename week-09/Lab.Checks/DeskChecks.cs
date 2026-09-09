// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-09/Lab.Checks
//
//  ⭐ CHECK 1 IS DIFFERENT THIS WEEK, AND IT IS THE POINT OF THE NIGHT.
//  It asserts everything the desk already did — including the seven
//  methods Task 1 rewrites. It is green before you start, and its job is
//  to still be green after you have deleted seven loops. Nothing else in
//  this course has ever asked you to change working code; check 1 and
//  your own suite are what make that a safe thing to do.
//
//  Checks 2 to 5 are the three questions the desk could not ask, and the
//  one it could only ask by putting the hour on the air.
//
//  ⚠️ Nothing in here greps output. Every assert is on a value or an
//  object's state — which is why you can print whatever you like.
//
//  ⚠️ Every value is fetched through Ask() below, because two of tonight's
//  likely rewrites THROW where the loop simply answered. A stack trace is
//  not a hint.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Checks;

public class DeskChecks
{
    // A scratch file of this check's own, in the folder the operating system
    // keeps for exactly this. Deleted first, so a previous run cannot pass a
    // check for you.
    private static string Scratch(string name)
    {
        string path = Path.Combine(Path.GetTempPath(), $"kdxr-check-{name}");
        File.Delete(path);
        return path;
    }

    // Call one of tonight's methods and turn a crash into something you can
    // read. Both of the traps below are ordinary first attempts, not blunders.
    private static T Ask<T>(string what, Func<T> question)
    {
        try
        {
            return question();
        }
        catch (Exception e)
        {
            throw new Xunit.Sdk.XunitException(
                $"{what} threw {e.GetType().Name} instead of answering:\n"
                + $"    {e.Message}\n"
                + "⚠️ Two rewrites throw where the loop you replaced did not:\n"
                + "  • First(), Last() and Single() OBJECT to an empty sequence. The\n"
                + "    ...OrDefault versions hand back null instead — which is what your\n"
                + "    loop did when it walked a list and found nothing.\n"
                + "  • MaxBy and MinBy hand back NULL for an empty list, so asking their\n"
                + "    answer for a .Name or a .Seconds straight off dies right here.\n"
                + "    Put a ?. in front of it, and say with ?? what to answer instead.\n"
                + "👉 Next: run the program and try the same thing by hand — the desk with "
                + "an empty hour, or a switchboard nobody has rung.");
        }
    }

    // Three carts, the same three as the desk, in a rotation of their own.
    private static Rotation ThreeCarts()
    {
        var rotation = new Rotation();
        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));
        rotation.Add(new Song("Slack Water", "Marguerite Vance", 252));
        rotation.Add(new Song("Long Way Round", "The Ferrymen", 331));
        return rotation;
    }

    private static string Titles(List<Song> songs) =>
        songs.Count == 0 ? "(nothing)" : string.Join(", ", songs.Select(s => s.Title));

    // ── Check 1 — everything the desk already did, and still has to ────────

    [Fact] // green before you start, and it has to STAY green
    public void Check1_TheDeskStillAnswers()
    {
        // ── the parts nothing this week touches ────────────────────────────

        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — that line ships finished and "
            + "nothing this week touches it. If this check is red, something in "
            + $"Broadcast.cs got changed by hand. (It returned: \"{Broadcast.CallSign()}\")");

        Assert.True(Broadcast.Clock(605) == "10:05",
            $"Broadcast.Clock(605) says \"{Broadcast.Clock(605)}\" and should say \"10:05\". "
            + "That is week 7's repair and it ships already made.");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. That door was shut in "
            + "week 4 and nothing this week goes near it.");

        var ad = new Ad("Pham's Bakery", "open at five", 1);
        ad.Play();
        ad.Play();
        Assert.True(ad.Remaining == 0,
            $"A one-run buy aired twice and Remaining is {ad.Remaining}. Week 7's guard, "
            + "and it ships already made.");

        var rotation = ThreeCarts();
        rotation.All().Clear();
        Assert.True(rotation.Count == 3,
            $"Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. All() hands back a COPY — week 4, and still the deal.\n"
            + "⚠️ Every query you write tonight hands back a new list for the same reason. "
            + "That is what the ToList() on the end of them is for.");

        // Week 8: the round trip, and the count that comes home with it.
        string path = Scratch("carried.json");
        var saved = ThreeCarts();
        saved.All()[0].Play();
        saved.All()[0].Play();
        saved.Save(path);

        var reopened = new Rotation();
        Ask("Rotation.Load", () => { reopened.Load(path); return 0; });

        Assert.True(reopened.Count == 3,
            $"Saved three carts and loaded {reopened.Count} back. Week 8's Save and Load, "
            + "and Task 1 only changes HOW Load fills the list — never how many go in it.");

        // ⚠️ Loading on top of a rotation that already holds carts, which is
        // exactly what Program.cs does: three added, then Load. An empty
        // rotation cannot tell you whether Clear() is still there.
        var already = ThreeCarts();
        Ask("Rotation.Load onto a rotation that already holds carts",
            () => { already.Load(path); return 0; });

        Assert.True(already.Count == 3,
            $"A rotation already holding 3 carts loaded a file holding 3 and now holds "
            + $"{already.Count}.\n"
            + "⚠️ Loading is REPLACING. The Clear() at the top of Load is what makes that "
            + "true, and if you swapped the foreach for AddRange and took the Clear() with "
            + "it, this is what happens — Program.cs adds three carts and then loads three "
            + "more on top of them.\n"
            + "👉 Next: put _songs.Clear(); back above the AddRange.");

        int carriedPlays = reopened.All().Sum(s => s.PlaysTonight);
        Assert.True(carriedPlays == 2,
            "A cart aired twice, saved and loaded, came back having aired "
            + $"{carriedPlays} times. That is week 8's [JsonInclude] on PlaysTonight, and "
            + "it ships already made.");

        string noLog = Ask("Broadcast.LastShift", () => Broadcast.LastShift(Scratch("no-such.txt")));
        Assert.True(noLog == "",
            "Broadcast.LastShift() should hand back an empty string for a log that isn't "
            + $"there, and it gave \"{noLog}\".\n"
            + "⚠️ Task 1 rewrites this method. The File.Exists guard at the top STAYS — "
            + "File.ReadAllLines throws on a missing file, so LINQ never gets asked "
            + "anything at all.");

        // ── the seven Task 1 rewrites, asserted ───────────────────────────
        // Every one of these was green before you touched it. If one has gone
        // red, the rewrite changed the answer — which is exactly what this
        // check exists to tell you.

        var hour = new Hour();
        hour.Add(new StationId("KDXR 88.1, The Owl"));           // 12
        hour.Add(new Song("Nightjar", "The Lamplighters", 227)); // 227
        hour.Add(new Ad("Pham's Bakery", "open at five", 3));    // 30
        hour.Add(new WeatherBed("clear, four below"));           // 45

        int hourSeconds = Ask("Hour.TotalSeconds", () => hour.TotalSeconds);
        Assert.True(hourSeconds == 314,
            $"Hour.TotalSeconds says {hourSeconds} for an hour holding 12 + 227 + 30 + 45 "
            + "seconds, and it should say 314.\n"
            + "This was a running total, a foreach and a += — and it was RIGHT. If it went "
            + "red just now, the one-liner is adding up something other than Seconds:\n"
            + "    public int TotalSeconds => _items.Sum(item => item.Seconds);\n"
            + "👉 Next: read the lambda. The name before the => is each item in turn; what "
            + "comes after it is the thing to add up about that item.");

        int rotationSeconds = Ask("Rotation.TotalSeconds", () => ThreeCarts().TotalSeconds);
        Assert.True(rotationSeconds == 810,
            $"Rotation.TotalSeconds says {rotationSeconds} for 227 + 252 + 331 seconds, and "
            + "it should say 810. Same line as Hour's, different nouns.");

        var empty = new Hour();
        int emptySeconds = Ask("Hour.TotalSeconds on an empty hour", () => empty.TotalSeconds);
        Assert.True(emptySeconds == 0,
            $"An empty hour says {emptySeconds} seconds long and should say 0. Sum over "
            + "nothing is 0 — you do not have to guard for it.");

        IScheduleItem? longest = Ask("Hour.LongestItem", () => hour.LongestItem());
        Assert.True(longest != null && longest.Seconds == 227,
            "Hour.LongestItem() should hand back the 227-second song out of an hour holding "
            + $"12, 227, 30 and 45 — it gave "
            + $"{(longest == null ? "null" : longest.Seconds + " seconds")}.\n"
            + "    public IScheduleItem? LongestItem() => _items.MaxBy(item => item.Seconds);\n"
            + "👉 Next: MaxBy hands back the ITEM. Max would hand back the number, and then "
            + "you could not ask it for its Cue.");

        IScheduleItem? nothingLongest = Ask("Hour.LongestItem on an empty hour",
            () => empty.LongestItem());
        Assert.True(nothingLongest == null,
            "The longest thing in an empty hour is null, and LongestItem() handed back "
            + "something instead.\n"
            + "MaxBy on an empty list answers null rather than throwing — which is why this "
            + "method's return type has a ? on the end of it, and why OrderByDescending "
            + "followed by First() is the wrong shape here.");

        var board = new Switchboard();
        board.Take("Dorothy");
        board.Take("Bex");
        board.Take("Dorothy");

        int calls = Ask("Switchboard.TotalCalls", () => board.TotalCalls);
        Assert.True(calls == 3,
            $"Switchboard.TotalCalls says {calls} after three calls and should say 3. Third "
            + "Sum in the program, same line as the other two.");

        string regular = Ask("Switchboard.TheRegular", () => board.TheRegular());
        Assert.True(regular == "Dorothy",
            $"Switchboard.TheRegular() says \"{regular}\" after Dorothy rang twice and Bex "
            + "once. It should say \"Dorothy\".\n"
            + "This is the method you wrote in WEEK 3, over a dictionary. Same question:\n"
            + "    _callers.MaxBy(caller => caller.CallsTonight)?.Name ?? \"nobody yet\"");

        var quiet = new Switchboard();
        string nobody = Ask("Switchboard.TheRegular on a board nobody has rung",
            () => quiet.TheRegular());
        Assert.True(nobody == "nobody yet",
            $"Switchboard.TheRegular() says \"{nobody}\" on a board nobody has rung, and it "
            + "has to say \"nobody yet\" — exactly that, the same string it has said since "
            + "week 3.\n"
            + "⚠️ MaxBy on an empty list hands back NULL, so asking it for a .Name straight "
            + "off throws. Two operators do the whole job:\n"
            + "    _callers.MaxBy(caller => caller.CallsTonight)?.Name ?? \"nobody yet\"\n"
            + "  ?.  don't ask a nothing for its Name\n"
            + "  ??  and when there is nothing, say this instead\n"
            + "👉 Next: that is what `string best = \"nobody yet\";` was doing before the "
            + "loop, and it still has to be said somewhere.");

        Assert.True(board.Find("Dorothy") != null
                    && ReferenceEquals(board.Find("Dorothy"), board.All()[0]),
            "Switchboard.Find no longer hands back the caller the board is holding. Week 5, "
            + "and nothing this week touches Find.");
    }

    // ── Check 2 — Task 2: a cart long enough to cover the news ─────────────

    [Fact]
    public void Check2_TheDeskFindsALongCart()
    {
        var rotation = ThreeCarts();

        List<Song> long240 = Ask("Rotation.LongerThan(240)", () => rotation.LongerThan(240));

        Assert.True(long240.Count == 2,
            $"Rotation.LongerThan(240) handed back {long240.Count} carts and should hand "
            + "back 2. The rotation holds 227, 252 and 331 seconds — two of those are "
            + $"longer than 240. (It gave: {Titles(long240)})\n"
            + "    public List<Song> LongerThan(int seconds)\n"
            + "    {\n"
            + "        return _songs.Where(song => song.Seconds > seconds).ToList();\n"
            + "    }\n"
            + "Where keeps the ones the question is TRUE for and drops the rest.\n"
            + "👉 Next: if you got 3, the comparison is the wrong way round. If you got 0, "
            + "the method is still handing back the empty list it ships with.");

        Assert.True(long240.All(s => s.Seconds > 240),
            "Rotation.LongerThan(240) handed back a cart that is 240 seconds or shorter: "
            + $"{Titles(long240.Where(s => s.Seconds <= 240).ToList())}. Read the "
            + "comparison inside the lambda.");

        Assert.True(long240.Select(s => s.Title)
                .SequenceEqual(new[] { "Slack Water", "Long Way Round" }),
            $"Rotation.LongerThan(240) gave {Titles(long240)} and the rotation's own order "
            + "is Nightjar, Slack Water, Long Way Round — so it should give Slack Water, "
            + "Long Way Round.\n"
            + "Where does not reorder anything. It keeps what is left in the order it found "
            + "it, which is worth knowing before you meet OrderBy in Task 4.");

        // The boundary, and it is the only assert in here that can tell > from >=.
        // Nightjar is exactly 227 seconds long.
        List<Song> over227 = Ask("Rotation.LongerThan(227)", () => rotation.LongerThan(227));
        Assert.True(over227.Count == 2 && !over227.Any(s => s.Title == "Nightjar"),
            $"Rotation.LongerThan(227) gave {Titles(over227)} and should give Slack Water, "
            + "Long Way Round.\n"
            + "⚠️ Nightjar is EXACTLY 227 seconds long, and a cart that is exactly 227 "
            + "seconds is not longer than 227. That is the whole difference between > and "
            + ">=, and asking at the boundary is the only way to see it — at 240 the two "
            + "spellings give the same answer and both look right.\n"
            + "👉 Next: read the comparison in your lambda.");

        int all = Ask("Rotation.LongerThan(0)", () => rotation.LongerThan(0).Count);
        int none = Ask("Rotation.LongerThan(999)", () => rotation.LongerThan(999).Count);
        Assert.True(all == 3 && none == 0,
            $"LongerThan(0) gave {all} and LongerThan(999) gave {none}; they should give 3 "
            + "and 0. A question that is true of everything keeps everything, and one that "
            + "is true of nothing hands back an empty list — not null, and not an error.");

        Assert.True(rotation.Count == 3,
            $"Asking LongerThan changed the rotation — it holds {rotation.Count} carts now "
            + "and it held 3 before.\n"
            + "⚠️ A query ASKS. It never changes the thing it is asking about. Where builds "
            + "a new sequence, ToList() copies that into a new list, and _songs is untouched "
            + "by both.");
    }

    // ── Check 3 — Task 3: what has not been out tonight ────────────────────

    [Fact]
    public void Check3_TheDeskKnowsWhatHasNotBeenOut()
    {
        var rotation = ThreeCarts();

        int coldAtStart = Ask("Rotation.NeverPlayed", () => rotation.NeverPlayed().Count);
        Assert.True(coldAtStart == 3,
            $"Nothing has aired and Rotation.NeverPlayed() found {coldAtStart} carts that "
            + "have not been out. All three of them have not been out.\n"
            + "This is Task 2's line with a different question in it — one list, one Where, "
            + "ToList() on the end.\n"
            + "👉 Next: the question is about PlaysTonight, and \"never\" is 0.");

        List<Song> carts = rotation.All();
        carts[0].Play();
        carts[2].Play();
        carts[2].Play();

        List<Song> cold = Ask("Rotation.NeverPlayed", () => rotation.NeverPlayed());

        Assert.True(cold.Count == 1,
            $"Two of the three carts have aired and NeverPlayed() found {cold.Count} that "
            + $"have not. It should find 1. (It gave: {Titles(cold)})\n"
            + "⚠️ Read the comparison. == 0 is \"never been out\"; > 0 is the opposite "
            + "answer, and that is also a list of carts — which is why this one is worth "
            + "checking rather than eyeballing.");

        Assert.True(cold[0].Title == "Slack Water",
            $"NeverPlayed() gave {Titles(cold)}, and the cart nobody has aired is Slack "
            + "Water — Nightjar went out once and Long Way Round went out twice.");

        Assert.True(cold.All(s => s.PlaysTonight == 0),
            "NeverPlayed() handed back a cart that HAS been out: "
            + $"{Titles(cold.Where(s => s.PlaysTonight > 0).ToList())}.");

        // The same carts, so a play counted here is the play the rotation sees.
        Assert.True(ReferenceEquals(cold[0], carts[1]),
            "NeverPlayed() handed back a cart the rotation is not holding — a copy, or a "
            + "new Song built from one.\n"
            + "A query hands back the THINGS, not copies of them. That is week 5's rule, "
            + "and it is what makes the numbers on this board add up: air the cart this "
            + "handed you and the rotation's own count moves.");
    }

    // ── Check 4 — Task 4: what the desk worked hardest ─────────────────────

    [Fact]
    public void Check4_TheDeskKnowsWhatItPlayedMost()
    {
        var rotation = ThreeCarts();
        List<Song> carts = rotation.All();

        // Nightjar 3, Long Way Round 1, Slack Water 0.
        carts[0].Play();
        carts[0].Play();
        carts[0].Play();
        carts[2].Play();

        List<Song> top = Ask("Rotation.TopPlayed(2)", () => rotation.TopPlayed(2));

        Assert.True(top.Count == 2,
            $"Rotation.TopPlayed(2) handed back {top.Count} carts and was asked for 2. "
            + $"(It gave: {Titles(top)})\n"
            + "    public List<Song> TopPlayed(int n)\n"
            + "    {\n"
            + "        return _songs.OrderByDescending(song => song.PlaysTonight)\n"
            + "            .Take(n).ToList();\n"
            + "    }\n"
            + "Three steps, read left to right: put them in order, stop after n, hand back "
            + "a list.\n"
            + "👉 Next: if you got 3, Take isn't there. If you got 0, the method is still "
            + "handing back the empty list it ships with.");

        Assert.True(top[0].Title == "Nightjar",
            $"TopPlayed(2) put {top[0].Title} first. Nightjar has aired 3 times, Long Way "
            + "Round once and Slack Water never — so the hardest-worked cart is Nightjar.\n"
            + "⚠️ If Slack Water came first, it is OrderBy rather than OrderByDescending. "
            + "OrderBy puts the smallest first, which is the quietest cart rather than the "
            + "busiest one.");

        Assert.True(top[1].Title == "Long Way Round",
            $"TopPlayed(2) gave {Titles(top)}. After Nightjar (3 airings) comes Long Way "
            + "Round (1), not Slack Water (0).");

        int more = Ask("Rotation.TopPlayed(99)", () => rotation.TopPlayed(99).Count);
        Assert.True(more == 3,
            $"TopPlayed(99) handed back {more} carts out of a rotation holding 3. Asking "
            + "Take for more than there are is not an error — it gives you everything it "
            + "has and stops.");

        int zero = Ask("Rotation.TopPlayed(0)", () => rotation.TopPlayed(0).Count);
        Assert.True(zero == 0,
            $"TopPlayed(0) handed back {zero} carts and should hand back none.");

        // ⚠️ The one this check really exists for.
        Assert.True(rotation.All().Select(s => s.Title)
                .SequenceEqual(new[] { "Nightjar", "Slack Water", "Long Way Round" }),
            "Sorting the rotation SORTED THE ROTATION. It is holding "
            + $"{Titles(rotation.All())} and it was loaded as Nightjar, Slack Water, Long "
            + "Way Round.\n"
            + "⚠️ OrderByDescending sorts a copy and hands the copy back. Whatever you did "
            + "reached the list itself — probably List.Sort, which rearranges in place.\n"
            + "That distinction is the one to keep from tonight: every query in this file "
            + "answers a question and leaves the thing alone. The rotation's own order is "
            + "the order the carts were loaded in, and something else in this program is "
            + "entitled to rely on that.\n"
            + "👉 Next: OrderByDescending, and let it build you a new sequence.");
    }

    // ── Check 5 — Task 5: reading the hour without airing it ───────────────

    [Fact]
    public void Check5_TheHourReadsWithoutAiring()
    {
        var ident = new StationId("KDXR 88.1, The Owl");
        var nightjar = new Song("Nightjar", "The Lamplighters", 227);
        var spot = new Ad("Pham's Bakery", "open at five", 3);
        var bed = new WeatherBed("clear, four below");

        var hour = new Hour();
        hour.Add(ident);
        hour.Add(nightjar);
        hour.Add(spot);
        hour.Add(bed);

        List<string> order = Ask("Hour.RunningOrder", () => hour.RunningOrder());

        Assert.True(order.Count == 4,
            $"Hour.RunningOrder() handed back {order.Count} lines for an hour holding 4 "
            + "items. One line per item, in the hour's own order.\n"
            + "    public List<string> RunningOrder()\n"
            + "    {\n"
            + "        return _items.Select(item => $\"{item.Kind} - {item.Cue}\").ToList();\n"
            + "    }\n"
            + "Where keeps SOME of the things. Select keeps all of them and turns each one "
            + "into something else — here, into a string.\n"
            + "👉 Next: if you got 0, the method is still handing back the empty list it "
            + "ships with.");

        Assert.True(order[0] == "IDENT - KDXR 88.1, The Owl",
            $"RunningOrder()'s first line is \"{order[0]}\" and should be\n"
            + "    \"IDENT - KDXR 88.1, The Owl\"\n"
            + "The same line Run() hands back: the item's Kind, then a space, a hyphen, a "
            + "space, and its Cue.");

        Assert.True(order[1] == "SONG - Nightjar - The Lamplighters",
            $"RunningOrder()'s second line is \"{order[1]}\" and should be\n"
            + "    \"SONG - Nightjar - The Lamplighters\"");

        // ⚠️ The whole reason this method exists.
        Assert.True(ident.TimesAired == 0 && nightjar.PlaysTonight == 0
                    && spot.Remaining == 3 && !bed.Aired,
            "Reading the running order PUT THE HOUR ON THE AIR. After RunningOrder() the "
            + $"ident has aired {ident.TimesAired} times, the song {nightjar.PlaysTonight}, "
            + $"the buy has {spot.Remaining} runs left and the weather bed "
            + $"{(bed.Aired ? "has" : "has not")} been read.\n"
            + "Every one of those should be exactly where it was: 0, 0, 3, and not read.\n"
            + "⚠️ You have called Play() inside the lambda. That is the whole difference "
            + "between this method and Run(), and it is the difference between asking and "
            + "doing — the DJ reads the running order at three minutes to the hour, and "
            + "nothing goes out over the transmitter when they do.\n"
            + "👉 Next: take Play() out of the Select. Run() is where it belongs, and Run() "
            + "already has it.");

        // And it agrees with itself — same hour, same answer, every time.
        var second = new Hour();
        second.Add(new StationId("KDXR 88.1, The Owl"));
        second.Add(new Song("Nightjar", "The Lamplighters", 227));
        second.Add(new Ad("Pham's Bakery", "open at five", 3));
        second.Add(new WeatherBed("clear, four below"));

        List<string> again = Ask("Hour.RunningOrder", () => second.RunningOrder());
        Assert.True(again.SequenceEqual(order),
            "Two identical hours gave different running orders, which means something in "
            + "RunningOrder() is reading state that moves while it reads it.");

        Assert.True(hour.Count == 4,
            $"Asking for the running order changed the hour — it holds {hour.Count} items "
            + "and it held 4.");
    }
}
