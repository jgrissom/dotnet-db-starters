// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  ANSWER KEY — week 5's Switchboard with week 9's Task 1 in: two loops
//  collapsed.
//
//  ⭐ TheRegular is the one to point at. The room wrote this method in
//  WEEK 3, over a Dictionary<string, int>, with two variables before the
//  loop and one comparison inside it. Same question, different list, one
//  line. That is the promise week 3 made.
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

    // ── week 9: two loops, gone ────────────────────────────────────────────

    // Every call the desk has taken tonight, added up.
    //
    // Task 1 — week 5's promise, collected. This is the third Sum in the
    // program and it is the same line as the other two with different nouns,
    // which is the point rather than a coincidence.
    public int TotalCalls => _callers.Sum(caller => caller.CallsTonight);

    // Who would not stop calling.
    //
    // Task 1 — week 3's promise, collected. The old version needed a leading
    // name, a highest count, and an if inside the loop. The two operators on
    // the end are doing the job that `string best = "nobody yet"` used to do:
    //
    //     ?.   there may be nobody at all, so don't ask a nothing for its Name
    //     ??   and when there is nobody, this is what to say instead
    //
    // MaxBy hands back the FIRST caller holding the highest count, which is
    // what the old `>` comparison did too — a tie goes to whoever rang first.
    public string TheRegular() => _callers.MaxBy(caller => caller.CallsTonight)?.Name ?? "nobody yet";
}
