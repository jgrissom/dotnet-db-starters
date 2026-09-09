// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight broadcast desk.
//
//  Weeks 1 through 8, finished. One method in here comes out tonight.
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

    // TODO — Task 1. The last line anybody wrote — or nothing at all, on a
    // desk that has never been signed off.
    //
    // Week 8's ⭐ Done early? item: "LastShift walks an array to reach the last
    // item… in week 9 that becomes one line."
    //
    // ⚠️ TWO of the three things in here can go. The File.Exists guard is not
    // one of them — work out why before you delete anything.
    public static string LastShift(string path)
    {
        if (!File.Exists(path))
        {
            return "";
        }

        string[] lines = File.ReadAllLines(path);

        if (lines.Length == 0)
        {
            return "";
        }

        return lines[lines.Length - 1];
    }
}
