// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the caller line.
//
//  This is LAST WEEK'S lab, finished. If you built it yourself, this is
//  the same code. If you missed week 2, you're not behind — it ships
//  complete, and tonight starts from here.
//
//  Nothing in this file needs touching tonight, and check 1 goes red if
//  it gets changed by accident. Tonight's work is in RequestLog.cs.
//
//  Worth knowing what's in here, though, because you'll be CALLING these:
//    CallerName  — a name for every caller, even the ones who won't say
//    TakeRequest — the line the DJ reads on air
// ═══════════════════════════════════════════════════════════════════

public static class CallerLine
{
    public static string CallerName(string? typed)
    {
        if (string.IsNullOrWhiteSpace(typed))
        {
            return "some night owl";
        }
        return typed.Trim();
    }

    public static bool IsOnTheStretch(string? typed)
    {
        return int.TryParse(typed, out int marker) && marker >= 1 && marker <= 400;
    }

    public static string WhereIsRay(string? typed)
    {
        if (int.TryParse(typed, out int marker) && marker >= 1 && marker <= 400)
        {
            return $"Ray at mile {marker} - {400 - marker} to go on his stretch.";
        }
        return "Ray's out there somewhere. He'll call back.";
    }

    public static string TakeRequest(string? name, string? request)
    {
        string track = string.IsNullOrWhiteSpace(request) ? "dealer's choice" : request.Trim();
        return $"For {CallerName(name)}: {track}.";
    }
}
