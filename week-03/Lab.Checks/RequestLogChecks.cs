// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as every week: these checks are how you know the
//  lab is done. Turn ❌ into ✅ by editing Lab/RequestLog.cs — never this
//  file.
//
//  Run them from your coursework folder — the one window you always have open:
//      dotnet test week-03/Lab.Checks
//
//  New this week: every check starts by emptying the log, because your two
//  collections are STATIC — one copy, shared by the whole program, alive for
//  as long as it runs. Without that reset, check 4 would be counting check
//  2's callers. Worth noticing: it's the same fact tonight ends on.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Checks;

public class RequestLogChecks
{
    // A clean desk before every check. Nothing to do with your work — see above.
    private static void FreshShift()
    {
        RequestLog.Tonight.Clear();
        RequestLog.Regulars.Clear();
    }

    [Fact] // passes out of the box — weeks 1 and 2 ship finished
    public void Check1_TheDeskStillAnswers()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — and Broadcast.cs ships finished "
            + $"this week, so if this check is red something in it got changed. (It returned: "
            + $"\"{Broadcast.CallSign()}\")\n"
            + "Tonight's work is all in Lab/RequestLog.cs.");

        Assert.True(CallerLine.CallerName("  Dorothy  ") == "Dorothy",
            "CallerLine.CallerName(\"  Dorothy  \") should return \"Dorothy\" — that's last "
            + "week's method, and it ships finished this week so nobody is behind.\n"
            + $"It returned: \"{CallerLine.CallerName("  Dorothy  ")}\"\n"
            + "Undo whatever changed in Lab/CallerLine.cs; tonight's file is RequestLog.cs.");

        Assert.True(CallerLine.TakeRequest("Dorothy", "something with strings")
                == "For Dorothy: something with strings.",
            "CallerLine.TakeRequest(\"Dorothy\", \"something with strings\") should return\n"
            + "    For Dorothy: something with strings.\n"
            + $"It returned:\n    {CallerLine.TakeRequest("Dorothy", "something with strings")}\n"
            + "That's last week's method, shipped finished. You'll be calling it tonight, so "
            + "it needs to be the way it left week 2.");
    }

    [Fact] // Task 2: the list
    public void Check2_TheNightIsKept()
    {
        FreshShift();

        var returned = RequestLog.Log("Dorothy", "something with strings");

        Assert.True(RequestLog.Tonight.Count == 1,
            $"After one call, RequestLog.Tonight holds {RequestLog.Tonight.Count} item(s) and "
            + "it should hold 1.\n"
            + "A List<Call> starts empty and grows when you put something on the end of it:\n"
            + "    Tonight.Add(new Call(name, onAir));\n"
            + "That's the whole difference between this week and last week — last week the "
            + "desk had nowhere to put a call.");

        Assert.True(returned == "For Dorothy: something with strings.",
            $"Log(\"Dorothy\", \"something with strings\") returned:\n    {returned}\n"
            + "...and it should hand back the on-air line, which CallerLine.TakeRequest "
            + "already knows how to build:\n"
            + "    string onAir = CallerLine.TakeRequest(caller, request);\n"
            + "    ...\n"
            + "    return onAir;\n"
            + "The shift reads that line out loud, which is why it comes back out of the "
            + "method as well as going into the list.");

        Assert.True(RequestLog.Tonight[0].OnAir == "For Dorothy: something with strings.",
            $"The Call you stored has OnAir = \"{RequestLog.Tonight[0].OnAir}\", and it should "
            + "be the on-air line:\n    For Dorothy: something with strings.\n"
            + "Store the same line you return — one line, in two places, built once.");

        Assert.True(RequestLog.Tonight[0].Caller == "Dorothy",
            $"The Call you stored has Caller = \"{RequestLog.Tonight[0].Caller}\" and it "
            + "should be \"Dorothy\".\n"
            + "Put the name through CallerLine.CallerName first — it's already the method "
            + "that knows how to clean a name up:\n"
            + "    string name = CallerLine.CallerName(caller);");

        RequestLog.Log("  Bex  ", "that one again");
        Assert.True(RequestLog.Tonight.Count == 2,
            $"After a second call, Tonight holds {RequestLog.Tonight.Count} and it should "
            + "hold 2. Every call goes on the END — Add() never replaces what's there.");

        Assert.True(RequestLog.Tonight[1].Caller == "Bex",
            $"The second call's Caller is \"{RequestLog.Tonight[1].Caller}\" — the spare "
            + "spaces came along for the ride, which means the name didn't go through "
            + "CallerLine.CallerName. Last week's method already solved trimming; call it.");

        var nobody = RequestLog.Log(null, null);
        Assert.True(nobody == "For some night owl: dealer's choice.",
            $"Log(null, null) returned:\n    {nobody}\n"
            + "A dead line is still a call. Because you're calling last week's methods, "
            + "this one is already handled — CallerName(null) is \"some night owl\" and a "
            + "blank request is \"dealer's choice\". If this is red, something in Log is "
            + "building the line itself instead of asking TakeRequest for it.");
    }

    [Fact] // Task 3: the dictionary, and the key that isn't there
    public void Check3_TheDeskKnowsItsRegulars()
    {
        FreshShift();

        Exception? crashed = Record.Exception(() => RequestLog.TimesCalled("Bex"));
        Assert.True(crashed == null,
            $"TimesCalled(\"Bex\") didn't return — it threw {crashed?.GetType().Name}.\n"
            + "Bex hasn't called tonight, and asking a dictionary for a key it hasn't got is "
            + "a KeyNotFoundException, not a zero:\n"
            + "    return Regulars[name];        // throws the first time anyone is new\n"
            + "The one that asks first has the shape you already know from int.TryParse:\n"
            + "    if (Regulars.TryGetValue(name, out int calls)) { return calls; }\n"
            + "    return 0;");

        Assert.True(RequestLog.TimesCalled("Bex") == 0,
            $"TimesCalled(\"Bex\") returned {RequestLog.TimesCalled("Bex")} before Bex has "
            + "rung at all, and it should be 0. Somebody who has never called has called "
            + "zero times — that's the answer TryGetValue lets you give instead of crashing.");

        RequestLog.Log("Dorothy", "something with strings");
        Assert.True(RequestLog.TimesCalled("Dorothy") == 1,
            $"Dorothy has rung once and TimesCalled(\"Dorothy\") says "
            + $"{RequestLog.TimesCalled("Dorothy")}.\n"
            + "Task 3 goes back into Log() and counts the caller as well as keeping the "
            + "line. The first time somebody rings there's no key to add to yet:\n"
            + "    if (Regulars.ContainsKey(name)) { Regulars[name] = Regulars[name] + 1; }\n"
            + "    else { Regulars[name] = 1; }\n"
            + "Setting Regulars[name] on a key that doesn't exist is fine — it CREATES it. "
            + "It's only READING a missing key that throws.");

        RequestLog.Log("Dorothy", "the slow one");
        RequestLog.Log("Dorothy", "anything, honestly");
        Assert.True(RequestLog.TimesCalled("Dorothy") == 3,
            $"Dorothy has now rung three times and TimesCalled(\"Dorothy\") says "
            + $"{RequestLog.TimesCalled("Dorothy")}. The count goes UP by one per call — if "
            + "you're seeing 1, the else branch is running every time and overwriting it.");

        Assert.True(RequestLog.TimesCalled("  Dorothy  ") == 3,
            $"TimesCalled(\"  Dorothy  \") says {RequestLog.TimesCalled("  Dorothy  ")} but "
            + "Dorothy with spaces round her is still Dorothy — three calls.\n"
            + "Put the name through CallerLine.CallerName in BOTH methods, so the name you "
            + "count under and the name you look up are always spelled the same way.");

        Assert.True(RequestLog.Regulars.Count == 1,
            $"Regulars holds {RequestLog.Regulars.Count} keys after three calls from one "
            + "person, and it should hold 1. A dictionary key is unique — Dorothy's fourth "
            + "call doesn't make a fourth Dorothy, it makes her number bigger. (That's the "
            + "difference between the list and the dictionary in one sentence.)");

        RequestLog.Log(null, null);
        Assert.True(RequestLog.TimesCalled(null) == 1,
            $"TimesCalled(null) says {RequestLog.TimesCalled(null)} after one anonymous "
            + "call, and it should say 1. Nameless callers all count under the same name — "
            + "whatever CallerName(null) returns — so they end up sharing one key. That's "
            + "correct, and quietly funny.");
    }

    [Fact] // Task 4: foreach over a dictionary
    public void Check4_TheDeskKnowsWhoWontStop()
    {
        FreshShift();

        Assert.True(RequestLog.TheRegular() == "nobody yet",
            $"With nobody having called, TheRegular() returned \"{RequestLog.TheRegular()}\" "
            + "and it should return exactly \"nobody yet\".\n"
            + "An empty dictionary means the foreach never runs even once — so whatever you "
            + "set up BEFORE the loop is what comes back. Start it at \"nobody yet\" and let "
            + "the loop overwrite it:\n"
            + "    string best = \"nobody yet\";\n"
            + "    int most = 0;\n"
            + "    foreach (KeyValuePair<string, int> entry in Regulars) { ... }");

        RequestLog.Log("Dorothy", "something with strings");
        RequestLog.Log("Bex", "that one again");
        RequestLog.Log("Bex", "that one again");
        RequestLog.Log("Bex", "you know the one");

        Assert.True(RequestLog.TheRegular() == "Bex",
            $"Bex has rung three times and Dorothy once, so TheRegular() should be \"Bex\" — "
            + $"it returned \"{RequestLog.TheRegular()}\".\n"
            + "Walk the dictionary and keep the best one so far:\n"
            + "    foreach (KeyValuePair<string, int> entry in Regulars)\n"
            + "    {\n"
            + "        if (entry.Value > most) { most = entry.Value; best = entry.Key; }\n"
            + "    }\n"
            + "entry.Key is the name, entry.Value is the count. (In week 9 this whole loop "
            + "becomes one line. It's worth writing by hand once first.)");

        RequestLog.Log("Dorothy", "the slow one");
        RequestLog.Log("Dorothy", "one more");
        RequestLog.Log("Dorothy", "last one, promise");
        RequestLog.Log("Dorothy", "sorry, one more");
        Assert.True(RequestLog.TheRegular() == "Dorothy",
            $"Dorothy has now rung five times to Bex's three, so TheRegular() should be "
            + $"\"Dorothy\" — it returned \"{RequestLog.TheRegular()}\".\n"
            + "If you're still getting Bex, the loop is stopping at the first person it "
            + "finds instead of comparing every one of them. There's no break in this loop: "
            + "you have to see everybody before you can know who called most.");
    }

    [Fact] // Task 5: the sign-off, built from both collections
    public void Check5_TheShiftAddsUp()
    {
        FreshShift();

        // ── blocked, not failed ────────────────────────────────────────────
        if (RequestLog.TheRegular() != "nobody yet")
        {
            throw new Xunit.Sdk.XunitException(
                "⏸ BLOCKED — this isn't a failure, and there's nothing wrong with your "
                + "Task 5.\n"
                + "SignOff() is built out of TheRegular(), and TheRegular() isn't finished "
                + "yet, so this check can't tell you anything true about SignOff.\n"
                + "👉 Next: finish Task 4, then run the checks again.");
        }

        Assert.True(RequestLog.SignOff() == "Nobody called. Not one person.",
            $"With an empty log, SignOff() returned:\n    {RequestLog.SignOff()}\n"
            + "...and a shift where the phone never rang gets its own line, exactly:\n"
            + "    Nobody called. Not one person.\n"
            + "Check Tonight.Count first and return early. (You'll see this line for real "
            + "at the end of tonight, and it won't be because you did anything wrong.)");

        RequestLog.Log("Dorothy", "something with strings");
        RequestLog.Log("Dorothy", "the slow one");
        RequestLog.Log("Bex", "that one again");
        RequestLog.Log("Teodoro", "for Junie, again");

        var signOff = RequestLog.SignOff();

        Assert.True(signOff.Contains("4"),
            $"Four calls came in and SignOff() returned:\n    {signOff}\n"
            + "...with no 4 in it. How many calls the desk took is Tonight.Count — ask the "
            + "list, don't count along in a separate variable. (Last week's desk had a "
            + "counter and nothing else. This is the upgrade.)");

        Assert.True(signOff.Contains("3"),
            $"Those four calls came from three different people and SignOff() returned:\n"
            + $"    {signOff}\n"
            + "...with no 3 in it. How many DIFFERENT people rang is Regulars.Count — one "
            + "key per person, however many times they called. Two collections, two "
            + "different questions, and this line is where the difference shows.");

        Assert.True(signOff.Contains("Dorothy"),
            $"Dorothy rang twice, more than anyone else, and SignOff() returned:\n"
            + $"    {signOff}\n"
            + "...without naming her. Ask TheRegular() rather than working it out again — "
            + "you already wrote that method, and two places that both know the answer is "
            + "one place too many.");
    }
}
