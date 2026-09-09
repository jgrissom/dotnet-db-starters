// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  Two loops in here come out tonight, and three empty methods get filled
//  in. The loops WORK. That is the point: you are not fixing anything.
//
//  ⚠️ All() copies the LIST. It does not copy the songs in it — which is
//  why the hour and the rotation hold the SAME carts, and why a play
//  counted in one is counted in the other.
// ═══════════════════════════════════════════════════════════════════
using System.Text.Json;

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

    // TODO — Task 1. How long the whole rotation runs.
    //
    // Week 4's ⭐ Done early? item, and it was promised to you then: "in week 9
    // that entire loop becomes one line." Here is the loop, working, whether
    // you wrote it that night or not.
    public int TotalSeconds
    {
        get
        {
            int total = 0;

            foreach (Song song in _songs)
            {
                total += song.Seconds;
            }

            return total;
        }
    }

    // ── week 8: the carts, written down ────────────────────────────────────
    // The path is handed in and never written down in here. Where a file goes
    // is a fact about the machine the program is running on, and this class
    // knows nothing about that machine.

    public void Save(string path)
    {
        string json = JsonSerializer.Serialize(_songs,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(path, json);
    }

    public void Load(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        List<Song>? loaded = JsonSerializer.Deserialize<List<Song>>(File.ReadAllText(path));

        if (loaded == null)
        {
            return;
        }

        _songs.Clear();

        // TODO — Task 1. Week 8's ⭐ Done early? item: "your Load walks a list
        // to fill another list… in week 9 that becomes one line."
        //
        // ⚠️ The Clear() above stays. Loading is REPLACING, and without it the
        // rotation ends the night with six carts in it.
        foreach (Song song in loaded)
        {
            _songs.Add(song);
        }
    }

    // ── week 9: three questions the desk cannot ask yet ────────────────────
    // All three ship handing back an empty list, which is why the night's
    // numbers screen has blanks on it. Each one is one line.

    // TODO — Task 2. Every cart longer than the number of seconds handed in.
    // The DJ needs something long enough to cover the 4 AM news feed.
    public List<Song> LongerThan(int seconds)
    {
        return new List<Song>();
    }

    // TODO — Task 3. The carts that have not been out at all tonight.
    public List<Song> NeverPlayed()
    {
        return new List<Song>();
    }

    // TODO — Task 4. The n carts with the most airings, hardest-worked first.
    //
    // ⚠️ Whatever you do in here, the rotation itself has to come out in the
    // order it was loaded in. Something else in this program relies on that.
    public List<Song> TopPlayed(int n)
    {
        return new List<Song>();
    }
}
