// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week. Turn ❌ into ✅ by editing the
//  files in Lab/, never this one.
//
//  Run them from your coursework folder — the one window you always have
//  open:
//      dotnet test week-07/Lab.Checks
//
//  ⚠️ This week you finally know what this file IS. It is a test project
//  — the same kind of thing as the Lab.Tests folder you are writing
//  tonight, wearing the name this course has used for it since week 1.
//  Open it. Read a check. You can read every line of it now.
//
//  Four of these are red out of the box, because the scheduler update
//  broke four rules of the desk. Your job each task: write YOUR test
//  first, watch it fail for the right reason, then fix the line.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Checks;

public class DeskChecks
{
    // ── Check 1 — everything the update did NOT touch ──────────────────────

    [Fact] // passes out of the box
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — that line ships finished and "
            + "the update never touched it. If this check is red, something in "
            + $"Broadcast.cs got changed by hand. (It returned: \"{Broadcast.CallSign()}\")");

        Assert.True(Broadcast.MinutesUntilSunrise(4, 30) == 90,
            $"Broadcast.MinutesUntilSunrise(4, 30) says {Broadcast.MinutesUntilSunrise(4, 30)} "
            + "and should say 90. That method ships finished — undo whatever changed in it.");

        var song = new Song("Nightjar", "The Lamplighters", 227);
        song.Seconds = -400;
        Assert.True(song.Seconds == 227,
            $"Song.Seconds accepted -400 and is now {song.Seconds}. That door was shut in "
            + "week 4 and it stays shut — Song.cs ships finished this week.");

        int plays = song.PlaysTonight;
        ((IScheduleItem)song).Play();
        Assert.True(song.PlaysTonight - plays == 1,
            "Playing a song should move PlaysTonight by exactly 1. Song.cs ships finished "
            + "— undo whatever changed in it.");

        var rotation = new Rotation();
        rotation.Add(song);
        rotation.All().Clear();
        Assert.True(rotation.Count == 1,
            $"Somebody emptied the list Rotation.All() handed them and the rotation went to "
            + $"{rotation.Count}. Rotation.cs ships finished — All() hands back a copy.");

        var board = new Switchboard();
        var dorothy = new Caller("Dorothy");
        board.Add(dorothy);
        Assert.True(ReferenceEquals(board.Find("Dorothy"), dorothy),
            "Switchboard.Find no longer hands back the Dorothy on the board. Find is the "
            + "half of the switchboard the update did NOT touch — if it is broken, that "
            + "was a hand edit. Take is Task 4; Find ships right.");

        Assert.True(board.Find("Somebody Else") == null,
            "Switchboard.Find handed something back for a name nobody has. It ships "
            + "returning null after the loop — undo whatever changed in it.");

        var bed = new WeatherBed("clear, four below, wind out of the northwest");
        Assert.True(bed is IScheduleItem && bed.Kind == "WEATHER" && bed.Seconds == 45,
            "WeatherBed.cs ships finished and the update never touched it. Undo whatever "
            + "changed in it.");

        var hour = new Hour();
        hour.Add(bed);
        hour.All().Clear();
        Assert.True(hour.Count == 1,
            $"Somebody emptied the list Hour.All() handed them and the hour went to "
            + $"{hour.Count}. Add, Count and All() ship finished — the update only "
            + "touched Run().");
    }

    // ── Check 2 — Task 2: the clock pads its seconds ───────────────────────

    [Fact]
    public void Check2_TheClockPadsItsSeconds()
    {
        Assert.True(Broadcast.Clock(605) == "10:05",
            $"Broadcast.Clock(605) says \"{Broadcast.Clock(605)}\" and should say \"10:05\". "
            + "605 seconds is ten minutes and five seconds, and a clock that prints "
            + "\"10:5\" reads as ten minutes and FIFTY seconds to a tired DJ.\n"
            + "The seconds always take two digits, and the format string can say so:\n"
            + "    return $\"{seconds / 60}:{seconds % 60:00}\";\n"
            + "The :00 is the part the update deleted. It means: at least two digits, "
            + "pad with zero.\n"
            + "👉 Next: one line in Broadcast.cs — and write your own test first.");

        Assert.True(Broadcast.Clock(600) == "10:00",
            $"Broadcast.Clock(600) says \"{Broadcast.Clock(600)}\" and should say \"10:00\" "
            + "— on the minute exactly, both second digits still show.");

        Assert.True(Broadcast.Clock(893) == "14:53",
            $"Broadcast.Clock(893) says \"{Broadcast.Clock(893)}\" and should say \"14:53\". "
            + "⚠️ This one passed even while the update's bug was in — 53 already has two "
            + "digits, which is exactly why the board looked fine all night. A test that "
            + "only asks easy questions agrees with everything.");

        Assert.True(Broadcast.Clock(45) == "0:45",
            $"Broadcast.Clock(45) says \"{Broadcast.Clock(45)}\" and should say \"0:45\" — "
            + "the MINUTES don't pad, only the seconds do. m:ss, same as every LENGTH "
            + "cell since week 4.");
    }

    // ── Check 3 — Task 3: a buy never goes below zero ──────────────────────

    [Fact]
    public void Check3_ABuyNeverGoesBelowZero()
    {
        var ad = (IScheduleItem)new Ad("Pham's Bakery", "open at five", 3);
        ad.Play();
        Assert.True(((Ad)ad).Remaining == 2,
            $"Airing a three-run buy once left Remaining at {((Ad)ad).Remaining}, and it "
            + "should be 2. The guard stops the count below zero — it must not stop the "
            + "spending. Every airing that has a run to spend, spends one.");

        var spent = (IScheduleItem)new Ad("Pham's Bakery", "open at five", 1);
        spent.Play();
        spent.Play();
        spent.Play();

        Assert.True(((Ad)spent).Remaining == 0,
            $"A one-run buy aired three times and Remaining is now {((Ad)spent).Remaining}. "
            + "That is three airings on a buy of one: two spots the station gave away "
            + "that nobody paid for. You wrote the line that "
            + "stopped this in week 6, and the update took it out:\n"
            + "    if (Remaining > 0) { Remaining--; }\n"
            + "👉 Next: put the guard back in Ad.Play() — and write your own test first, "
            + "so you see it fail while the bug is still there.");
    }

    // ── Check 4 — Task 4: Take hands back the caller on the board ──────────

    [Fact]
    public void Check4_TakeHandsBackTheCallerOnTheBoard()
    {
        var board = new Switchboard();
        var dorothy = new Caller("Dorothy");
        board.Add(dorothy);

        var took = board.Take("Dorothy");

        Assert.True(ReferenceEquals(took, dorothy),
            "Take(\"Dorothy\") handed back a brand-new Caller with Dorothy's name on it — "
            + "not the Dorothy on the board. That is the update's \"improvement\", and it "
            + "is week 5's copy lesson applied in exactly the wrong place: All() copies "
            + "the LIST so nobody can empty the board; Take hands back THE CALLER so the "
            + "call lands on the person who made it. Two names for one person — never "
            + "two people.\n"
            + "👉 Next: Take asks Find first, and only news up a caller Find couldn't "
            + "find. Write your own test first — Assert.Same is the question to ask.");

        Assert.True(board.Count == 1,
            $"Taking a call from a caller already on the board left {board.Count} caller(s) "
            + "on it. Dorothy rang; nobody new signed up.");

        Assert.True(dorothy.CallsTonight == 1,
            $"Dorothy took a call and HER CallsTonight says {dorothy.CallsTonight}. The "
            + "count went somewhere — onto a fresh Caller that was thrown away. The call "
            + "has to land on the caller the board is holding.");

        var stranger = board.Take("Pham's Bakery");
        Assert.True(board.Count == 2
                    && ReferenceEquals(board.Find("Pham's Bakery"), stranger)
                    && stranger.CallsTonight == 1,
            "A first-time caller should be signed up, counted once, and handed back — the "
            + "same object the board now holds. That half of Take was right before the "
            + "update and has to stay right after your fix.");
    }

    // ── Check 5 — Task 5: the desk prints what actually aired ──────────────

    [Fact]
    public void Check5_TheDeskPrintsWhatActuallyAired()
    {
        var song = new Song("Nightjar", "The Lamplighters", 227);
        var ad = new Ad("Pham's Bakery", "open at five", 3);

        var hour = new Hour();
        hour.Add(song);
        hour.Add(ad);

        var aired = hour.Run();

        Assert.True(aired.Count == 2,
            $"Running an hour of two items handed back {aired.Count} line(s). One line per "
            + "item, in the order they are scheduled — that part ships finished.");

        Assert.True(song.PlaysTonight == 1 && ad.Remaining == 2,
            "The hour ran and something didn't actually air — the song's count or the "
            + "ad's buy never moved. Run() still has to Play() every item; the update "
            + "only changed WHEN it did.");

        // Compared against the ad's own Cue, before and after — never against
        // any particular wording.
        string before = ((IScheduleItem)new Ad("Pham's Bakery", "open at five", 3)).Cue;
        string after = ((IScheduleItem)ad).Cue;

        Assert.True(before != after,
            "Ad.Cue reads the same before and after an airing. It ships showing the runs "
            + "left on the buy — Ad.cs is Task 3's file, and this check can't tell the "
            + "order of Run() apart until the cue moves when the ad airs.");

        Assert.True(aired[1].Contains(after) && !aired[1].Contains(before),
            $"The desk printed \"{aired[1]}\" for the ad — the cue as it read BEFORE the "
            + "spot went out. Its cue reads \"" + before + "\" before an airing and \""
            + after + "\" after one, and the line the station keeps is the after one.\n"
            + "The desk prints what happened, not what was about to:\n"
            + "        item.Play();\n"
            + "        aired.Add($\"{item.Kind} - {item.Cue}\");\n"
            + "👉 Next: two lines in Hour.Run(), back in the order you wrote them in "
            + "week 6 — and write your own test first.");
    }
}
