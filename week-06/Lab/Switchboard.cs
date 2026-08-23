// ═══════════════════════════════════════════════════════════════════
//  ANSWER KEY — KDXR 88.1 "The Owl", who has rung the desk tonight.
//
//  Add / Count / All ship finished (week 4). Tasks 3 and 4 are the two
//  new members, and they are one idea in two halves: Find is allowed to
//  come back with nothing, and Take is the place that copes with it.
//
//  ⚠️ The reference-identity beat lives here, and it is what checks 3 and
//  4 actually assert: Find hands back THE caller on the board, never a
//  new one with the same name. Take then hands back that same object
//  again. Two names for one person.
// ═══════════════════════════════════════════════════════════════════

public class Switchboard
{
    private readonly List<Caller> _callers = new List<Caller>();

    public void Add(Caller caller)
    {
        _callers.Add(caller);
    }

    public int Count => _callers.Count;

    public List<Caller> All()
    {
        return new List<Caller>(_callers);
    }

    // Task 3. Walk the board; hand back the caller, or nothing.
    // The `return null` goes AFTER the loop — inside it, the method gives
    // up on the first caller whose name doesn't match.
    public Caller? Find(string name)
    {
        foreach (Caller caller in _callers)
        {
            if (caller.Name == name)
            {
                return caller;
            }
        }

        return null;
    }

    // Task 4. One door for "a call came in".
    //
    // Both roads end in the same place: the call is counted whether they
    // were already on the board or not, so Calls() sits after the `if`
    // rather than in one branch of it.
    public Caller Take(string name)
    {
        Caller? caller = Find(name);

        if (caller == null)
        {
            caller = new Caller(name);
            Add(caller);
        }

        caller.Calls();
        return caller;
    }
}
