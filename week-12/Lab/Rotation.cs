// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  YOURS, all night. In Task 2 Load brings each cart's airings back with
//  it, and in Task 3 Aired asks the database a question. Everything else
//  is weeks 9 to 11's, finished.
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

    // Week 9. FirstOrDefault hands back the first cart the question is true
    // for, or null when it is true of none — which is exactly what the loop
    // did when it fell out of the bottom. First would THROW there instead.
    public Song? Find(string title)
    {
        return _songs.FirstOrDefault(song => song.Title == title);
    }

    // Week 11. Built on Find, the way the switchboard's is: no cart by that
    // title is an answer, not a crash.
    public bool Remove(string title)
    {
        Song? found = Find(title);

        if (found == null)
        {
            return false;
        }

        _songs.Remove(found);
        return true;
    }

    // Week 10: a cart with Id 0 is new, so Add; any other Id has a row, so Update.
    // Week 11: then every row in the table that no cart in the list has the Id
    // of comes out. The table ends up holding exactly what the list holds.
    public void Save(DeskContext db)
    {
        foreach (Song song in _songs)
        {
            if (song.Id == 0)
            {
                db.Songs.Add(song);
            }
            else
            {
                db.Songs.Update(song);
            }
        }

        foreach (Song row in db.Songs.ToList())
        {
            if (!_songs.Any(song => song.Id == row.Id))
            {
                db.Songs.Remove(row);
            }
        }

        db.SaveChanges();
    }

    // Load returns on an empty table — the first night — before the Clear().
    // TODO — Task 2: the carts come back, and their airings don't.
    public void Load(DeskContext db)
    {
        List<Song> loaded = db.Songs.OrderBy(song => song.Id).ToList();

        if (loaded.Count == 0)
        {
            return;
        }

        _songs.Clear();

        _songs.AddRange(loaded);
    }

    // TODO — Task 3: the titles of the carts that have been on air, in order
    // by title, asked of the DATABASE rather than of _songs. The n key calls it.
    public List<string> Aired(DeskContext db)
    {
        return new List<string>();
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
