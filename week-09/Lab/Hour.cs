// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one hour of radio.
//
//  Two working loops come out tonight, and one empty method gets filled in.
//
//  ⚠️ Run() is the one that does NOT change. Read it before you touch
//  anything else in here — it is the method tonight's last task is
//  measured against.
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

    // TODO — Task 1. The hour has to add up, and nothing here knows what a
    // song is.
    //
    // This is the loop week 7 pointed at: "your Check5 and the hour's
    // TotalSeconds both walk a list. In week 9 every one of those loops becomes
    // one line — and your tests will be how you prove the one-liners do the
    // same job."
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

    // TODO — Task 1. The longest thing in the hour, whatever kind of thing it
    // turns out to be.
    //
    // Week 6's ⭐ Done early? item: "in week 9 that whole loop becomes one
    // line." Here it is, working.
    //
    // ⚠️ It hands back IScheduleItem? — nullable — because an empty hour has no
    // longest anything. Whatever replaces this still has to answer that.
    public IScheduleItem? LongestItem()
    {
        IScheduleItem? longest = null;

        foreach (IScheduleItem item in _items)
        {
            if (longest == null || item.Seconds > longest.Seconds)
            {
                longest = item;
            }
        }

        return longest;
    }

    // Put the hour on air. Every item plays itself, and hands back the
    // line the desk prints — which is a different line for every kind.
    //
    // ⚠️ THIS ONE STAYS EXACTLY AS IT IS. It does not just build strings; it
    // AIRS the hour on the way past, and every count in the station moves
    // because of the item.Play() below.
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

    // ── week 9: reading the hour without airing it ─────────────────────────

    // TODO — Task 5. The same lines Run() hands back, with nothing going out
    // over the air.
    //
    // This is what the DJ wants at three minutes to the hour: what is coming,
    // in order, so it can be read off the screen before it happens.
    //
    // ⚠️ Every count in the hour must be exactly where it was when this was
    // called. Read Run() above, and then be careful about which part of it you
    // borrow.
    public List<string> RunningOrder()
    {
        return new List<string>();
    }
}
