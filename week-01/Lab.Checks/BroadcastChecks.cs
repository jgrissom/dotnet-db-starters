// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — these checks are how you know the lab is done. They are not
//  your grade, and they are not collected. Your job is turning ❌ into ✅ by
//  editing Lab/Broadcast.cs — never this file.
//
//  Run them from your coursework folder — the one window you always have open:
//      dotnet test week-01/Lab.Checks
//
//  Notice what they do NOT look at: nothing in here reads what your program
//  prints. They call your methods and check what comes back. That's the whole
//  reason Broadcast.cs exists as a separate thing from Program.cs — and it's
//  the shape every piece of homework in this course will ask you for.
//
//  In week 7 you find out this file is not magic, and you write one yourself.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Checks;

public class BroadcastChecks
{
    [Fact] // passes out of the box — the station you were handed is on the air
    public void Check1_TheStationIsOnTheAir()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\", and it's the one method that was "
            + "written for you — so if this check is red, something else in the project got "
            + $"changed by accident. (It returned: \"{Broadcast.CallSign()}\")\n"
            + "Undo whatever you last did in Lab/Broadcast.cs and this goes green again.");
    }

    [Fact] // Task 2: a method that takes something in and gives something back
    public void Check2_TheSignOnGreetsTheDj()
    {
        var marisol = Broadcast.SignOn("Marisol");

        Assert.False(string.IsNullOrWhiteSpace(marisol),
            "Broadcast.SignOn(\"Marisol\") came back empty. This is the first method you write "
            + "tonight: it takes the DJ's name and returns the line they see when their shift "
            + "starts. Something like:\n"
            + "    return $\"{CallSign()} 88.1 The Owl - you're on with {djName}.\";\n"
            + "The $ in front of the quotes is what lets you drop a value straight into the "
            + "text — anything inside the braces gets evaluated.");

        Assert.True(marisol.Contains("Marisol"),
            "SignOn(\"Marisol\") returned a line with no \"Marisol\" in it:\n"
            + $"    {marisol}\n"
            + "The DJ's name arrives as the djName parameter — the value in the brackets. Put "
            + "it in the text with {djName} inside a $\"...\" string.");

        Assert.True(marisol.Contains("KDXR"),
            "SignOn(\"Marisol\") doesn't mention the station:\n"
            + $"    {marisol}\n"
            + "It should contain the call sign, and you get that by CALLING the method that "
            + "already knows it — CallSign() — rather than typing \"KDXR\" again. Two places "
            + "that both know the station's name is one place too many.");

        // A second name, because a line that only works for Marisol isn't a method.
        var ford = Broadcast.SignOn("Ford");
        Assert.True(ford.Contains("Ford") && !ford.Contains("Marisol"),
            "SignOn works for \"Marisol\" but not for anybody else:\n"
            + $"    SignOn(\"Ford\") returned: {ford}\n"
            + "The name is typed into the text instead of coming from the djName parameter. A "
            + "method has to work for every value it's handed, not just the one you tested with.");
    }

    [Fact] // Task 3: arithmetic, and reading two parameters
    public void Check3_TheClockCountsDownToSunrise()
    {
        // 6:00 AM is 360 minutes past midnight. Everything here is that minus "now".
        Assert.True(Broadcast.MinutesUntilSunrise(2, 15) == 225,
            "At 2:15 AM there are 225 minutes until 6:00, and "
            + $"MinutesUntilSunrise(2, 15) returned {Broadcast.MinutesUntilSunrise(2, 15)}.\n"
            + "Two steps: turn the time you were given into minutes past midnight (the hour is "
            + "worth 60 each), then subtract that from 360, which is what 6:00 AM is worth.");

        Assert.True(Broadcast.MinutesUntilSunrise(0, 0) == 360,
            "Midnight is a full six hours before sunrise, so MinutesUntilSunrise(0, 0) should "
            + $"be 360 — it returned {Broadcast.MinutesUntilSunrise(0, 0)}.");

        Assert.True(Broadcast.MinutesUntilSunrise(5, 59) == 1,
            "One minute before six should leave 1 minute, and MinutesUntilSunrise(5, 59) "
            + $"returned {Broadcast.MinutesUntilSunrise(5, 59)}. If you're off by a multiple of "
            + "59, the minutes got added before the hour was multiplied — brackets fix it: "
            + "(hour * 60) + minute.");
    }

    [Fact] // Task 4: the integer-division trap — the silent one from the demo
    public void Check4_TheHoursIncludeThePartHours()
    {
        var got = Broadcast.HoursOnAir(330);

        // Whole hours can't tell the two mistakes apart (5 and 5.0 look alike),
        // so every case here has a half in it on purpose.
        Assert.True(Math.Abs(got - 5.5) < 0.0001,
            $"330 minutes is five and a half hours. HoursOnAir(330) returned {got}.\n"
            + (Math.Abs(got - 5.0) < 0.0001
                ? "You got 5, so the halves are being thrown away — the demo's generator bug, "
                  + "in your own code.\n"
                  + "In C#, a whole number divided by a "
                  + "whole number is a WHOLE NUMBER: the division happens first, the remainder "
                  + "is dropped on the floor, and only then does the answer become a double. "
                  + "Nothing warns you, because nothing went wrong as far as C# is concerned.\n"
                  + "Make one side of the division not-a-whole-number:\n"
                  + "    return minutes / 60.0;\n"
                  + "The 0 after the point is doing all of the work."
                : "Divide the minutes by 60 — and make sure one side of that division is a "
                  + "double, or C# throws away the remainder:\n"
                  + "    return minutes / 60.0;"));

        var quarter = Broadcast.HoursOnAir(90);
        Assert.True(Math.Abs(quarter - 1.5) < 0.0001,
            $"HoursOnAir(90) should be 1.5 and returned {quarter} — 90 minutes is an hour and "
            + "a half. Same cause as above: whole-number division drops the half.");

        var whole = Broadcast.HoursOnAir(60);
        Assert.True(Math.Abs(whole - 1.0) < 0.0001,
            $"HoursOnAir(60) should be 1 and returned {whole}. An exact hour is the easy case, "
            + "so if this one is wrong the arithmetic isn't a division by 60 at all.");
    }

    [Fact] // Task 5: a bool, and a condition that wraps around midnight
    public void Check5_TheOwlKnowsWhenItsLate()
    {
        foreach (var hour in new[] { 22, 23, 0, 1, 3, 5 })
        {
            Assert.True(Broadcast.IsOvernight(hour),
                $"IsOvernight({hour}) returned false, but {hour}:00 is inside the overnight "
                + "block — it runs 22:00 through 05:59.\n"
                + "This is the one that needs || (\"or\") rather than &&: the block wraps past "
                + "midnight, so an hour counts if it's late enough OR early enough. No single "
                + "hour is ever both, which is why && can never be true here.");
        }

        foreach (var hour in new[] { 6, 9, 12, 17, 21 })
        {
            Assert.False(Broadcast.IsOvernight(hour),
                $"IsOvernight({hour}) returned true, but {hour}:00 is daytime — the overnight "
                + "block is 22:00 through 05:59, so 6 through 21 are outside it.\n"
                + "Check the boundaries: 6 is NOT overnight, and 22 is.");
        }
    }
}
