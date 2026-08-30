// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — a spot somebody paid for.
//
//  Task 4. Pham's Bakery bought three runs tonight. A buy is a fixed
//  number of airings, so this is the one whose Play() counts DOWN.
//
//  Two steps, same as the ident: keep the promise — `: IScheduleItem`
//  after the class name — then the four members. The second step should
//  be quicker this time.
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

    // Remaining ships finished and is already the right shape: how many runs
    // are left on the buy, readable anywhere, writable only in here. Leave it
    // alone. It is the thing your Play() spends.
    public int Remaining { get; private set; }

    // TODO — Task 4. Nothing above this line changes. Two things to add:
    //
    //   1. `: IScheduleItem` after the class name. Build before you write a
    //      member — the compiler lists what you owe, one CS0535 each.
    //   2. Those four members. Three are quick; Play() is the interesting
    //      one, because this is the kind that counts DOWN — every airing
    //      spends one run off Remaining.
    //      ⚠️ It never goes below zero. A station that owes minus one spot
    //      is a station with a bug, so ask before you spend.
    //
    //  Then take the `//` off the Ad line in Program.cs.
}
