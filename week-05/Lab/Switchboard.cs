// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  The same shape as last week's Rotation: a class that owns a private
//  list, with the only ways in and out written on the outside. Those
//  three members ship FINISHED — you wrote them last week.
//
//  Tonight adds the two that are actually hard, and both of them are
//  about the same thing: what the desk does when there is nobody there.
// ═══════════════════════════════════════════════════════════════════

public class Switchboard
{
    // Everyone who has rung tonight. PRIVATE — the members below are the
    // only way in, which is why nothing outside can quietly empty it.
    private readonly List<Caller> _callers = new List<Caller>();

    // Week 4, finished. Put a caller on the board.
    public void Add(Caller caller)
    {
        _callers.Add(caller);
    }

    // Week 4, finished. Ask the list; never count along beside it.
    public int Count => _callers.Count;

    // Week 4, finished — and only half a copy, which is tonight's other
    // lesson. This hands back a new LIST. The callers in it are the same
    // callers, so anything you do to one of them, you do to the real one.
    public List<Caller> All()
    {
        return new List<Caller>(_callers);
    }

    // TODO — Task 3. Who is this?
    //
    // Walk the list and hand back the caller with that name. If nobody on
    // the board has it, hand back `null` — the return type says `Caller?`
    // rather than `Caller` precisely so that it is allowed to.
    //
    // It ships answering `null` to everything, which is why the desk
    // treats Dorothy like a stranger every time she rings.
    public Caller? Find(string name)
    {
        return null;
    }

    // TODO — Task 4. Somebody is on the line.
    //
    // One door for "a call came in", and it has to work for both kinds of
    // caller: a regular who is already on the board, and somebody who has
    // never rung before.
    //
    // Ask Find first. If it hands back a caller, that is the one — use it,
    // don't make a second Dorothy. If it hands back null, then this really
    // is a stranger: make one, put them on the board, and carry on.
    //
    // Either way the call gets counted, and the caller comes back out so
    // the desk can take their request.
    //
    // ⚠️ It ships making a brand-new caller every single time, which is
    // why a request never lands on the board.
    public Caller Take(string name)
    {
        return new Caller(name);
    }
}
