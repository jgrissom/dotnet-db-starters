// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  This file is your INSTRUCTOR'S, not a task. Its Save and Load move into
//  the database live in class, and you copy the finished file in at the
//  start of Tasks 4 and 5. Read it as you work: every change made here is
//  the one you are about to make in Rotation.cs.
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
        return _callers.FirstOrDefault(caller => caller.Name == name);
    }

    // One door for "a call came in". Take hands back THE caller on the
    // board, and only makes a new one when Find comes back empty. Both roads
    // end in the same place: the call is counted on whoever is handed back.
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

    public void Save(DeskContext db)
    {
        foreach (Caller caller in _callers)
        {
            if (caller.Id == 0)
            {
                db.Callers.Add(caller);
            }
            else
            {
                db.Callers.Update(caller);
            }
        }

        db.SaveChanges();
    }

    public void Load(DeskContext db)
    {
        List<Caller> loaded = db.Callers.OrderBy(caller => caller.Id).ToList();

        if (loaded.Count == 0)
        {
            return;
        }

        _callers.Clear();

        _callers.AddRange(loaded);
    }


    // ── the night's numbers ────────────────────────────────────────────────

    // Every call the desk has taken tonight, added up.
    public int TotalCalls
    {
        get { return _callers.Sum(caller => caller.CallsTonight); }
    }

    // The callers who have rung more than this many times.
    public List<Caller> CalledMoreThan(int calls)
    {
        return _callers.Where(caller => caller.CallsTonight > calls).ToList();
    }

    // The n callers who have rung most, busiest first.
    public List<Caller> Busiest(int n)
    {
        return _callers.OrderByDescending(caller => caller.CallsTonight).Take(n).ToList();
    }

    // The name of whoever has rung most.
    public string TheRegular()
    {
        return _callers.MaxBy(caller => caller.CallsTonight)?.Name ?? "nobody yet";
    }
}
