// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-08/Lab.Checks
//
//  Every check in here hands Save and Load a path of ITS OWN, in the
//  system's temp folder — never week-08/rotation.json. That is not
//  politeness: a check that wrote to the station's real files would grade
//  you on last night's shift instead of on your code, and `dotnet test`
//  does not even stand in the same folder your program does.
//
//  Four are red out of the box, because the desk cannot write anything
//  down yet. Check 1 is everything it already does, and it stays green.
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

    // ── Check 1 — everything the desk already does ─────────────────────────

    [Fact] // passes out of the box
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — that line ships finished and "
            + "nothing this week touches it. If this check is red, something in "
            + $"Broadcast.cs got changed by hand. (It returned: \"{Broadcast.CallSign()}\")");

        Assert.True(Broadcast.MinutesUntilSunrise(4, 30) == 90,
            $"Broadcast.MinutesUntilSunrise(4, 30) says {Broadcast.MinutesUntilSunrise(4, 30)} "
            + "and should say 90. That method ships finished — undo whatever changed in it.");

        Assert.True(Broadcast.Clock(605) == "10:05",
            $"Broadcast.Clock(605) says \"{Broadcast.Clock(605)}\" and should say \"10:05\". "
            + "That is last week's repair — the :00 in the format string — and it ships "
            + "already made. Undo whatever took it back out.");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. That door was shut in "
            + "week 4 and it stays shut — the only line of Song.cs that changes this week "
            + "is one attribute.");

        int plays = song.PlaysTonight;
        ((IScheduleItem)song).Play();
        Assert.True(song.PlaysTonight - plays == 1,
            "Playing a song should move PlaysTonight by exactly 1. Play() ships finished — "
            + "this week changes how that number is STORED, never how it moves.");

        var rotation = new Rotation();
        rotation.Add(song);
        rotation.All().Clear();
        Assert.True(rotation.Count == 1,
            $"Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. All() hands back a copy, and it ships finished — Save and "
            + "Load are the two members that change this week.");

        var ad = new Ad("Pham's Bakery", "open at five", 1);
        ad.Play();
        ad.Play();
        Assert.True(ad.Remaining == 0,
            $"A one-run buy aired twice and Remaining is {ad.Remaining}. That guard is last "
            + "week's repair and it ships already made.");

        var board = new Switchboard();
        var dorothy = new Caller("Dorothy");
        board.Add(dorothy);
        Assert.True(ReferenceEquals(board.Take("Dorothy"), dorothy),
            "Switchboard.Take handed back a new Caller instead of the Dorothy on the board. "
            + "That is last week's repair and it ships already made.");

        var hour = new Hour();
        var spot = new Ad("Pham's Bakery", "open at five", 3);
        hour.Add(spot);
        Assert.True(hour.Run()[0].Contains("(2 left)"),
            "The hour printed the buy as it stood BEFORE the spot aired. Hour.Run() plays "
            + "first and reads the cue second — last week's repair, shipped already made.");
    }

    // ── Check 2 — Task 2: the rotation is written down ─────────────────────

    [Fact]
    public void Check2_TheRotationIsWrittenDown()
    {
        string path = Scratch("save.json");

        var rotation = new Rotation();
        rotation.Add(new Song("Nightjar", "The Lamplighters", 227));
        rotation.Add(new Song("Slack Water", "Marguerite Vance", 252));

        rotation.Save(path);

        Assert.True(File.Exists(path),
            "Rotation.Save() was handed a path and no file appeared there. Nothing was "
            + "written down, so nothing can come back.\n"
            + "Two lines: turn the list into text, then put the text in the file.\n"
            + "    string json = JsonSerializer.Serialize(_songs,\n"
            + "        new JsonSerializerOptions { WriteIndented = true });\n"
            + "    File.WriteAllText(path, json);\n"
            + "👉 Next: Rotation.cs, the TODO under Task 2. Save the path you are HANDED "
            + "— never a name written inside the method.");

        string written = File.ReadAllText(path);

        Assert.True(written.Contains("Nightjar") && written.Contains("Slack Water"),
            "A file appeared, and neither song's title is in it. Whatever got written, it "
            + $"was not the rotation. The file says:\n{written}\n"
            + "Serialize the list itself — `_songs` — not a count of it and not a "
            + "description of it.");

        Assert.True(written.Contains("227"),
            "The titles are in the file and the lengths are not. Serialize the whole "
            + "SONG — the serializer writes every public property it can read, and that "
            + "is the point of handing it the objects rather than strings you built.");
    }

    // ── Check 3 — Task 3: the rotation comes back ──────────────────────────

    [Fact]
    public void Check3_TheRotationComesBack()
    {
        string path = Scratch("load.json");

        var saved = new Rotation();
        saved.Add(new Song("Nightjar", "The Lamplighters", 227));
        saved.Add(new Song("Slack Water", "Marguerite Vance", 252));
        saved.Save(path);

        // A second rotation, holding nothing, reading the same file.
        var reopened = new Rotation();
        reopened.Load(path);

        Assert.True(reopened.Count == 2,
            $"A rotation of two carts was saved, and loading it back gave {reopened.Count}. "
            + "Load reads the text, turns it back into a list of songs, and puts them in "
            + "_songs:\n"
            + "    List<Song>? loaded = JsonSerializer.Deserialize<List<Song>>(\n"
            + "        File.ReadAllText(path));\n"
            + "👉 Next: Rotation.cs, the TODO under Task 3. Clear the list before you fill "
            + "it, or the loaded carts land on top of the ones already there.");

        var titles = new List<string>();
        foreach (var s in reopened.All()) { titles.Add(s.Title); }

        Assert.True(titles.Contains("Nightjar") && titles.Contains("Slack Water"),
            $"Two carts came back and they are not the two that were saved: {string.Join(", ", titles)}. "
            + "Deserialize hands back the songs the file describes — put those in the "
            + "rotation rather than making new ones.");

        Assert.True(reopened.All()[0].Seconds == 227,
            $"Nightjar came back {reopened.All()[0].Seconds} seconds long instead of 227. "
            + "The lengths are in the file — check that Load is handing Deserialize the "
            + "file's text and not a path or a name.");

        // The first night on a desk: no file at all, and that is not an error.
        var firstNight = new Rotation();
        firstNight.Add(new Song("Long Way Round", "The Ferrymen", 331));
        firstNight.Load(Path.Combine(Path.GetTempPath(), "kdxr-no-such-file.json"));

        Assert.True(firstNight.Count == 1,
            $"Loading a file that does not exist left the rotation holding "
            + $"{firstNight.Count} cart(s) instead of the 1 it already had. A desk that "
            + "has never been signed off has no file, and that is a first night rather "
            + "than a failure — ask File.Exists(path) FIRST and just return.");
    }

    // ── Check 4 — Task 4: a cart remembers its plays ───────────────────────

    [Fact]
    public void Check4_ACartRemembersItsPlays()
    {
        string path = Scratch("plays.json");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        ((IScheduleItem)song).Play();
        ((IScheduleItem)song).Play();

        var rotation = new Rotation();
        rotation.Add(song);
        rotation.Save(path);

        var reopened = new Rotation();
        reopened.Load(path);

        Assert.True(reopened.Count == 1,
            "This check needs Task 3 working first — one cart was saved and it did not "
            + "come back. Finish Load, then come back to this one.");

        Assert.True(reopened.All()[0].PlaysTonight == 2,
            $"The cart aired twice, and after a save and a load its PlaysTonight says "
            + $"{reopened.All()[0].PlaysTonight}.\n"
            + "Open the file and look: the number IS in there. A serializer writes every "
            + "property it can READ, and reads back only the ones it can WRITE — and "
            + "PlaysTonight has a private setter, so it goes out and never comes home.\n"
            + "One line above the property says otherwise:\n"
            + "    [JsonInclude]\n"
            + "    public int PlaysTonight { get; private set; }\n"
            + "It needs `using System.Text.Json.Serialization;` at the top of Song.cs.\n"
            + "👉 Next: write the fact FIRST and watch it go red against this, the way "
            + "you did four times last week.");

        Assert.True(reopened.All()[0].Title == "Nightjar",
            "The play count survived and the title did not, which is backwards. Whatever "
            + "changed in Song.cs, the title is a plain public property and it should "
            + "still be coming back on its own.");
    }

    // ── Check 5 — Task 5: the air log remembers the last shift ─────────────

    [Fact]
    public void Check5_TheAirLogRemembersTheLastShift()
    {
        string path = Scratch("air-log.txt");

        Assert.True(Broadcast.LastShift(path) == "",
            $"Broadcast.LastShift() was pointed at a log that does not exist and answered "
            + $"\"{Broadcast.LastShift(path)}\". Nobody has ever signed off on that desk, "
            + "so the honest answer is an empty string — ask File.Exists(path) first.");

        Broadcast.LogShift(path, "Dorothy signed off - 6 in the hour.");

        Assert.True(File.Exists(path),
            "Broadcast.LogShift() was handed a path and no file appeared. AppendAllText "
            + "makes the file if it isn't there yet, which is why the air log needs no "
            + "setting-up:\n"
            + "    File.AppendAllText(path, line + \"\\n\");\n"
            + "👉 Next: Broadcast.cs, the TODO under Task 5.");

        Broadcast.LogShift(path, "Teodoro signed off - 9 in the hour.");

        Assert.True(Broadcast.LastShift(path) == "Teodoro signed off - 9 in the hour.",
            $"Two shifts signed off and the log's last line says "
            + $"\"{Broadcast.LastShift(path)}\". It should be the SECOND one — the last "
            + "line of the file, which is lines[lines.Length - 1].");

        string[] all = File.ReadAllLines(path);

        Assert.True(all.Length == 2,
            $"Two shifts signed off and the air log holds {all.Length} line(s). An air log "
            + "is added to, never rewritten — WriteAllText starts the file over every "
            + "time and AppendAllText does not. That is the whole difference, and it is "
            + "the difference between a log and a snapshot.");

        Assert.True(all[0] == "Dorothy signed off - 6 in the hour.",
            $"The log's FIRST line says \"{all[0]}\". The oldest line stays where it is; "
            + "new lines go on the end.");
    }
}
