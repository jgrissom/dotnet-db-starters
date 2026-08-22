// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — somebody who rang the request line.
//
//  ⚠️ READ THIS ONE FIRST. It ships broken, and it is broken in exactly
//  the way the demo was broken tonight.
//
//  Every caller gets one of these. Dorothy gets one, Bex gets one,
//  Teodoro gets one. Three objects, three people — and, right now, ONE
//  number between them, because the field behind CallsTonight is
//  `static`.
//
//  `static` means the field belongs to the CLASS, not to any one caller:
//  one copy, made once, alive as long as the program runs. So when
//  Dorothy rings, Bex's count goes up too, and Teodoro's, and the board
//  cannot tell three people apart.
//
//  Run the shift and press `c` if you want to watch that happen before
//  you fix it. It is worth thirty seconds.
// ═══════════════════════════════════════════════════════════════════

public class Caller
{
    // The name they gave the desk. Set once, when the call comes in.
    public string Name { get; }

    // TODO — Task 2. The count that belongs to nobody.
    //
    // `_calls` is one number for the whole class, and the property in front
    // of it just reads that one number out, so every caller reports the
    // same total.
    //
    // Three members, three different jobs — only one of them actually goes:
    //
    //   _calls        DELETE it. An instance property brings its own field.
    //   CallsTonight  REWRITE it to hold its own number, not read a shared
    //                 one. Same name, still an int.
    //   Calls()       KEEP it — only the body changes, because the field it
    //                 adds to is about to stop existing.
    //
    // The shape is already in this folder: Song.cs ships finished and holds
    // PlaysTonight and Play(), which is exactly this with different names.
    // Read it; don't change it.
    private static int _calls;

    public int CallsTonight => _calls;

    public void Calls()
    {
        _calls++;
    }

    // TODO — Task 5. What they asked for.
    //
    // A caller who has just rung for the first time has not asked for
    // anything yet, and there is no sensible song to pretend they did.
    // That is what `Song?` is for: the question mark says "there might
    // not be one", and `null` is the honest answer until there is.
    //
    // Make this a property the outside world can read and cannot write —
    // same as CallsTonight — and let Asks() below be the only way in.
    public Song? Favourite => null;

    // TODO — Task 5. One request.
    //
    // Remember what they asked for. One line — and keep the song you were
    // handed rather than building a new one, because the song you were
    // handed is the actual cart in the rotation.
    //
    // ⚠️ Do NOT count the call in here. Switchboard.Take already did that
    // when it put them on the line; counting again here would put Dorothy
    // up by two every time she asks for something. One rule, one place.
    public void Asks(Song song)
    {
    }

    public Caller(string name)
    {
        Name = name;
    }
}
