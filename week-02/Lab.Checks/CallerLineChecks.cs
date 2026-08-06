// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY — same deal as last week: these checks are how you know the
//  lab is done. Turn ❌ into ✅ by editing Lab/CallerLine.cs — never this
//  file.
//
//  Run them from your coursework folder — the one window you always have open:
//      dotnet test week-02/Lab.Checks
//
//  New this week: these checks hand your methods input that is null, blank,
//  or flatly lying — because that's what a phone line does. A method that
//  CRASHES on bad input fails its check just as hard as one that returns
//  the wrong answer, and the failure message will tell you which of those
//  happened. Read it.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Checks;

public class CallerLineChecks
{
    [Fact] // passes out of the box — last week's desk ships finished
    public void Check1_TheDeskSurvivedTheWeek()
    {
        Assert.True(Broadcast.CallSign() == "KDXR",
            "Broadcast.CallSign() should return \"KDXR\" — and Broadcast.cs ships finished "
            + "this week, so if this check is red, something in that file got changed by "
            + $"accident. (It returned: \"{Broadcast.CallSign()}\")\n"
            + "Tonight's work is all in Lab/CallerLine.cs. Undo whatever you last did in "
            + "Broadcast.cs and this goes green again.");

        var signOn = Broadcast.SignOn("Marisol");
        Assert.True(signOn.Contains("KDXR") && signOn.Contains("Marisol"),
            "Broadcast.SignOn(\"Marisol\") should still greet the DJ and name the station — "
            + $"it returned:\n    {signOn}\n"
            + "That method shipped finished. Undo your last change to Broadcast.cs.");
    }

    [Fact] // Task 2: null, blank, and the spare spaces
    public void Check2_TheLineCanBeDead()
    {
        string? gotNull;
        try
        {
            gotNull = CallerLine.CallerName(null);
        }
        catch (Exception e)
        {
            throw new Xunit.Sdk.XunitException(
                $"CallerName(null) didn't return — it threw {e.GetType().Name}.\n"
                + "null is what your method gets when there was no caller at all, and it "
                + "arrives BEFORE you can call .Trim() on it — trimming nothing is the crash. "
                + "Test for nothing first:\n"
                + "    if (string.IsNullOrWhiteSpace(typed)) { return \"some night owl\"; }\n"
                + "    return typed.Trim();\n"
                + "IsNullOrWhiteSpace catches null, \"\" and \"   \" in one question.");
        }

        Assert.True(gotNull == "some night owl",
            $"CallerName(null) returned \"{gotNull}\", and it should be exactly "
            + "\"some night owl\" — the desk's word for a caller who won't say.\n"
            + "null means nobody was on the line at all. The one-question test that covers "
            + "null, empty and whitespace together is:\n"
            + "    if (string.IsNullOrWhiteSpace(typed)) { return \"some night owl\"; }");

        Assert.True(CallerLine.CallerName("   ") == "some night owl",
            $"CallerName(\"   \") returned \"{CallerLine.CallerName("   ")}\" — three spaces "
            + "is nobody. A caller who breathed on the phone and hung up still isn't a name.\n"
            + "string.IsNullOrWhiteSpace treats null, \"\" and whitespace-only as the same "
            + "answer, which is exactly what the desk wants.");

        Assert.True(CallerLine.CallerName("  Dorothy  ") == "Dorothy",
            $"CallerName(\"  Dorothy  \") returned \"{CallerLine.CallerName("  Dorothy  ")}\" "
            + "and it should be \"Dorothy\" — name kept, spare spaces gone.\n"
            + "    return typed.Trim();\n"
            + "Trim() takes the whitespace off both ends and leaves the middle alone.");
    }

    [Fact] // Task 3: TryParse, and Ray's stretch
    public void Check3_TheMarkerHasToBeReal()
    {
        Exception? ex = Record.Exception(() => CallerLine.IsOnTheStretch("no idea, tumbleweeds"));
        Assert.True(ex == null,
            $"IsOnTheStretch(\"no idea, tumbleweeds\") didn't return — it threw {ex?.GetType().Name}.\n"
            + "That's int.Parse in there: Parse BELIEVES the input, and when the input isn't "
            + "a number it throws instead of answering. The tool that asks first is:\n"
            + "    int.TryParse(typed, out int marker)\n"
            + "It returns false for anything that isn't a whole number — including null — and "
            + "it never, ever throws.");

        Assert.True(CallerLine.IsOnTheStretch("240"),
            "IsOnTheStretch(\"240\") returned false, and mile 240 is a real place on Ray's "
            + "stretch (it runs mile 1 to mile 400).\n"
            + "The whole method is one line — parse it AND range-check it:\n"
            + "    return int.TryParse(typed, out int marker) && marker >= 1 && marker <= 400;\n"
            + "TryParse fills in `marker` when it succeeds, so the range check can use it "
            + "right there on the same line.");

        Assert.True(CallerLine.IsOnTheStretch("  240  "),
            "IsOnTheStretch(\"  240  \") returned false — but TryParse shrugs off spaces "
            + "around a number on its own. If you're getting false here while \"240\" works, "
            + "something is testing the raw string instead of letting TryParse read it.");

        Assert.False(CallerLine.IsOnTheStretch("9000"),
            "IsOnTheStretch(\"9000\") returned true. 9000 parses fine — it's a perfectly good "
            + "number — but Ray's stretch runs mile 1 to mile 400, and a marker that isn't on "
            + "it isn't a place he can be. Parsing and believing are still two different "
            + "steps: after TryParse succeeds, check the range.");

        Assert.False(CallerLine.IsOnTheStretch("0"),
            "IsOnTheStretch(\"0\") returned true, and the stretch starts at mile 1. "
            + "Check the boundary: >= 1, not >= 0.");

        Assert.False(CallerLine.IsOnTheStretch("240.5"),
            "IsOnTheStretch(\"240.5\") returned true — but mile markers are whole numbers, "
            + "and int.TryParse says no to \"240.5\" on its own. If this is red, something "
            + "other than int.TryParse is doing the parsing.");

        Assert.False(CallerLine.IsOnTheStretch(null),
            "IsOnTheStretch(null) returned true. null is no answer at all — TryParse "
            + "returns false for it without any special handling from you.");
    }

