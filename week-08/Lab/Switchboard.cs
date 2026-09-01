// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  ANSWER KEY — week 5's Switchboard with Task 4's fix in: the update
//  made Take hand out fresh copies, so a regular's calls landed on a
//  ghost. Assert.Same is the test that catches it.
//
//  Find is untouched and it is the half to lean on: it hands back THE
//  caller on the board, or nothing. Never a new one with the same name.
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

    // Walk the board; hand back the caller, or nothing.
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

    // One door for "a call came in".
    //
    // Task 4's fix: Take hands back THE caller on the board, and only news
    // one up when Find comes back empty. Both roads end in the same place:
    // the call is counted on whoever is handed back.
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
