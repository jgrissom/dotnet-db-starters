// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — a spot somebody paid for.
//
//  Task 4. Pham's Bakery bought three runs tonight. A buy is a fixed
//  number of airings, so this is the one whose Play() counts DOWN.
//
//  Same two steps as the ident, and the second one should be quicker.
// ═══════════════════════════════════════════════════════════════════

public class Ad
{
    public string Sponsor { get; }
    public string Copy { get; }

    public Ad(string sponsor, string copy, int runs)
    {
        Sponsor = sponsor;
        Copy = copy;
        Remaining = runs;
    }

    // TODO — Task 4.
    //
    //   1. `: IScheduleItem` after the class name. Build, read the list.
    //   2. Remaining is spelled that way because the checks read it by
    //      name — it is how many runs are left on the buy, readable
    //      anywhere, and Play() is the only thing that spends one.
    //      ⚠️ It never goes below zero. A station that owes minus one
    //      spot is a station with a bug.
    //   3. A spot is thirty seconds, always.
    //
    //  Then take the `//` off the Ad line in Program.cs.
    public int Remaining { get; private set; }
}
