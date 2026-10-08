// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-09/Lab.Checks
//
//  ⭐ CHECK 1 IS THE ONE TO READ TONIGHT. It asserts everything the desk
//  already does — including Rotation.Find and Rotation.Load, which Task 1
//  has you rewrite. It is green before you start, and its job is to still
//  be green when you have finished. Check 1 and your own suite are what
//  make changing working code a safe thing to do.
//
//  Checks 2, 3 and 4 are three questions the rotation cannot answer yet.
//
//  ⚠️ Nothing in here reads what the desk prints. Every assert is on a
//  value or on an object's state, so you can print whatever you like.
//
//  ⚠️ Every answer is fetched through Ask() below, because one of
//  tonight's likely rewrites THROWS where the loop simply answered. A
//  stack trace is not a hint.
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
    // read.
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
                + "⚠️ First(), Last() and Single() OBJECT when there is nothing to hand "
                + "back. The ...OrDefault versions hand back null instead — which is what "
                + "the loop did when it walked the list and found nothing.\n"
                + "👉 Next: if this is Find, it is First where FirstOrDefault belongs. Run "
                + "the desk, press f, and ask for a title that is not there.");
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

    private static string Words(List<string> words) =>
        words.Count == 0 ? "(nothing)" : string.Join(", ", words.Select(w => $"\"{w}\""));

    // ── Check 1 — everything the desk already does, and still has to ───────

    [Fact] // green before you start, and it has to STAY green
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — that line ships finished and "
            + "nothing this week touches it. If this check is red, something in "
            + $"Broadcast.cs got changed by hand. (It returned: \"{Broadcast.CallSign()}\")");

        Assert.True(Broadcast.Clock(605) == "10:05",
            $"Broadcast.Clock(605) says \"{Broadcast.Clock(605)}\" and should say \"10:05\". "
            + "That method ships finished — undo whatever changed in it.");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. Song.cs ships finished "
            + "and nothing this week touches it.");

        var rotation = ThreeCarts();
        rotation.All().Clear();
        Assert.True(rotation.Count == 3,
            "Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. All() hands back a copy, and it ships finished.");

        // ── Find, which Task 1 rewrites ────────────────────────────────────

        Song? found = Ask("Rotation.Find(\"Slack Water\")", () => rotation.Find("Slack Water"));

        Assert.True(found != null && ReferenceEquals(found, rotation.All()[1]),
            "Rotation.Find(\"Slack Water\") did not hand back the Slack Water the rotation "
            + $"is holding. It handed back: {(found == null ? "nothing" : found.Title)}.\n"
            + "Find hands back the cart ITSELF — the same object, never a copy — for the "
            + "title it was asked for:\n"
            + "    return _songs.FirstOrDefault(song => song.Title == title);\n"
            + "👉 Next: read the question inside the brackets. It compares the song's Title "
            + "with the title Find was handed.");

        Song? missing = Ask("Rotation.Find(\"No Such Cart\")", () => rotation.Find("No Such Cart"));

        Assert.True(missing == null,
            "Rotation.Find was asked for a title no cart has, and it handed back "
            + $"\"{missing?.Title}\" instead of nothing. A title that is not in the "
            + "rotation gets null.");

        var empty = new Rotation();
        Song? fromEmpty = Ask("Rotation.Find on an empty rotation", () => empty.Find("Nightjar"));

        Assert.True(fromEmpty == null,
            "Rotation.Find on an empty rotation should hand back nothing.");

        // ── Save and Load, and Task 1 rewrites the bottom of Load ──────────

        string path = Scratch("still-answers.json");

        var saved = new Rotation();
        var nightjar = new Song("Nightjar", "The Lamplighters", 227);
        nightjar.Play();
        nightjar.Play();
        saved.Add(nightjar);
        saved.Add(new Song("Slack Water", "Marguerite Vance", 252));
        saved.Save(path);

        var reopened = ThreeCarts();
        Ask("Rotation.Load", () => { reopened.Load(path); return 0; });

        Assert.True(reopened.Count != 5,
            "A rotation already holding 3 carts loaded a file of 2 and now holds 5. The "
            + "loaded carts went in ON TOP of the ones already there.\n"
            + "⚠️ Loading is replacing. The _songs.Clear() above the loop stays when the "
            + "loop becomes one call:\n"
            + "    _songs.Clear();\n"
            + "    _songs.AddRange(loaded);\n"
            + "👉 Next: Rotation.cs, in Load. Put the Clear() back.");

        Assert.True(reopened.Count == 2,
            $"A file of 2 carts was loaded and the rotation holds {reopened.Count}. Load "
            + "empties the rotation and then puts every loaded cart in:\n"
            + "    _songs.Clear();\n"
            + "    _songs.AddRange(loaded);\n"
            + "👉 Next: if it holds 3, the loaded carts never went in. If it holds 0, they "
            + "went in before the Clear() instead of after it.");

        Assert.True(reopened.All()[0].Title == "Nightjar" && reopened.All()[1].Title == "Slack Water",
            $"The file held Nightjar then Slack Water, and the rotation came back as "
            + $"{Titles(reopened.All())}. Load puts the carts in the order the file has them.");

        Assert.True(reopened.All()[0].PlaysTonight == 2,
            $"Nightjar aired twice before it was saved and came back saying "
            + $"{reopened.All()[0].PlaysTonight}. The [JsonInclude] on Song.PlaysTonight "
            + "ships already in place — undo whatever changed in Song.cs.");

        var firstNight = new Rotation();
        firstNight.Add(new Song("Long Way Round", "The Ferrymen", 331));
        firstNight.Load(Path.Combine(Path.GetTempPath(), "kdxr-no-such-file.json"));

        Assert.True(firstNight.Count == 1,
            "Loading a file that does not exist left the rotation holding "
            + $"{firstNight.Count} cart(s) instead of the 1 it already had. The "
            + "File.Exists guard at the top of Load stays.");

        // ── the parts of the desk nobody touches tonight ───────────────────

        var board = new Switchboard();
        var dorothy = new Caller("Dorothy");
        board.Add(dorothy);
        Assert.True(ReferenceEquals(board.Take("Dorothy"), dorothy),
            "Switchboard.Take handed back a new Caller instead of the Dorothy on the board. "
            + "Switchboard.cs is your instructor's file — copy it in again from the "
            + "starters repo rather than editing it.");

        var hour = new Hour();
        var spot = new Ad("Pham's Bakery", "open at five", 3);
        hour.Add(spot);
        hour.Add(new Song("Nightjar", "The Lamplighters", 227));

        Assert.True(hour.TotalSeconds == 257,
            $"An hour holding a 30-second spot and a 227-second song adds up to "
            + $"{hour.TotalSeconds} and should add up to 257. Hour.cs is your instructor's "
            + "file — copy it in again from the starters repo rather than editing it.");

        Assert.True(hour.Run()[0].Contains("(2 left)"),
            "The hour printed the buy as it stood BEFORE the spot aired. Hour.cs is your "
            + "instructor's file — copy it in again from the starters repo.");
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
            + "comparison inside the brackets.");

        Assert.True(long240.Select(s => s.Title)
                .SequenceEqual(new[] { "Slack Water", "Long Way Round" }),
            $"Rotation.LongerThan(240) gave {Titles(long240)} and the rotation's own order "
            + "is Nightjar, Slack Water, Long Way Round — so it should give Slack Water, "
            + "Long Way Round.\n"
            + "Where does not reorder anything. It keeps what is left in the order it found "
            + "it.");

        // The boundary, and it is the only assert in here that can tell > from >=.
        // Nightjar is exactly 227 seconds long.
        List<Song> over227 = Ask("Rotation.LongerThan(227)", () => rotation.LongerThan(227));
        Assert.True(over227.Count == 2 && !over227.Any(s => s.Title == "Nightjar"),
            $"Rotation.LongerThan(227) gave {Titles(over227)} and should give Slack Water, "
            + "Long Way Round.\n"
            + "⚠️ Nightjar is EXACTLY 227 seconds long, and a cart that is exactly 227 "
            + "seconds is not longer than 227. That is the whole difference between > and "
            + ">=. At 240 the two spellings give the same answer and both look right.\n"
            + "👉 Next: read the comparison inside the brackets.");

        int all = Ask("Rotation.LongerThan(0)", () => rotation.LongerThan(0).Count);
        int none = Ask("Rotation.LongerThan(999)", () => rotation.LongerThan(999).Count);
        Assert.True(all == 3 && none == 0,
            $"LongerThan(0) gave {all} and LongerThan(999) gave {none}; they should give 3 "
            + "and 0. A question that is true of everything keeps everything, and one that "
            + "is true of nothing hands back an empty list — not null, and not an error.");

        Assert.True(ReferenceEquals(long240[0], rotation.All()[1]),
            "LongerThan handed back a cart the rotation is not holding — a copy, or a new "
            + "Song built from one. Where hands back the carts themselves.");

        Assert.True(rotation.Count == 3,
            $"Asking LongerThan changed the rotation — it holds {rotation.Count} carts now "
            + "and it held 3 before. A question never changes the list it is asked of.");
    }

    // ── Check 3 — Task 3: just the titles ──────────────────────────────────

    [Fact]
    public void Check3_TheDeskReadsOffItsTitles()
    {
        var rotation = ThreeCarts();

        List<string> titles = Ask("Rotation.Titles()", () => rotation.Titles());

        Assert.True(titles.Count == 3,
            $"Rotation.Titles() handed back {titles.Count} titles out of a rotation holding "
            + $"3. (It gave: {Words(titles)})\n"
            + "One title per cart. Select turns every item into something else:\n"
            + "    public List<string> Titles()\n"
            + "    {\n"
            + "        return _songs.Select(song => song.Title).ToList();\n"
            + "    }\n"
            + "Where keeps SOME of the carts. Select keeps all of them and hands back one "
            + "thing about each — here, the title.\n"
            + "👉 Next: if you got 0, the method is still handing back the empty list it "
            + "ships with.");

        Assert.True(titles.SequenceEqual(new[] { "Nightjar", "Slack Water", "Long Way Round" }),
            $"Rotation.Titles() gave {Words(titles)} and should give\n"
            + "    \"Nightjar\", \"Slack Water\", \"Long Way Round\"\n"
            + "Two things this could be. Either it is handing back something other than the "
            + "title — the artist, or the whole cue — or it has put them in order. Titles() "
            + "hands them back in the rotation's own order. Sorting is Task 4's job.");

        var empty = new Rotation();
        int fromEmpty = Ask("Rotation.Titles() on an empty rotation", () => empty.Titles().Count);
        Assert.True(fromEmpty == 0,
            $"Rotation.Titles() on an empty rotation handed back {fromEmpty} title(s). "
            + "Select over nothing is an empty list — not null, and not an error.");

        Assert.True(rotation.Count == 3,
            $"Asking for the titles changed the rotation — it holds {rotation.Count} carts "
            + "now and it held 3 before.");
    }

    // ── Check 4 — Task 4: in order, and the rotation left alone ────────────

    [Fact]
    public void Check4_TheCartsComeBackInOrder()
    {
        var rotation = ThreeCarts();
        List<Song> asLoaded = rotation.All();

        List<Song> sorted = Ask("Rotation.ByTitle()", () => rotation.ByTitle());

        Assert.True(sorted.Count == 3,
            $"Rotation.ByTitle() handed back {sorted.Count} carts out of a rotation holding "
            + "3. Sorting does not drop anything — every cart comes back, in a different "
            + "order:\n"
            + "    public List<Song> ByTitle()\n"
            + "    {\n"
            + "        return _songs.OrderBy(song => song.Title).ToList();\n"
            + "    }\n"
            + "👉 Next: if you got 0, the method is still handing back the empty list it "
            + "ships with.");

        // The three carts were added as Nightjar, Slack Water, Long Way Round, so
        // each way of getting this wrong puts a DIFFERENT cart first.
        string diagnosis =
            sorted[0].Title == "Nightjar"
                ? "That is the first cart LOADED, so nothing sorted at all — ByTitle() is "
                  + "handing back the rotation's own order.\n"
                  + "👉 Next: OrderBy(song => song.Title), and ToList() on the end."
            : sorted[0].Title == "Slack Water"
                ? "That is LAST alphabetically, so it sorted backwards — that is "
                  + "OrderByDescending.\n"
                  + "👉 Next: OrderBy is the one that starts at A."
                : "That is not an order I can account for, so it is sorting by something "
                  + "other than the title.\n"
                  + "👉 Next: read what is inside the brackets.";

        Assert.True(sorted[0].Title == "Long Way Round",
            $"Rotation.ByTitle() put {sorted[0].Title} first. In order by title the three "
            + "carts are Long Way Round, Nightjar, Slack Water.\n"
            + "⚠️ " + diagnosis);

        Assert.True(sorted[1].Title == "Nightjar" && sorted[2].Title == "Slack Water",
            $"Rotation.ByTitle() gave {Titles(sorted)}. In order by title it is Long Way "
            + "Round, Nightjar, Slack Water.");

        Assert.True(ReferenceEquals(sorted[1], asLoaded[0]),
            "ByTitle() handed back carts the rotation is not holding — copies, or new "
            + "Songs. OrderBy hands back the carts themselves, in a new list.");

        // ⚠️ The one this check really exists for.
        Assert.True(rotation.All().Select(s => s.Title)
                .SequenceEqual(new[] { "Nightjar", "Slack Water", "Long Way Round" }),
            "Sorting the rotation SORTED THE ROTATION. It is holding "
            + $"{Titles(rotation.All())} and it was loaded as Nightjar, Slack Water, Long "
            + "Way Round.\n"
            + "⚠️ OrderBy sorts a COPY and hands the copy back. Whatever you did reached "
            + "_songs itself — probably List.Sort, which rearranges the list it is called "
            + "on.\n"
            + "The rotation's own order is the order the carts were loaded in. Save writes "
            + "the file in that order, so a Sort in here quietly rewrites "
            + "week-09/rotation.json too. Open the file and look.\n"
            + "👉 Next: OrderBy, and let it build you a new list.");

        var empty = new Rotation();
        int fromEmpty = Ask("Rotation.ByTitle() on an empty rotation", () => empty.ByTitle().Count);
        Assert.True(fromEmpty == 0,
            $"Rotation.ByTitle() on an empty rotation handed back {fromEmpty} cart(s). "
            + "Sorting nothing gives an empty list — not null, and not an error.");
    }
}
