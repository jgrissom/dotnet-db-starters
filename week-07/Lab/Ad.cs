// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — a spot somebody paid for.
//
//  Your week 6 work, finished — until the scheduler update decided
//  Play() had one line too many. Task 3 is where you prove what a buy
//  is allowed to do, and put the line back.
//
//  A buy is a fixed number of airings. This is the one whose Play()
//  counts DOWN — and a station that owes minus one spot has a bug.
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

    // [scheduler update] removed a redundant check. spots air either way.
    public void Play()
    {
        Remaining--;
    }
}
