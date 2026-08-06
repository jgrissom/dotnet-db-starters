// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the caller line.
//
//  Broadcast.cs keeps the station on the air. THIS file answers the phone.
//  The phone is where programs die, because everything a caller says
//  arrives as a string typed by a tired human at 2 AM.
//
//  Rule of the night: typed input can be null (nobody there), it can be
//  blank, and it can claim to be a number and lie. Every method below has
//  to take all three without crashing. That's not politeness — a crashed
//  desk is dead air.
// ═══════════════════════════════════════════════════════════════════

public static class CallerLine
{
    // TODO — Task 2.
    // The log wants a name for every call, and callers don't always give one.
    // Return the name with the spare spaces trimmed off — and if what you got
    // is null, empty, or nothing but whitespace, return exactly:
    //     "some night owl"
    // That's the desk's word for a caller who won't say. Spelling matters —
    // the on-air line in Task 5 is built from it.
    public static string CallerName(string? typed)
    {
        return "";
    }

    // TODO — Task 3.
    // Ray drives the same stretch of interstate every night, and his stretch
    // runs from mile 1 to mile 400. Given whatever the DJ typed when Ray said
    // where he was, return true only if it's a whole number in that range.
    // ⚠️ int.Parse is not the tool. Parse BELIEVES the input. You want the
    //    one that asks first: int.TryParse. The notes show it done.
    public static bool IsOnTheStretch(string? typed)
    {
        return false;
    }

    // ✅ Task 4 — already written, by the day shift. It works every single
    //    time Ray answers with a number. The day shift never heard how Ray
    //    actually talks.
    //    Your job: make it survive anything a human says, with the same tool
    //    as Task 3. A marker on the stretch keeps the mile line below.
    //    Anything else — words, nothing at all, mile 9000 — returns exactly:
    //        "Ray's out there somewhere. He'll call back."
    public static string WhereIsRay(string? typed)
    {
        int marker = int.Parse(typed ?? "");
        return $"Ray at mile {marker} - {400 - marker} to go on his stretch.";
    }

    // TODO — Task 5.
    // The line the DJ reads on air. Rules:
    //   • the name goes through CallerName — CALL it, don't redo the trimming
    //   • a null or blank request becomes exactly: "dealer's choice"
    //   • the line contains the name and the request, shaped like:
    //         For Dorothy: something with strings.
    public static string TakeRequest(string? name, string? request)
    {
        return "";
    }
}
