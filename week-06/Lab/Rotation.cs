// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  WEEK 4, FINISHED. A class that owns a private list and hands out a
//  COPY of it, so nobody outside can empty the rotation.
//
//  ⚠️ Worth a second look tonight, because it is only half the story.
//  All() copies the LIST. It does not copy the songs in it — and the
//  Switchboard you write tonight has exactly the same property.
//
//  Nothing in here needs touching. Check 1 goes red if it changes.
// ═══════════════════════════════════════════════════════════════════

public class Rotation
{
    // The night's rotation. PRIVATE — nothing outside this class can touch
    // this list, which is why the three members below are the only way in.
    private readonly List<Song> _songs = new List<Song>();

    // Task 2.
    public void Add(Song song)
    {
        _songs.Add(song);
    }

    // Task 2. Ask the list. A second count is a second thing that can be wrong.
    public int Count => _songs.Count;

    // Task 2. A COPY. Return _songs itself and the `private` above stops
    // meaning anything — whoever asked could empty the rotation, and Count
    // would agree with them.
    public List<Song> All()
    {
        return new List<Song>(_songs);
    }
}
