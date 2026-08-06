// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight broadcast desk.
//
//  Program.cs is the part a human sees. THIS is the part that has to be
//  right, and it's the part the checks can call.
//
//  Every method below is a shape you've written before, with every type
//  spelled out:
//
//      public static string SignOn(string djName)
//
//  The word before the name is the type of what comes BACK. The words in
//  the brackets are the types of what goes IN. That's the whole idea.
// ═══════════════════════════════════════════════════════════════════

public static class Broadcast
{
    // ✅ Done for you. Check 1 uses this to prove the station is on the air
    //    before you've written a line — it should be green out of the box.
    public static string CallSign()
    {
        return "KDXR";
    }

    // TODO — Task 2.
    // Return the sign-on line the DJ hears when their shift starts. It has to
    // contain the station's call sign AND the DJ's name, and you get the call
    // sign by CALLING the method above rather than typing "KDXR" again.
    public static string SignOn(string djName)
    {
        return "";
    }

    // TODO — Task 3.
    // The Owl runs midnight to 6:00 AM. Given the time right now as an hour and
    // a minute (so 2:15 AM arrives as hour = 2, minute = 15), return how many
    // minutes are left until 6:00.
    //   2:15 AM  →  225        // in minutes past midnight: 2:15 is 135, and 6:00 is 360
    public static int MinutesUntilSunrise(int hour, int minute)
    {
        return 0;
    }

    // TODO — Task 4.
    // Given a number of minutes already broadcast, return how many HOURS that
    // is — including the part hours.
    //   330 minutes  →  5.5    // five and a half hours on air
    // ⚠️ Read the check's message carefully when this one fails. It is the
    //    generator bug from the demo, arriving in your own code.
    public static double HoursOnAir(int minutes)
    {
        return 0;
    }

    // TODO — Task 5.
    // The overnight block runs from 22:00 through 05:59 — so 22, 23, 0, 1, 2,
    // 3, 4 and 5 are overnight hours, and 6 through 21 are not.
    // Return true when the given hour (0–23) falls in the overnight block.
    public static bool IsOvernight(int hour)
    {
        return false;
    }
}
