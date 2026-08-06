// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — and this time it IS your grade. These are the exact checks
//  I run against your repo; I don't have a second, secret set.
//
//  Run them from your coursework folder — the one window you always have open:
//      dotnet test week-02/Homework.Checks
//
//  Notice what's new this week: several checks hand your methods null,
//  blank text, and answers that only claim to be numbers — because that's
//  what this week is about. A method that crashes on ugly input fails its
//  check exactly the way it would fail a caller. The message says which
//  input did it; read the message.
// ═══════════════════════════════════════════════════════════════════
using System.Text.RegularExpressions;

namespace Homework.Checks;

public class HomeworkChecks
{
    [Fact]
    public void Check1_TheStationCameForward()
    {
        var method = StudentCode.RequireMethod("Station", StudentCode.StationHint,
            "CallSign", typeof(string));
        var callSign = (string)StudentCode.Call(method);

        Assert.False(string.IsNullOrWhiteSpace(callSign),
            "Station.CallSign() came back empty. This is your week 1 station carried "
            + "forward — copy Station.cs from week-01/Homework into week-02/Homework, or "
            + "type the minimum version from the homework if you're starting fresh.");

        Assert.True(Regex.IsMatch(callSign, "^[A-Z]{4}$") && (callSign[0] == 'K' || callSign[0] == 'W'),
            $"Station.CallSign() returned \"{callSign}\" — same rule as week 1: four capital "
            + "letters, starting with K or W. If it passed last week, it passes this week "
            + "untouched; if it didn't, this is the week to fix it.");
    }

    [Fact]
    public void Check2_TheGreetingSaysWhoYouAre()
    {
        var callSign = (string)StudentCode.Call(
            StudentCode.RequireMethod("Station", StudentCode.StationHint, "CallSign", typeof(string)));

        var method = StudentCode.RequireMethod("Switchboard", StudentCode.SwitchboardHint,
            "Greeting", typeof(string));
        var greeting = (string)StudentCode.Call(method);

        Assert.False(string.IsNullOrWhiteSpace(greeting),
            "Switchboard.Greeting() came back empty. It's the line your station answers the "
            + "phone with — whatever your station would say, as long as it says who it is.");

        Assert.True(greeting.Contains(callSign),
            $"Switchboard.Greeting() returned:\n    {greeting}\n"
            + $"...and your call sign, \"{callSign}\", isn't in it. A caller has to know "
            + "what they've dialled.\n"
            + "Build it by CALLING your own class rather than retyping the letters:\n"
            + "    return $\"{Station.CallSign()} request line, what'll it be?\";\n"
            + "Same rule as week 1's sign-on: two places that both know the call sign is "
            + "one place too many.");
    }

    [Fact]
    public void Check3_ACallerCanBeNobody()
    {
        var method = StudentCode.RequireMethod("Switchboard", StudentCode.SwitchboardHint,
            "CallerName", typeof(string), typeof(string));

        var crashed = StudentCode.CallExpectingSurvival(method, new object?[] { null });
        Assert.True(crashed == null,
            $"Switchboard.CallerName(null) didn't return — it threw {crashed?.GetType().Name}.\n"
            + "null is what your method gets when there was no caller at all, and it arrives "
            + "BEFORE you can call .Trim() on it. Ask the one question that covers null, "
            + "empty and whitespace together:\n"
            + "    if (string.IsNullOrWhiteSpace(typed)) { return \"...your default...\"; }\n"
            + "    return typed.Trim();");

        var forNull = (string)StudentCode.Call(method, new object?[] { null });
        Assert.False(string.IsNullOrWhiteSpace(forNull),
            "Switchboard.CallerName(null) returned nothing (or only whitespace). A caller "
            + "who won't give a name still needs to be called SOMETHING on air — that "
            + "something is yours to invent, and it can't be blank.");

        var forEmpty = (string)StudentCode.Call(method, "");
        var forSpaces = (string)StudentCode.Call(method, "   ");
        Assert.True(forEmpty == forNull && forSpaces == forNull,
            $"Your no-name default isn't consistent: CallerName(null) says \"{forNull}\", "
            + $"CallerName(\"\") says \"{forEmpty}\", CallerName(\"   \") says \"{forSpaces}\".\n"
            + "null, empty and whitespace-only are all the same situation — nobody there — "
            + "and string.IsNullOrWhiteSpace treats them as one question, which keeps the "
            + "answer consistent for free.");

        var trimmed = (string)StudentCode.Call(method, "  Marisol  ");
        Assert.True(trimmed == "Marisol",
            $"CallerName(\"  Marisol  \") returned \"{trimmed}\" and it should be "
            + "\"Marisol\" — a real name gets kept, with the spare spaces trimmed off:\n"
            + "    return typed.Trim();");
    }

