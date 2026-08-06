// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this time it IS your grade. These are the exact checks
//  I run against your repo; I don't have a second, secret set.
//
//  Run them from your coursework folder — the one window you always have open:
//      dotnet test week-01/Homework.Checks
//
//  Notice what they never do: they never assume what your station is called,
//  what city it's in, or when it signs off. They ask YOUR code what its own
//  answers are, and then hold it to them. That's the only way a check can
//  grade fifteen different stations — and from week 4, fifteen different
//  semester projects.
// ═══════════════════════════════════════════════════════════════════
using System.Text.RegularExpressions;

namespace Homework.Checks;

public class HomeworkChecks
{
    [Fact]
    public void Check1_TheStationHasALegalCallSign()
    {
        var method = StudentCode.RequireMethod("CallSign", typeof(string));
        var callSign = (string)StudentCode.Call(method);

        Assert.False(string.IsNullOrWhiteSpace(callSign),
            "Station.CallSign() came back empty. It should return your station's call sign — "
            + "four letters, yours to invent.");

        Assert.True(callSign.Length == 4,
            $"Station.CallSign() returned \"{callSign}\", which is {callSign.Length} characters. "
            + "US call signs are four letters — KOWL, WXYZ, KRVN. Yours can be anything you "
            + "like as long as it's four.");

        Assert.True(Regex.IsMatch(callSign, "^[A-Z]{4}$"),
            $"Station.CallSign() returned \"{callSign}\". It needs to be four capital letters — "
            + "no digits, no spaces, no punctuation. Broadcasters write them in capitals and so "
            + "do we, because check 3 goes looking for this exact text inside your sign-on.");

        Assert.True(callSign[0] == 'K' || callSign[0] == 'W',
            $"Station.CallSign() returned \"{callSign}\", which starts with '{callSign[0]}'. "
            + "Real US call signs start with K (roughly, west of the Mississippi) or W (east of "
            + "it). Pick whichever suits where you've put your station — this one is a free "
            + "point for reading the assignment.");
    }

    [Fact]
    public void Check2_TheStationHasAHome()
    {
        var method = StudentCode.RequireMethod("City", typeof(string));
        var city = (string)StudentCode.Call(method);

        Assert.False(string.IsNullOrWhiteSpace(city),
            "Station.City() came back empty. Your station broadcasts from somewhere — a real "
            + "town, an invented one, the bottom of a lake. It just has to say where.");

        Assert.True(city.Trim().Length >= 3,
            $"Station.City() returned \"{city}\", which is too short to be a place. Three "
            + "characters or more.");

        var placeholders = new[] { "your city", "city", "citygoeshere", "todo", "changeme", "xxx" };
        Assert.False(placeholders.Contains(city.Trim().ToLowerInvariant()),
            $"Station.City() still returns \"{city}\" — that's the placeholder, not a place. "
            + "Put your station somewhere.");
    }

    [Fact]
    public void Check3_TheSignOnIdentifiesTheStation()
    {
        var callSign = (string)StudentCode.Call(StudentCode.RequireMethod("CallSign", typeof(string)));
        var city = (string)StudentCode.Call(StudentCode.RequireMethod("City", typeof(string)));

        Assert.False(string.IsNullOrWhiteSpace(callSign) || string.IsNullOrWhiteSpace(city),
            "This check can't run yet — it asks whether SignOn() says your call sign and your "
            + "city, and one of those two is still blank, so there's nothing to look for.\n"
            + "👉 Next: get Check 1 and Check 2 green first, then come back to this one.");

        var method = StudentCode.RequireMethod("SignOn", typeof(string));
        var signOn = (string)StudentCode.Call(method);

        Assert.False(string.IsNullOrWhiteSpace(signOn),
            "Station.SignOn() came back empty. This is the line your station says when it goes "
            + "on the air, and by law it has to identify the station — call sign and city, at "
            + "minimum. Build it with a $\"...\" string, the same way the lab's SignOn did.");

        Assert.True(signOn.Contains(callSign),
            $"Station.SignOn() returned:\n    {signOn}\n"
            + $"...and your call sign, \"{callSign}\", isn't in it. A station identification that "
            + "doesn't say the call sign isn't an identification.\n"
            + "Build the line by CALLING your own methods rather than retyping the text:\n"
            + "    return $\"{CallSign()}, broadcasting from {City()}.\";\n"
            + "That way changing the call sign in one place changes it everywhere.");

        Assert.True(signOn.Contains(city),
            $"Station.SignOn() returned:\n    {signOn}\n"
            + $"...and your city, \"{city}\", isn't in it. Same fix as the call sign — put "
            + "{City()} inside the $\"...\" string rather than typing the name again.");
    }

    [Fact]
    public void Check4_TheClockCountsDownToSignOff()
    {
        var hourMethod = StudentCode.RequireMethod("SignOffHour", typeof(int));
        var signOffHour = (int)StudentCode.Call(hourMethod);

        Assert.True(signOffHour >= 1 && signOffHour <= 12,
            $"Station.SignOffHour() returned {signOffHour}. The homework asks for an hour "
            + "between 1 and 12 — a station that shuts down at 6:00 AM returns 6. (The limit "
            + "isn't fussiness: it's what lets me check your countdown against YOUR sign-off "
            + "time instead of a time I picked.)");

        var method = StudentCode.RequireMethod("MinutesUntilSignOff", typeof(int), typeof(int), typeof(int));

        // Every expectation below is built from the student's OWN sign-off hour.
        var fromMidnight = (int)StudentCode.Call(method, 0, 0);
        Assert.True(fromMidnight == signOffHour * 60,
            $"Your station signs off at {signOffHour}:00, so at midnight there are "
            + $"{signOffHour * 60} minutes to go — and MinutesUntilSignOff(0, 0) returned "
            + $"{fromMidnight}.\n"
            + "Two steps: turn the time you're given into minutes past midnight (each hour is "
            + "worth 60), then subtract that from your sign-off time in minutes.\n"
            + "⚠️ Work it out from SignOffHour() rather than typing the number in. If you typed "
            + $"a number and it isn't {signOffHour * 60}, that's the mismatch.");

        var halfHourLeft = (int)StudentCode.Call(method, signOffHour - 1, 30);
        Assert.True(halfHourLeft == 30,
            $"At {signOffHour - 1}:30 your station has half an hour left, so "
            + $"MinutesUntilSignOff({signOffHour - 1}, 30) should be 30 — it returned "
            + $"{halfHourLeft}.\n"
            // (hour + minute) * 60 lands on this exact number whatever the sign-off
            // hour is: the 30 that was wanted, less 59 for each of the 30 minutes.
            + (halfHourLeft == 30 - 59 * 30
                ? "Being off by a multiple of 59 means the minutes got added before the hours "
                  + "were multiplied. Brackets fix it: (hour * 60) + minute."
                : "Check the order: multiply the hour by 60 FIRST, then add the minutes, then "
                  + "subtract the lot from your sign-off time."));

        var quarterPast = (int)StudentCode.Call(method, 1, 15);
        Assert.True(quarterPast == signOffHour * 60 - 75,
            $"MinutesUntilSignOff(1, 15) should be {signOffHour * 60 - 75} for a station that "
            + $"signs off at {signOffHour}:00 — it returned {quarterPast}.");
    }
}
