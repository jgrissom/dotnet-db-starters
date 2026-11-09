// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — who has rung the desk tonight.
//
//  FINISHED. Nothing in this file changes tonight.
// ═══════════════════════════════════════════════════════════════════

using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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

    public bool Remove(string name)
    {
        Caller? found = Find(name);

        if (found == null)
        {
            return false;
        }

        _callers.Remove(found);
        return true;
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

        foreach (Caller row in db.Callers.ToList())
        {
            if (!_callers.Any(caller => caller.Id == row.Id))
            {
                db.Callers.Remove(row);
            }
        }

        db.SaveChanges();
    }

    public void Load(DeskContext db)
    {
        List<Caller> loaded = db.Callers.Include(caller => caller.Requests).OrderBy(caller => caller.Id).ToList();

        if (loaded.Count == 0)
        {
            return;
        }

        _callers.Clear();

        _callers.AddRange(loaded);
    }

    // The names of the callers who have asked for this title, in order by
    // name. The database answers it: everything before ToList is sent as SQL.
    public List<string> AskedFor(DeskContext db, string title)
    {
        return db.Callers
            .Where(caller => caller.Requests.Any(request => request.Title == title))
            .OrderBy(caller => caller.Name)
            .Select(caller => caller.Name)
            .ToList();
    }

    // Reads the day list and adds the names, and says how many were new. A
    // name already on the switchboard is left out.
    public int Import(string path)
    {
        string json = File.ReadAllText(path);
        List<string> names = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

        // The whole list first: a blank name throws before anybody is added.
        foreach (string name in names)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidDataException("The list has a blank name on it.");
            }
        }

        int added = 0;

        foreach (string name in names)
        {
            if (Find(name) == null)
            {
                Add(new Caller(name));
                added++;
            }
        }

        return added;
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
