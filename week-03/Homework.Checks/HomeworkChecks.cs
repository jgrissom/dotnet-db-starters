// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this IS your grade. These are the exact checks I run
//  against your repo; I don't have a second, secret set.
//
//  Run them from your coursework folder — the one window you always have open:
//      dotnet test week-03/Homework.Checks
//
//  What's new this week: the checks look at a collection your class KEEPS,
//  not just at what your methods hand back. They also keep handing your code
//  people who never called, because "a key that isn't there" is the most
//  ordinary thing that happens to a dictionary and the thing that crashes it.
//
//  Your station's wording is still yours. What's fixed is the shape.
// ═══════════════════════════════════════════════════════════════════
using System.Text.RegularExpressions;

namespace Homework.Checks;

public class HomeworkChecks
{
    // Your collections are static — one copy, alive as long as the program runs.
    // Every check starts from an empty desk so it isn't counting another check's
    // callers. (That's a fact about static, not a comment on your code.)
    private static void FreshShift()
    {
        StudentCode.ClearIfPresent<List<string>>("Playlist", "Tonight");
        StudentCode.ClearIfPresent<Dictionary<string, int>>("Playlist", "Regulars");
    }

    private static List<string> TheList() =>
        StudentCode.RequireCollection<List<string>>(
            "Playlist", StudentCode.PlaylistHint, "Tonight",
            "public static List<string> Tonight = new List<string>();");

    private static Dictionary<string, int> TheDictionary() =>
        StudentCode.RequireCollection<Dictionary<string, int>>(
            "Playlist", StudentCode.PlaylistHint, "Regulars",
            "public static Dictionary<string, int> Regulars = new Dictionary<string, int>();");

    [Fact]
    public void Check1_TheStationCameForward()
    {
        var callSign = (string)StudentCode.Call(
            StudentCode.RequireMethod("Station", StudentCode.StationHint,
                "CallSign", typeof(string)));

        Assert.True(Regex.IsMatch(callSign, "^[A-Z]{4}$") && (callSign[0] == 'K' || callSign[0] == 'W'),
            $"Station.CallSign() returned \"{callSign}\" — same rule as weeks 1 and 2: four "
            + "capital letters, starting with K or W. If it passed last week it passes this "
            + "week untouched; the file just has to make the trip into week-03/Homework.");

        var callerName = StudentCode.RequireMethod("Switchboard", StudentCode.SwitchboardHint,
            "CallerName", typeof(string), typeof(string));

        Assert.True((string)StudentCode.Call(callerName, "  Marisol  ") == "Marisol",
            "Switchboard.CallerName(\"  Marisol  \") should still return \"Marisol\" — it "
            + $"returned \"{(string)StudentCode.Call(callerName, "  Marisol  ")}\".\n"
            + "That's week 2's method, carried forward. Tonight's Playlist is built on top "
            + "of it, so it has to arrive working.");

        var noName = (string)StudentCode.Call(callerName, new object?[] { null });
        Assert.False(string.IsNullOrWhiteSpace(noName),
            "Switchboard.CallerName(null) came back blank. That's your station's word for a "
            + "caller who won't say — you invented it in week 2, and tonight's log is going "
            + "to be full of it. Copy Switchboard.cs forward rather than retyping it.");
    }

