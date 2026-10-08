// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  This file is your INSTRUCTOR'S, not a task. It gets rewritten live in
//  class, a piece at a time, and you copy the finished file in at the
//  start of each task. Read it as you work: every change made here is the
//  one you are about to make in Rotation.cs.
//
//  The four methods at the bottom are empty until then, and the desk's
//  [n] screen shows a dash where each answer will go.
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

    public void Save(string path)
    {
        string json = JsonSerializer.Serialize(_callers,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(path, json);
    }

    public void Load(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        List<Caller>? loaded = JsonSerializer.Deserialize<List<Caller>>(File.ReadAllText(path));

        if (loaded == null)
        {
            return;
        }

        _callers.Clear();

        foreach (Caller caller in loaded)
        {
            _callers.Add(caller);
        }
    }

    // ── the night's numbers ────────────────────────────────────────────────
    // Your instructor writes each of these live in class.

    // Every call the desk has taken tonight, added up.
    public int TotalCalls
    {
        get { return 0; }
    }

    // The callers who have rung more than this many times.
    public List<Caller> CalledMoreThan(int calls)
    {
        return new List<Caller>();
    }

    // The n callers who have rung most, busiest first.
    public List<Caller> Busiest(int n)
    {
        return new List<Caller>();
    }

    // The name of whoever has rung most.
    public string TheRegular()
    {
        return "";
    }
}
