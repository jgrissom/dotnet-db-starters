// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one hour of radio.
//
//  This file is your INSTRUCTOR'S, not a task. Two pieces of it change
//  live in class: TotalSeconds, and the empty RunningOrder at the bottom.
//  You copy the finished file in, and you never edit it.
//
//  Run() stays a loop all night, and that is on purpose: it puts the hour
//  on the air, which is doing something rather than asking something.
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
            // Play FIRST, then read the cue. The desk prints what actually
            // happened, never what was about to.
            item.Play();
            aired.Add($"{item.Kind} - {item.Cue}");
        }

        return aired;
    }

    // The same lines Run() hands back, for the DJ to read at three minutes
    // to the hour. Your instructor writes this one live in class.
    public List<string> RunningOrder()
    {
        return new List<string>();
    }
}
