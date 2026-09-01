// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight broadcast desk.
//
//  Weeks 1 through 7, FINISHED. Clock's ":00" is last week's repair and
//  it ships already made — check 1 goes red if it comes back out.
//
//  Task 5 is the two empty methods at the bottom.
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

    // ── the air log ────────────────────────────────────────────────────────
    // The rotation is written afresh every sign-off. The air log is not: it
    // should get one more line every night and keep every line before it.

    // TODO — Task 5: add this line to the end of the file. There is a File
    // method that appends rather than overwrites, and it makes the file if it
    // isn't there yet — which is why the air log needs no setting-up. (check 5)
    public static void LogShift(string path, string line)
    {
    }

    // TODO — Task 5: the last line anybody wrote — or nothing at all, on a
    // desk that has never been signed off. An empty string is the honest
    // answer to "who was on before me?" when the answer is nobody. (check 5)
    public static string LastShift(string path)
    {
        return "";
    }
}
