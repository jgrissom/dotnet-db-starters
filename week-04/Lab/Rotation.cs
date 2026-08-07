// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what's in rotation tonight.
//
//  Last week's log was a `public static List<Call>` — one copy, shared by
//  the whole program, and anybody could reach in and empty it.
//
//  This one is an OBJECT. You make one with `new Rotation()`, it keeps its
//  own list, and that list is PRIVATE: the only ways in and out are the
//  three members you write below.
//
//  ⚠️ Worth knowing before you start: this is the same shape your homework
//  asks you to build for your own project tonight. Different name, same
//  idea — a class that holds a collection and guards the door.
// ═══════════════════════════════════════════════════════════════════

public class Rotation
{
    // The night's rotation. PRIVATE — nothing outside this class can touch
    // this list, which is why the three members below are the only way in.
    private readonly List<Song> _songs = new List<Song>();

    // TODO — Task 2.
    // Put a song in the rotation. One line: add it to _songs.
    public void Add(Song song)
    {
    }

    // TODO — Task 2.
    // How many songs are in the rotation. Ask _songs — never count along in
    // a separate variable, because a second count is a second thing that can
    // be wrong.
    public int Count => 0;

    // TODO — Task 2.
    // Everything in the rotation, for the board to draw.
    //
    // ⚠️ Hand back a COPY, not _songs itself:
    //
    //     return new List<Song>(_songs);
    //
    // Return the real list and you have quietly undone the `private` on it —
    // whoever asked can now empty your rotation, and your Count will agree
    // with them. Try it: return _songs, run the shift, press `g`.
    public List<Song> All()
    {
        return new List<Song>();
    }
}
