// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight broadcast desk.
//
//  Weeks 1 through 6, finished — until last night, when the scheduler
//  update came through. It touched one line in here. Task 2 is where
//  you prove what that line should do, and then put it right.
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
    // at the bottom of it.
    //
    // [scheduler update] simplified the format string here. cleaner.
    public static string Clock(int seconds)
    {
        return $"{seconds / 60}:{seconds % 60}";
    }
}
