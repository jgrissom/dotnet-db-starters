// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  Two working loops come out tonight.
//
//  ⭐ TheRegular is the one to look at first. You wrote this method in
//  WEEK 3, over a Dictionary<string, int> — two variables before the loop
//  and one comparison inside it. Same question, different list. Week 3
//  said: "in week 9 this whole loop becomes one line."
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
    // Take hands back THE caller on the board, and only news one up when Find
    // comes back empty. Both roads end in the same place: the call is counted
    // on whoever is handed back.
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

    // ── two loops, and tonight they are one line each ──────────────────────

    // TODO — Task 1. Every call the desk has taken tonight, added up.
    //
    // Week 5's ⭐ Done early? item: "a TotalCalls that loops every caller
    // adding up their count… in week 9 that entire loop becomes one line."
    public int TotalCalls
    {
        get
        {
            int total = 0;

            foreach (Caller caller in _callers)
            {
                total += caller.CallsTonight;
            }

            return total;
        }
    }

    // TODO — Task 1. Who would not stop calling.
    //
    // Week 3's, moved onto the list of callers you have had since week 5. Two
    // variables before the loop, one comparison inside it.
    //
    // ⚠️ "nobody yet" is not decoration. A board nobody has rung has no
    // busiest caller, and this method has answered that with those exact two
    // words since week 3. Whatever replaces the loop still has to say them.
    public string TheRegular()
    {
        string best = "nobody yet";
        int most = 0;

        foreach (Caller caller in _callers)
        {
            if (caller.CallsTonight > most)
            {
                most = caller.CallsTonight;
                best = caller.Name;
            }
        }

        return best;
    }
}
