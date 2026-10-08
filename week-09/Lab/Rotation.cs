// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  This is YOUR file tonight. Everything down to Load works, and Task 1
//  has you rewrite two pieces of it anyway: the loop in Find and the loop
//  at the bottom of Load. The three methods under Load are empty, and
//  they are Tasks 2, 3 and 4.
//
//  Every one of tonight's answers is one line, and none of them changes
//  the list it asks about.
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

    // TODO — Task 1. The cart with this title, or nothing at all. It works:
    // the desk's [f] key calls it. Write your fact about it FIRST, then make
    // the five lines of loop one line.
    public Song? Find(string title)
    {
        foreach (Song song in _songs)
        {
            if (song.Title == title)
            {
                return song;
            }
        }

        return null;
    }

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

        // TODO — Task 1. This loop puts every loaded cart into the rotation.
        // The list has one method that does the same job. The Clear() above
        // it stays where it is.
        foreach (Song song in loaded)
        {
            _songs.Add(song);
        }
    }

    // TODO — Task 2. Every cart longer than the number of seconds handed in,
    // in the rotation's own order.
    public List<Song> LongerThan(int seconds)
    {
        return new List<Song>();
    }

    // TODO — Task 3. Just the titles, one per cart, in the rotation's own
    // order.
    public List<string> Titles()
    {
        return new List<string>();
    }

    // TODO — Task 4. Every cart, in order by title. The rotation's own order
    // must be exactly what it was before anybody asked.
    public List<Song> ByTitle()
    {
        return new List<Song>();
    }
}