    [Fact]
    public void Check4_TheContestCantBeCrashed()
    {
        var luckyMethod = StudentCode.RequireMethod("Switchboard", StudentCode.SwitchboardHint,
            "LuckyCallerNumber", typeof(int));
        var lucky = (int)StudentCode.Call(luckyMethod);

        Assert.True(lucky >= 1 && lucky <= 100,
            $"Switchboard.LuckyCallerNumber() returned {lucky}. The homework asks for a "
            + "number between 1 and 100 — it's a call-in contest, not a lottery. (The limit "
            + "is what lets me test your contest against YOUR number instead of one I "
            + "picked.)");

        var method = StudentCode.RequireMethod("Switchboard", StudentCode.SwitchboardHint,
            "IsWinner", typeof(bool), typeof(string));

        // Every expectation below is built from the student's OWN lucky number.
        Assert.True((bool)StudentCode.Call(method, lucky.ToString()),
            $"Your lucky number is {lucky}, but IsWinner(\"{lucky}\") returned false — the "
            + "right caller just lost your contest.\n"
            + "Parse the text, then compare with what LuckyCallerNumber() says:\n"
            + "    return int.TryParse(typed, out int caller) && caller == LuckyCallerNumber();\n"
            + "⚠️ Work it out from LuckyCallerNumber() rather than typing the number again — "
            + "if you typed a number and this is red, the two have already disagreed.");

        Assert.True((bool)StudentCode.Call(method, $"  {lucky}  "),
            $"IsWinner(\"  {lucky}  \") returned false — but TryParse shrugs off spaces "
            + "around a number on its own. If the bare number wins and the padded one "
            + "doesn't, something is comparing the raw text instead of the parsed value.");

        Assert.False((bool)StudentCode.Call(method, (lucky + 1).ToString()),
            $"IsWinner(\"{lucky + 1}\") returned true, and your lucky number is {lucky}. "
            + "Off by one wins your contest. Compare with == against LuckyCallerNumber(), "
            + "not with anything looser.");

        var crashedWords = StudentCode.CallExpectingSurvival(method, "tonight? me? really?");
        Assert.True(crashedWords == null,
            $"IsWinner(\"tonight? me? really?\") didn't return — it threw "
            + $"{crashedWords?.GetType().Name}.\n"
            + "That's int.Parse believing a caller. People don't answer contests with "
            + "digits; int.TryParse returns false for words instead of throwing, and false "
            + "is exactly the right answer here.");

        Assert.False((bool)StudentCode.Call(method, "tonight? me? really?"),
            "IsWinner(\"tonight? me? really?\") returned true. Words aren't a caller "
            + "number — TryParse would have said false for you.");

        var crashedNull = StudentCode.CallExpectingSurvival(method, new object?[] { null });
        Assert.True(crashedNull == null && !(bool)StudentCode.Call(method, new object?[] { null }),
            "IsWinner(null) should be false, without crashing — null is a dead line, and a "
            + "dead line didn't win. int.TryParse handles null by returning false; if your "
            + "method is built on TryParse, this is already done.");

        Assert.False((bool)StudentCode.Call(method, ""),
            "IsWinner(\"\") returned true — an empty answer won your contest. TryParse "
            + "says false for empty text; let it.");
    }
}
