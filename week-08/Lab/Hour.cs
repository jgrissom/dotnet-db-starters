// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one hour of radio.
//
//  ANSWER KEY — week 6's Hour with Task 5's fix in: the update read
//  each item's cue before playing it, so the desk printed the buy as it
//  stood BEFORE the spot aired. Play first; then speak.
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
            // Task 5's fix: play FIRST, then read the cue. The desk prints
            // what actually happened, never what was about to.
            item.Play();
            aired.Add($"{item.Kind} - {item.Cue}");
        }

        return aired;
    }
}