    [Fact]
    public void Check2_TheNightIsKept()
    {
        FreshShift();
        var tonight = TheList();

        var take = StudentCode.RequireMethod("Playlist", StudentCode.PlaylistHint,
            "Take", typeof(string), typeof(string), typeof(string));

        var line = (string)StudentCode.Call(take, "Marisol", "something with strings");

        Assert.False(string.IsNullOrWhiteSpace(line),
            "Playlist.Take(\"Marisol\", \"something with strings\") came back empty. It "
            + "should hand back the line your station puts on air — whatever that sounds "
            + "like, as long as it names the caller and what they asked for:\n"
            + "    return $\"For {Switchboard.CallerName(caller)}: {request}.\";\n"
            + "The wording is yours. The two facts in it aren't.");

        Assert.True(line.Contains("Marisol"),
            $"Take(\"Marisol\", ...) returned:\n    {line}\n"
            + "...without the caller's name in it. The DJ reads this out loud; the caller "
            + "has to hear their own name.");

        Assert.True(line.Contains("something with strings"),
            $"Take(..., \"something with strings\") returned:\n    {line}\n"
            + "...without the request in it. That's the other half of what a request line "
            + "is for.");

        Assert.True(tonight.Count == 1,
            $"After one call, Playlist.Tonight holds {tonight.Count} item(s) and it should "
            + "hold 1.\n"
            + "Returning the line isn't enough — the whole point of tonight is that the "
            + "station KEEPS it:\n"
            + "    Tonight.Add(line);\n"
            + "A List<string> starts empty and grows on the end. Last week your station "
            + "could take a call and then had nowhere to put it.");

        Assert.True(tonight[0] == line,
            $"Tonight[0] holds:\n    {tonight[0]}\n"
            + $"...and Take returned:\n    {line}\n"
            + "Those should be the same line. Build it once, keep it, hand it back.");

        StudentCode.Call(take, "  Bex  ", "that one again");
        Assert.True(tonight.Count == 2,
            $"After a second call, Tonight holds {tonight.Count} and it should hold 2. Every "
            + "call goes on the END — Add() never replaces what's already there.");

        Assert.True(!tonight[1].Contains("  Bex"),
            $"The second line reads:\n    {tonight[1]}\n"
            + "...with the caller's spare spaces still in it. Put the name through "
            + "Switchboard.CallerName — week 2's method already solved trimming, and two "
            + "places that both know the rule is one place too many.");

        var crashed = StudentCode.CallExpectingSurvival(take, new object?[] { null, null });
        Assert.True(crashed == null,
            $"Take(null, null) didn't return — it threw {crashed?.GetType().Name}.\n"
            + "A dead line is still a call, and null is what your method gets when nobody "
            + "was there. Switchboard.CallerName already handles null; let it, and don't "
            + "call .Trim() on anything before it's been checked.");

        Assert.True(tonight.Count == 3,
            $"Tonight holds {tonight.Count} after three calls, and the third one was the "
            + "anonymous caller. A call from nobody is still a call — it goes in the log "
            + "like the rest.");
    }

