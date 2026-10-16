// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  YOURS, all night. Save and Load still use a file. In Task 4 they take
//  the desk's database instead, and in Task 5 Save learns to tell a new
//  cart from one that came out of the table. Everything below them is
//  week 9's, finished.
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

    // Week 9. FirstOrDefault hands back the first cart the question is true
    // for, or null when it is true of none — which is exactly what the loop
    // did when it fell out of the bottom. First would THROW there instead.
    public Song? Find(string title)
    {
        return _songs.FirstOrDefault(song => song.Title == title);
    }

    // TODO — Task 4: Save and Load take a DeskContext instead of a path.
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

        // Week 9. Not a query: "put all of these in" is a job, and the list
        // has a method for it. The Clear() above it stays — loading replaces.
        _songs.AddRange(loaded);
    }

    // Week 9. Where keeps the carts the question is true for, in the order
    // it found them. ToList turns that into a list to hand back.
    public List<Song> LongerThan(int seconds)
    {
        return _songs.Where(song => song.Seconds > seconds).ToList();
    }

    // Week 9. Select keeps every cart and hands back one thing about each.
    public List<string> Titles()
    {
        return _songs.Select(song => song.Title).ToList();
    }

    // Week 9. OrderBy builds a NEW, sorted list and leaves _songs alone.
    // _songs.Sort(...) would rearrange the rotation itself, and Save would
    // then write the file in the new order.
    public List<Song> ByTitle()
    {
        return _songs.OrderBy(song => song.Title).ToList();
    }

    // Week 9. Where again, with a string question: Contains asks whether the
    // term is anywhere inside the title. == would ask whether the title IS
    // the term, and that is Find's job.
    public List<Song> Matching(string term)
    {
        return _songs.Where(song => song.Title.Contains(term)).ToList();
    }
}
