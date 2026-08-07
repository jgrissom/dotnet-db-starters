// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight broadcast desk.
//
//  Weeks 1 through 3, FINISHED. If you built it yourself, this is the same
//  desk. If you missed a week, you're not behind — it ships complete, and
//  tonight starts from here.
//
//  Nothing in this file needs touching tonight. Check 1 goes red if it
//  gets changed by accident.
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
}
