// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one hour of radio.
//
//  Same shape as Rotation and Switchboard: a class that owns a private
//  list and hands out a copy of it. The only new thing is the type in the
//  angle brackets — and it is not a class.
//
//  Add, Count and All ship finished. Task 5 is the two members at the
//  bottom, and the second of them is what the whole night has been for.
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

    // TODO — Task 5. REPLACE this one. The hour has to add up: walk the items,
    // add up their Seconds, hand back the total. Nothing in here knows what a
    // song is.
    //
    // ⚠️ `=> 0` computes an answer rather than holding one, so there is no
    // room in it for a loop. It becomes `{ get { ... } }` — the shape is in
    // the notes.
    public int TotalSeconds => 0;

    // TODO — Task 5. Put the hour on air.
    //
    // Make a List<string> first — this ships handing back an empty one built
    // inside the return, and there is nowhere to put a line in that.
    //
    // Then walk the items. Play() each one — every kind counts that
    // differently and this loop will never find out how — and add one line
    // per item to your list, in this exact shape, so the desk can print it:
    //
    //     SONG - Nightjar - The Lamplighters
    //     ^^^^   ^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //     Kind    Cue, with " - " between them
    //
    // ⚠️ Play() the item BEFORE you read its Cue. An ad that has just aired
    // says one fewer left, and the desk should print what actually happened.
    public List<string> Run()
    {
        return new List<string>();
    }
}