    [Fact] // Task 4: the day shift's method, made crash-proof
    public void Check4_RayCanSayAnything()
    {
        // The happy path first — this worked before you touched anything.
        var at240 = CallerLine.WhereIsRay("240");
        Assert.True(at240.Contains("240") && at240.Contains("160"),
            $"WhereIsRay(\"240\") returned:\n    {at240}\n"
            + "Mile 240 should keep the day shift's mile line — the marker (240) and the "
            + "miles left on his stretch (400 - 240 = 160). If you rewrote the whole method, "
            + "keep the working half working.");

        var ex = Record.Exception(() => CallerLine.WhereIsRay("somewhere past the truck stop"));
        Assert.True(ex == null,
            $"WhereIsRay(\"somewhere past the truck stop\") didn't return — it threw "
            + $"{ex?.GetType().Name}.\n"
            + "That's the day shift's int.Parse. It works every time Ray answers with a "
            + "number, and Ray doesn't always answer with a number — this crash is the whole "
            + "desk going down over how a man talks at 2 AM.\n"
            + "Same tool as Task 3:\n"
            + "    if (int.TryParse(typed, out int marker) && marker >= 1 && marker <= 400)\n"
            + "    {\n"
            + "        return $\"Ray at mile {marker} - {400 - marker} to go on his stretch.\";\n"
            + "    }\n"
            + "    return \"Ray's out there somewhere. He'll call back.\";");

        Assert.True(CallerLine.WhereIsRay("somewhere past the truck stop")
                == "Ray's out there somewhere. He'll call back.",
            $"WhereIsRay(\"somewhere past the truck stop\") returned:\n"
            + $"    {CallerLine.WhereIsRay("somewhere past the truck stop")}\n"
            + "No crash — good. But an answer that isn't a marker gets the desk's standard "
            + "line, exactly:\n"
            + "    Ray's out there somewhere. He'll call back.");

        Assert.True(CallerLine.WhereIsRay("9000") == "Ray's out there somewhere. He'll call back.",
            $"WhereIsRay(\"9000\") returned:\n    {CallerLine.WhereIsRay("9000")}\n"
            + "9000 parses — but it isn't on the stretch (mile 1 to 400), so it gets the "
            + "same standard line as words do. A number can lie too; that's why Task 3's "
            + "range check exists. Use it here.");

        var exNull = Record.Exception(() => CallerLine.WhereIsRay(null));
        Assert.True(exNull == null && CallerLine.WhereIsRay(null) == "Ray's out there somewhere. He'll call back.",
            "WhereIsRay(null) should get the standard line too — null is the line going "
            + "dead mid-call. TryParse handles null by returning false, so if Task 4 is "
            + "built on TryParse, this one is already done.");
    }

    [Fact] // Task 5: one line, built from your own methods
    public void Check5_EveryCallGetsAnAnswer()
    {
        var dorothy = CallerLine.TakeRequest("Dorothy", "something with strings");
        Assert.False(string.IsNullOrWhiteSpace(dorothy),
            "TakeRequest(\"Dorothy\", \"something with strings\") came back empty. This is "
            + "the line the DJ reads on air — build it with a $\"...\" string, from the "
            + "name and the request:\n"
            + "    return $\"For {CallerName(name)}: {...}.\";");

        Assert.True(dorothy.Contains("Dorothy") && dorothy.Contains("something with strings"),
            $"TakeRequest(\"Dorothy\", \"something with strings\") returned:\n    {dorothy}\n"
            + "The on-air line needs the caller's name and what they asked for — both.");

        var nobody = CallerLine.TakeRequest(null, null);
        Assert.True(nobody.Contains("some night owl"),
            $"TakeRequest(null, null) returned:\n    {nobody}\n"
            + "...and a nameless caller should show up as \"some night owl\" — which is "
            + "exactly what CallerName(null) already returns. Don't redo Task 2 in here; "
            + "CALL it:\n"
            + "    $\"For {CallerName(name)}: ...\"\n"
            + "One method that knows the rule beats two that might disagree.");

        Assert.True(nobody.Contains("dealer's choice"),
            $"TakeRequest(null, null) returned:\n    {nobody}\n"
            + "...and a caller with no request gets the desk's standard offer, exactly: "
            + "\"dealer's choice\". Same shape as Task 2's default — test for "
            + "IsNullOrWhiteSpace, fall back to the house wording.");

        var trimmed = CallerLine.TakeRequest("  Ray  ", "the long one");
        Assert.True(trimmed.Contains("Ray") && !trimmed.Contains("  Ray"),
            $"TakeRequest(\"  Ray  \", \"the long one\") returned:\n    {trimmed}\n"
            + "The spare spaces came along for the ride — which means the name didn't go "
            + "through CallerName. Task 2 already solved trimming; this task's job is to "
            + "USE it, not to solve it again.");
    }
}
