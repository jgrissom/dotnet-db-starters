// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  ANSWER KEY — week 8's Rotation with week 9's work in: two loops
//  collapsed (Task 1) and three questions the desk could not ask before
//  (Tasks 2, 3 and 4).
//
//  ⚠️ All() copies the LIST. It does not copy the songs in it — which is
//  why the hour and the rotation hold the SAME carts, and why a play
//  counted in one is counted in the other. Every query below hands back a
//  new list for the same reason.
// ═══════════════════════════════════════════════════════════════════

public class Rotation
{
    // The night's rotation. PRIVATE — nothing outside this class can touch
    // this list, which is why the members below are the only way in.
    private readonly List<Song> _songs = new List<Song>();

    public void Add(Song song)
    {
        _songs.Add(song);
    }

    // Ask the list. A second count is a second thing that can be wrong.
    public int Count => _songs.Count;

    // A COPY. Return _songs itself and the `private` above stops
    // meaning anything — whoever asked could empty the rotation, and Count
    // would agree with them.
    public List<Song> All()
    {
        return new List<Song>(_songs);
    }

    // How long the whole rotation runs.
    //
    // Task 1 — and this is week 4's promise, collected. It was a running
    // total, a foreach and a `+=`; it is the word Sum and the thing being
    // added up. Exactly the same answer, and the suite says so.
    public int TotalSeconds => _songs.Sum(song => song.Seconds);

    // ── week 10: the carts, in the database ────────────────────────────────
    // The context is handed in and never made in here, for exactly the reason
    // the path was handed in in week 8: WHERE the desk keeps its carts is a
    // fact about the machine and the network, and this class knows nothing
    // about either. It is also what lets a test hand in somewhere else.

    // TODO — Task 4. Write the whole rotation back to the table.
    //
    // ⚠️ Every cart is one of two things, and its Id is how you tell:
    //
    //   Id == 0   this cart has never been in the table. It is NEW.
    //   Id != 0   the table already has a row for it, so that row CHANGES —
    //             which is how tonight's plays land on last night's cart.
    //
    // And whichever it is, nothing happens at all until you tell the database
    // to do it. That is the line people forget.
    //
    // Until then the desk still runs — it just forgets everything at sign-off.
    public void Save(DeskContext db)
    {
    }

    // TODO — Task 5. Read the carts back.
    //
    // ⚠️ Two things this has to get right, and week 8 had both:
    //   • loading is REPLACING, not adding
    //   • the FIRST NIGHT — nobody has saved anything yet, and the rotation
    //     already holds the three carts Program.cs put in it
    public void Load(DeskContext db)
    {
    }

    // ── week 9: things the desk can finally ask ────────────────────────────
    // Three questions. Each one is a list, a question, and ToList() on the
    // end — because a caller wants an answer, not a recipe for working one out.

    // Task 2. Anything long enough to cover the 4 AM news feed.
    public List<Song> LongerThan(int seconds)
    {
        return _songs.Where(song => song.Seconds > seconds).ToList();
    }

    // Task 3. The carts that have not been out tonight — the ones a DJ at
    // 4 AM is actually looking for.
    public List<Song> NeverPlayed()
    {
        return _songs.Where(song => song.PlaysTonight == 0).ToList();
    }

    // Task 4. The carts that have had the most airings, hardest-worked first.
    //
    // OrderByDescending sorts a COPY and hands it back; the rotation itself is
    // in the order it was loaded in, before and after. Take stops at n, and
    // asking for more than there are is not an error — it just gives you all
    // of them.
    public List<Song> TopPlayed(int n)
    {
        return _songs.OrderByDescending(song => song.PlaysTonight).Take(n).ToList();
    }
}
