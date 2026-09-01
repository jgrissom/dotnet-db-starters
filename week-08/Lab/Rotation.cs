// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  Weeks 4 and 6, FINISHED down as far as All(). Two of tonight's tasks
//  are the two empty methods at the bottom.
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

    // ── the carts, written down ────────────────────────────────────────────
    // The path is handed IN. Where a file goes is a fact about the machine the
    // program is running on, and this class knows nothing about that machine —
    // which is also what lets a check hand in a scratch file instead of the
    // station's real rotation.

    // TODO — Task 2: turn the list into text and put the text in the file.
    // Nothing is written down yet, so nothing can come back. (check 2)
    public void Save(string path)
    {
    }

    // TODO — Task 3: the same trip, backwards. Ask whether the file is even
    // there first — a desk that has never been signed off has no file, and
    // that is a first night rather than a failure. (check 3)
    public void Load(string path)
    {
    }
}
