// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing
//  Lab/Song.cs and Lab/Rotation.cs, never this file.
//
//  Run them from your coursework folder — the one window you always have open:
//      dotnet test week-04/Lab.Checks
//
//  New this week: a couple of these look at the SHAPE of your class rather
//  than at what it hands back — whether Title is a field or a property,
//  whether PlaysTonight has a setter anybody can reach. That is the week's
//  subject, so it is the week's check.
//
//  No FreshShift() this week, and that is worth a second. Last week the log
//  was `static` — one copy, shared by every check, so each one had to sweep
//  up after itself. Tonight a Rotation is an OBJECT: every check below says
//  `new Rotation()` and gets its own. Nothing to reset, nothing to leak.
// ═══════════════════════════════════════════════════════════════════
using System.Reflection;

namespace Lab.Checks;

public class RotationChecks
{
    // ── plumbing: the shape questions ──────────────────────────────────────

    private static FieldInfo? PublicField(Type type, string name) =>
        type.GetField(name, BindingFlags.Public | BindingFlags.Instance);

    private static PropertyInfo? PublicProperty(Type type, string name) =>
        type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

    private static string[] PublicFieldNames(Type type) =>
        type.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f => f.Name)
            .ToArray();

    // The same sentence gets said about Title, Artist and Seconds, so it is
    // written once. Says what is wrong, what to write, and why it matters.
    private static void MustBeAProperty(string member, string backing)
    {
        Assert.True(PublicField(typeof(Song), member) == null,
            $"Song.{member} is still a public FIELD, so anything anywhere in the program "
            + "can write anything into it — which is exactly what `g` does at 03:14.\n"
            + $"    private {(member == "Seconds" ? "int" : "string")} {backing};\n"
            + $"    public {(member == "Seconds" ? "int" : "string")} {member}\n"
            + "    {\n"
            + $"        get {{ return {backing}; }}\n"
            + "        set { ... }        // ← your rule goes here\n"
            + "    }\n"
            + "A field is a hole in the wall. A property is a door with somebody standing "
            + "at it.");

        Assert.True(PublicProperty(typeof(Song), member) != null,
            $"Song has no public property called {member} — check the spelling and the "
            + "capital letter. The rest of the program (and this check) reads it by that "
            + "exact name.");
    }

    // ── Check 1 — ships finished ───────────────────────────────────────────

    [Fact] // passes out of the box: weeks 1-3 ship complete
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — Broadcast.cs ships finished this "
            + $"week, so if this check is red something in it got changed. (It returned: "
            + $"\"{Broadcast.CallSign()}\")\n"
            + "Tonight's work is all in Lab/Song.cs and Lab/Rotation.cs.");

        Assert.True(Broadcast.SignOn("Marisol").Contains("Marisol"),
            "Broadcast.SignOn(\"Marisol\") should still name the DJ it was handed.\n"
            + $"It returned:\n    {Broadcast.SignOn("Marisol")}\n"
            + "That's week 1's method, shipped finished. Undo whatever changed in "
            + "Lab/Broadcast.cs.");
    }

    // ── Check 2 — Task 2: the rotation is an object with a private list ────

    [Fact]
    public void Check2_TheRotationKeepsItsOwnList()
    {
        var rotation = new Rotation();

        Assert.True(rotation.Count == 0,
            $"A brand-new Rotation says it holds {rotation.Count} songs, and it should hold "
            + "0. Count has to ask the list:\n"
            + "    public int Count => _songs.Count;\n"
            + "It ships returning a hard-coded 0, which is right exactly once and wrong "
            + "forever after.");

        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));

        Assert.True(rotation.Count == 1,
            $"After one Add, Count says {rotation.Count} and it should say 1.\n"
            + "Add puts the song in the private list — one line:\n"
            + "    _songs.Add(song);\n"
            + "If Count is already asking _songs, then Add is the half that's missing.");

        rotation.Add(new Song("Slack Water", "Marguerite Vance", 252));
        Assert.True(rotation.Count == 2,
            $"After a second Add, Count says {rotation.Count} and it should say 2. Every "
            + "song goes on the END — Add never replaces what's already there.");

        List<Song> handedBack = rotation.All();
        Assert.True(handedBack.Count == 2,
            $"All() handed back {handedBack.Count} song(s) after two Adds, and it should "
            + "hand back 2. It ships returning a brand-new empty list, which is why your "
            + "board is blank no matter how much you load into it.");

        // The one that matters: a copy, not the real thing.
        handedBack.Clear();
        Assert.True(rotation.Count == 2,
            $"Somebody emptied the list All() gave them, and the Rotation went from 2 songs "
            + $"to {rotation.Count}. That means All() handed out _songs ITSELF, so the "
            + "`private` on it was never really doing anything:\n"
            + "    return new List<Song>(_songs);      // a copy of the list\n"
            + "Press `g` on the running shift to watch the version you have now get wiped "
            + "by the automation.");

        Assert.True(PublicFieldNames(typeof(Rotation)).Length == 0,
            $"Rotation has a public field ({string.Join(", ", PublicFieldNames(typeof(Rotation)))}) "
            + "— so the list is reachable from outside after all, and Add/Count/All are "
            + "decoration. Keep _songs private; the three members are the door.");
    }

    // ── Check 3 — Task 3: Title and Artist become properties that refuse ───

    [Fact]
    public void Check3_ABlankTitleNeverGetsIn()
    {
        MustBeAProperty("Title", "_title");
        MustBeAProperty("Artist", "_artist");

        var song = new Song("Nightjar", "The Lamplighters", 227);

        Assert.True(song.Title == "Nightjar",
            $"new Song(\"Nightjar\", ...).Title came back as \"{song.Title}\".\n"
            + "A good title still has to go straight in — the setter refuses blanks, not "
            + "everything. Make sure the `set` stores `value` when it's fine:\n"
            + "        set { if (!string.IsNullOrWhiteSpace(value)) { _title = value; } }");

        Assert.True(song.Artist == "The Lamplighters",
            $"new Song(..., \"The Lamplighters\", ...).Artist came back as \"{song.Artist}\".\n"
            + "Same as Title: a real value goes in untouched.");

        song.Title = "   ";
        Assert.True(song.Title == "Nightjar",
            $"The automation set Title to three spaces and the song is now called "
            + $"\"{song.Title}\". A blank title must leave the old one alone — "
            + "string.IsNullOrWhiteSpace is the one that catches spaces as well as empty.");

        song.Artist = "";
        Assert.True(song.Artist == "The Lamplighters",
            $"Artist was set to \"\" and came back as \"{song.Artist}\". Same rule, same "
            + "method — and if you wrote the rule twice, that's two places to get it wrong "
            + "later.");

        // Straight through the constructor, which is the same door.
        var nameless = new Song("", "", 200);
        Assert.False(string.IsNullOrWhiteSpace(nameless.Title),
            "new Song(\"\", \"\", 200) produced a song with a blank Title. The constructor "
            + "assigns to the PROPERTY, so your setter should have caught it — and the "
            + "field's starting value is what's left:\n"
            + "    private string _title = \"(untitled)\";\n"
            + "A blank row on the board is how a cart goes missing for a week.");
    }

    // ── Check 4 — Task 4: Seconds refuses nonsense, Length reads off it ────

    [Fact]
    public void Check4_ASongCannotBeMinusSevenMinutes()
    {
        MustBeAProperty("Seconds", "_seconds");

        var song = new Song("Nightjar", "The Lamplighters", 227);

        Assert.True(song.Seconds == 227,
            $"new Song(..., 227).Seconds came back as {song.Seconds}. A sensible length "
            + "goes straight in; the setter only turns away the impossible ones.");

        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"The automation set Seconds to -400 and the song is now {song.Seconds} seconds "
            + "long. Nothing on air runs backwards:\n"
            + "        set { if (value >= 1) { _seconds = value; } }");

        song.Seconds = 0;
        Assert.True(song.Seconds == 227,
            $"Seconds was set to 0 and came back as {song.Seconds}. Zero is the one that "
            + "slips through a `value < 0` test — a nothing-long song is still dead air, so "
            + "the line is `value >= 1`, not `value >= 0`.");

        Assert.True(song.Length == "3:47",
            $"227 seconds should read as 3:47 on the board, and Length says \"{song.Length}\".\n"
            + "    public string Length => $\"{Seconds / 60}:{Seconds % 60:00}\";\n"
            + "Nothing stores this — it's worked out from Seconds every time anybody asks, "
            + "which is why it has a get and no set.");

        var shortOne = new Song("Sign-off", "KDXR", 187);
        Assert.True(shortOne.Length == "3:07",
            $"187 seconds should read as 3:07, and Length says \"{shortOne.Length}\".\n"
            + "If you got 3:7, the seconds need padding to two digits — that's what the :00 "
            + "in {Seconds % 60:00} is for.");
    }

    // ── Check 5 — Task 5: the play count cannot be forged ──────────────────

    [Fact]
    public void Check5_ThePlayCountCannotBeForged()
    {
        var song = new Song("Nightjar", "The Lamplighters", 227);

        var plays = PublicProperty(typeof(Song), "PlaysTonight");
        Assert.True(plays != null,
            "Song has no public property called PlaysTonight.\n"
            + "    public int PlaysTonight { get; private set; }\n"
            + "That one line is tonight's whole argument: readable by anybody, writable by "
            + "nobody outside this class.");

        Assert.True(PublicField(typeof(Song), "PlaysTonight") == null,
            "PlaysTonight is still a public field, so any line anywhere can claim a song "
            + "played forty times.\n"
            + "    public int PlaysTonight { get; private set; }");

        Assert.True(plays!.GetSetMethod(nonPublic: false) == null,
            "PlaysTonight is a property, but its setter is public — so it is still writable "
            + "from outside and nothing has changed.\n"
            + "    public int PlaysTonight { get; private set; }\n"
            + "                                    ^^^^^^^\n"
            + "`private set` is the whole difference.");

        Assert.True(song.PlaysTonight == 0,
            $"A song that has never been on air says it played {song.PlaysTonight} times.");

        song.Play();
        Assert.True(song.PlaysTonight == 1,
            $"After one Play(), PlaysTonight says {song.PlaysTonight} and it should say 1.\n"
            + "Play() is now the only thing in the entire program that can move this number:\n"
            + "    public void Play() { PlaysTonight++; }");

        song.Play();
        song.Play();
        Assert.True(song.PlaysTonight == 3,
            $"After three Play() calls, PlaysTonight says {song.PlaysTonight}. Each play "
            + "adds one — if you're stuck on 1, the method is setting rather than adding.");

        // The night's actual claim, checked in one line.
        Assert.True(PublicFieldNames(typeof(Song)).Length == 0,
            "Song still has "
            + $"{PublicFieldNames(typeof(Song)).Length} public field(s): "
            + $"{string.Join(", ", PublicFieldNames(typeof(Song)))}.\n"
            + "Every one of them is a way in that nothing is watching. That was the shape "
            + "this class shipped in, and closing the last of them is the end of the shift.");
    }
}
