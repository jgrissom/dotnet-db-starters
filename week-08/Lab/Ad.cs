// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — a spot somebody paid for.
//
//  ANSWER KEY — week 6's Ad with Task 3's fix in: the update deleted
//  the guard in Play(), and a one-run buy aired three times went to -2.
//  The student's test proves it red first; the guard turns it green.
//
//  A buy is a fixed number of airings. This is the one whose Play()
//  counts DOWN — and airing more spots than the sponsor bought is a bug.
// ═══════════════════════════════════════════════════════════════════

public class Ad : IScheduleItem
{
    public string Sponsor { get; }
    public string Copy { get; }

    // How many runs are left on the buy. Only Play() spends one.
    public int Remaining { get; private set; }

    public string Kind => "AD";

    public string Cue => $"{Sponsor} - \"{Copy}\" ({Remaining} left)";

    // A spot is thirty seconds. It has been thirty seconds since radio began.
    public int Seconds => 30;

    public Ad(string sponsor, string copy, int runs)
    {
        Sponsor = sponsor;
        Copy = copy;
        Remaining = runs;
    }

    // Task 3's fix: the guard is back. Every airing that has a run to
    // spend, spends one — and the station never airs a spot nobody bought.
    public void Play()
    {
        if (Remaining > 0)
        {
            Remaining--;
        }
    }
}
