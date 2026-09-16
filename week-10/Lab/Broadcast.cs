// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight broadcast desk.
//
//  ANSWER KEY — weeks 1 through 8, with week 9's Task 1 in: LastShift
//  walks the file in one line.
//
//  Clock's ":00" is week 7's repair and it stays: check 1 goes red if the
//  padding ever comes back out.
// ═══════════════════════════════════════════════════════════════════

public static class Broadcast
{
    // 6:00 AM, counted in minutes past midnight.
    private const int SunriseInMinutes = 360;

    public static string CallSign()
    {
        return "KDXR";
    }

    public static string SignOn(string djName)
    {
        return $"{CallSign()} 88.1 The Owl - you're on with {djName}. Keep it low and slow.";
    }

    public static int MinutesUntilSunrise(int hour, int minute)
    {
        int nowInMinutes = hour * 60 + minute;
        return SunriseInMinutes - nowInMinutes;
    }

    public static double HoursOnAir(int minutes)
    {
        return minutes / 60.0;
    }

    public static bool IsOvernight(int hour)
    {
        return hour >= 22 || hour < 6;
    }

    // Seconds into m:ss, for every LENGTH cell on the hour and the clock
    // at the bottom of it. The :00 is load-bearing: at least two digits,
    // pad with zero — "10:05", never "10:5".
    public static string Clock(int seconds)
    {
        return $"{seconds / 60}:{seconds % 60:00}";
    }

    // ── week 8: the air log ────────────────────────────────────────────────
    // The rotation is written afresh every sign-off. The air log is not: it
    // gets one more line every night and keeps every line before it.

    // Task 5. AppendAllText adds to the end of the file, and makes the file if
    // it isn't there yet. That is the whole difference between it and
    // WriteAllText, which starts the file over every single time.
    //
    // "\n" rather than Environment.NewLine, so the file reads the same
    // whichever machine wrote it. ReadAllLines below is happy with either.
    public static void LogShift(string path, string line)
    {
        File.AppendAllText(path, line + "\n");
    }

    // The last line anybody wrote — or nothing at all, on a desk that has
    // never been signed off. An empty string is the honest answer to
    // "who was on before me?" when the answer is nobody.
    //
    // Task 1 — week 8's promise, collected. Two of the three things this used
    // to do have gone: LastOrDefault reaches the end of the array without
    // being told the last index is one less than the length, and it copes with
    // an empty file on its own, so the second guard went with it.
    //
    // ⚠️ The File.Exists guard STAYS. ReadAllLines throws on a file that isn't
    // there — LINQ never gets a chance to be asked anything.
    public static string LastShift(string path)
    {
        if (!File.Exists(path))
        {
            return "";
        }

        return File.ReadAllLines(path).LastOrDefault() ?? "";
    }
}
