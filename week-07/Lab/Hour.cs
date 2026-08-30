// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one hour of radio.
//
//  Your week 6 work, finished — except that the scheduler update
//  reordered two lines in Run(). Task 5 is where you prove which order
//  the station is obliged to keep, and swap them back.
//
//  The rule at stake: the desk prints what actually went out over the
//  air — never what was about to.
// ═══════════════════════════════════════════════════════════════════

public class Hour
{
    private readonly List<IScheduleItem> _items = new List<IScheduleItem>();

    public void Add(IScheduleItem item)
    {
        _items.Add(item);
    }

    public int Count => _items.Count;

    // A copy, for the same reason as every week since four.
    public List<IScheduleItem> All()
    {
        return new List<IScheduleItem>(_items);
    }

    // The hour has to add up, and nothing here knows what a song is.
    public int TotalSeconds
    {
        get
        {
            int total = 0;

            foreach (IScheduleItem item in _items)
            {
                total += item.Seconds;
            }

            return total;
        }
    }

    // Put the hour on air. Every item plays itself, and hands back the
    // line the desk prints — which is a different line for every kind.
    public List<string> Run()
    {
        List<string> aired = new List<string>();

        foreach (IScheduleItem item in _items)
        {
            // [scheduler update] read the cue while it's fresh, then play.
            aired.Add($"{item.Kind} - {item.Cue}");
            item.Play();
        }

        return aired;
    }
}
