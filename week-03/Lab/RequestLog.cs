// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the night's log.
//
//  Last week the desk took calls and forgot every one of them. It could
//  tell you HOW MANY came in and nothing else — you watched it say so at
//  the end of your shift.
//
//  Tonight it remembers. Two collections do the whole job:
//
//      List<Call>                 the night, in order
//      Dictionary<string, int>    who called, and how many times
//
//  Both of them live exactly as long as the program does. Hold that
//  thought — it's the last thing tonight is about.
// ═══════════════════════════════════════════════════════════════════

public static class RequestLog
{
    // The night, in order. Every call the desk takes goes on the end.
    public static List<Call> Tonight = new List<Call>();

    // Who called, and how many times each. The desk learns its regulars.
    // The KEY is the caller's name; the VALUE is how many times they've rung.
    public static Dictionary<string, int> Regulars = new Dictionary<string, int>();

    // TODO — Task 2.
    // Take a call: build the on-air line, keep it, and hand it back so the
    // shift can read it out.
    //   • CallerLine.TakeRequest(caller, request) already builds the line —
    //     call it, don't rebuild it.
    //   • CallerLine.CallerName(caller) already cleans up the name.
    //   • Put a new Call on the end of Tonight, and return the on-air line.
    // Task 3 comes back to this method and adds an if/else to it.
    public static string Log(string? caller, string? request)
    {
        return "";
    }

    // TODO — Task 3.
    // How many times has this person rung tonight?
    // ⚠️ Regulars[name] is NOT the tool. Asking a dictionary for a key it
    //    hasn't got THROWS — and "somebody who has never called" is the most
    //    ordinary question this method gets asked.
    //    The one that asks first is Dictionary.TryGetValue, and it has the
    //    exact shape you met last week in int.TryParse.
    // Somebody who has never called has called 0 times.
    public static int TimesCalled(string? caller)
    {
        return 0;
    }

    // TODO — Task 4.
    // Who has rung the most tonight? Walk the dictionary with a foreach,
    // keep the best one so far, hand back their name.
    // If nobody has called at all, return exactly:
    //     "nobody yet"
    public static string TheRegular()
    {
        return "";
    }

    // TODO — Task 5.
    // The line the desk signs off with. Two shapes:
    //   • nobody called at all — return exactly:
    //         "Nobody called. Not one person."
    //   • otherwise, a line containing how many calls came in, how many
    //     different people rang, and who would not stop. Something like:
    //         4 calls from 3 people. Dorothy would not stop.
    //     Ask Tonight and Regulars for their counts, and ask TheRegular()
    //     for the name — don't work any of it out a second time.
    public static string SignOff()
    {
        return "";
    }
}
