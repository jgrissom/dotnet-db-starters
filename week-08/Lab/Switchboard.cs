// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  Weeks 5 and 7, FINISHED, down as far as Take. The two empty methods at
//  the bottom are your INSTRUCTOR'S, not a task: they get written live in
//  class tonight, and you copy the finished file in. Your own Save and Load
//  are in Rotation.cs.
// ═══════════════════════════════════════════════════════════════════
using System.Text.Json;

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

    // ── the switchboard, written down ──────────────────────────────────────
    // Your instructor's. Program.cs already calls both, so the moment the
    // finished file is copied in, the switchboard remembers the night too.
    public void Save(string path)
    {
    }

    public void Load(string path)
    {
    }
}
