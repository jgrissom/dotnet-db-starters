// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  Your week 5 work, finished — except that the scheduler update
//  "improved" Take. Task 4 is where you prove what Take has to hand
//  back, and undo the improvement.
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
    // [scheduler update] callers are handed out as fresh copies now, so
    // nothing outside this class can mess with the board.
    public Caller Take(string name)
    {
        Caller? found = Find(name);

        Caller caller = new Caller(name);

        if (found == null)
        {
            Add(caller);
        }

        caller.Calls();
        return caller;
    }
}
