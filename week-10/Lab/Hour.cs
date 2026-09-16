// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one hour of radio.
//
//  ANSWER KEY — week 6's Hour with week 9's work in: two loops collapsed
//  (Task 1) and one new question (Task 5).
//
//  ⚠️ The pair to look at is Run() and RunningOrder(). They hand back the
//  same strings and they are not the same method: one of them AIRS the
//  hour on the way past. That is the difference between doing and asking,
//  and it is the whole week in two methods.
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
    //
    // Task 1 — week 7's promise, collected. Four lines of running total gone;
    // the suite that was watching this method is still green.
    public int TotalSeconds => _items.Sum(item => item.Seconds);

    // The longest thing in the hour, whatever kind of thing it turns out to be.
    //
    // Task 1 — week 6's promise, collected. The loop kept a "best so far" and
    // a "longest so far" and compared; MaxBy is that paragraph.
    //
    // ⚠️ It hands back IScheduleItem? — nullable — because an empty hour has
    // no longest anything, and MaxBy says so with null rather than inventing
    // one. The old loop said it with a `best` that started at null too.
    public IScheduleItem? LongestItem() => _items.MaxBy(item => item.Seconds);

    // Put the hour on air. Every item plays itself, and hands back the
    // line the desk prints — which is a different line for every kind.
    //
    // ⚠️ STAYS A LOOP, and it is the most important thing in this file that
    // did NOT change tonight. There is a Select that builds these strings in
    // one line — and it would have to call Play() inside the lambda to do it,
    // which means the hour would go out over the air as a side effect of
    // somebody asking a question. A query asks. This does something.
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

    // Task 5. The same lines Run() prints, and nothing goes out over the air.
    //
    // This is the one the DJ wants at three minutes to the hour: what is
    // coming, in order, so it can be read off the screen before it happens.
    // Every count in the hour is exactly where it was when this was called.
    public List<string> RunningOrder()
    {
        return _items.Select(item => $"{item.Kind} - {item.Cue}").ToList();
    }
}
