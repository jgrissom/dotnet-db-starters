// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — a spot somebody paid for.
//
//  FINISHED. Nothing in here changes tonight.
//
//  A buy is a fixed number of airings. This is the one whose Play()
//  counts DOWN — which makes it the item to watch when something airs
//  that should only have been read.
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

    // Every airing that has a run to spend, spends one — and the station
    // never airs a spot nobody bought.
    public void Play()
    {
        if (Remaining > 0)
        {
            Remaining--;
        }
    }
}