    [Fact]
    public void Check3_TheDeskKnowsItsRegulars()
    {
        FreshShift();
        var regulars = TheDictionary();

        var take = StudentCode.RequireMethod("Playlist", StudentCode.PlaylistHint,
            "Take", typeof(string), typeof(string), typeof(string));
        var timesCalled = StudentCode.RequireMethod("Playlist", StudentCode.PlaylistHint,
            "TimesCalled", typeof(int), typeof(string));

        var crashed = StudentCode.CallExpectingSurvival(timesCalled, "Teodoro");
        Assert.True(crashed == null,
            $"TimesCalled(\"Teodoro\") didn't return — it threw {crashed?.GetType().Name}.\n"
            + "Teodoro hasn't called tonight, and asking a Dictionary for a key it hasn't "
            + "got THROWS rather than answering 0:\n"
            + "    return Regulars[name];        // KeyNotFoundException, the first time\n"
            + "The one that asks first has the shape you already know from int.TryParse:\n"
            + "    if (Regulars.TryGetValue(name, out int calls)) { return calls; }\n"
            + "    return 0;");

        Assert.True((int)StudentCode.Call(timesCalled, "Teodoro") == 0,
            $"TimesCalled(\"Teodoro\") returned "
            + $"{(int)StudentCode.Call(timesCalled, "Teodoro")} before Teodoro has called at "
            + "all, and it should return 0. Somebody who has never rung has rung zero times "
            + "— that's the answer TryGetValue lets you give instead of crashing.");

        StudentCode.Call(take, "Marisol", "the slow one");
        Assert.True((int)StudentCode.Call(timesCalled, "Marisol") == 1,
            $"Marisol has rung once and TimesCalled(\"Marisol\") says "
            + $"{(int)StudentCode.Call(timesCalled, "Marisol")}.\n"
            + "Take() has to count the caller as well as keeping the line. The first time "
            + "somebody rings there's no number to add to yet:\n"
            + "    if (Regulars.ContainsKey(name)) { Regulars[name] = Regulars[name] + 1; }\n"
            + "    else { Regulars[name] = 1; }\n"
            + "Setting Regulars[name] on a key that doesn't exist is fine — that CREATES "
            + "it. It's only READING a missing key that throws.");

        StudentCode.Call(take, "Marisol", "one more");
        StudentCode.Call(take, "  Marisol  ", "sorry, last one");
        Assert.True((int)StudentCode.Call(timesCalled, "Marisol") == 3,
            $"Marisol has now rung three times — the third with spaces round her name — and "
            + $"TimesCalled(\"Marisol\") says "
            + $"{(int)StudentCode.Call(timesCalled, "Marisol")}.\n"
            + "Two things this catches: the count going UP by one per call (if you're stuck "
            + "on 1, the else branch is running every time and overwriting it), and the "
            + "name being cleaned the SAME way in both methods — put it through "
            + "Switchboard.CallerName in each, and \"  Marisol  \" can't become a second "
            + "person.");

        Assert.True((int)StudentCode.Call(timesCalled, "  Marisol  ") == 3,
            $"TimesCalled(\"  Marisol  \") says "
            + $"{(int)StudentCode.Call(timesCalled, "  Marisol  ")}, but Marisol with spaces "
            + "round her is still Marisol — three calls.\n"
            + "Your program hands this method whatever the caller typed, spaces and all, so "
            + "the cleaning has to happen in HERE as well as in Take:\n"
            + "    string name = Switchboard.CallerName(caller);\n"
            + "Store under one spelling and look up under another and the two never meet — "
            + "the key is there, the lookup misses it, and you get 0 for somebody who has "
            + "rung all night.");

        Assert.True(regulars.Count == 1,
            $"Playlist.Regulars holds {regulars.Count} keys after three calls from one "
            + "person, and it should hold 1. A dictionary key is unique: Marisol's third "
            + "call doesn't make a third Marisol, it makes her number bigger. That's the "
            + "difference between your list and your dictionary, in one sentence — the "
            + "list is every call, the dictionary is every caller.");
    }

    [Fact]
    public void Check4_TheSignOffTellsTheTruth()
    {
        FreshShift();

        var signOff = StudentCode.RequireMethod("Playlist", StudentCode.PlaylistHint,
            "SignOff", typeof(string));
        var take = StudentCode.RequireMethod("Playlist", StudentCode.PlaylistHint,
            "Take", typeof(string), typeof(string), typeof(string));

        var quiet = (string)StudentCode.Call(signOff);
        Assert.False(string.IsNullOrWhiteSpace(quiet),
            "With nobody having called, Playlist.SignOff() came back blank. A shift where "
            + "the phone never rang still gets a line — yours to write, and you'll be "
            + "reading it more often than you'd think:\n"
            + "    if (Tonight.Count == 0) { return \"...\"; }");

        StudentCode.Call(take, "Marisol", "something with strings");
        StudentCode.Call(take, "Marisol", "the slow one");
        StudentCode.Call(take, "Bex", "that one again");
        StudentCode.Call(take, "Teodoro", "for Junie, again");

        var busy = (string)StudentCode.Call(signOff);

        Assert.True(busy != quiet,
            $"SignOff() says the same thing after four calls as it did after none:\n"
            + $"    {busy}\n"
            + "The whole reason the station keeps a list is so it can say something true "
            + "about the night. Ask Tonight.Count and let the answer change.");

        Assert.True(busy.Contains("4"),
            $"Four calls came in and SignOff() returned:\n    {busy}\n"
            + "...with no 4 in it. How many calls came in is Tonight.Count — ask the list. "
            + "(Counting along in a separate int variable is what your station did last "
            + "week, and last week it couldn't tell you anything else.)");
    }
}
