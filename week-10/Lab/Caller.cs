// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — somebody who rang the request line.
//
//  Your week 5 work, FINISHED, untouched by the update. One count per
//  caller, a Favorite that only Asks() moves — nothing in here changes
//  tonight.
// ═══════════════════════════════════════════════════════════════════

public class Caller
{
    // The name they gave the desk. Set once, when the call comes in.
    public string Name { get; }

    // One per caller, because every caller is a separate object. Read it
    // anywhere; write it nowhere except inside this class.
    public int CallsTonight { get; private set; }

    // The only thing in the program that moves that number.
    public void Calls()
    {
        CallsTonight++;
    }

    // Song? — a caller who has just rung has not asked for
    // anything yet, and null is the honest answer rather than a made-up
    // song. Asks() is the only way it moves.
    public Song? Favorite { get; private set; }

    // Keep the song we were handed — the actual cart in the
    // rotation, not a copy of it.
    //
    // ⚠️ The call is NOT counted here. Switchboard.Take counted it when it
    // put them on the line, and counting again would put Dorothy up by two
    // every time she asked for something. Check 5 asserts that it doesn't.
    public void Asks(Song song)
    {
        Favorite = song;
    }

    public Caller(string name)
    {
        Name = name;
    }
}
