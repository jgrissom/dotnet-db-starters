// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — somebody who rang the request line.
//
//  Your week 5 work, FINISHED. One count per caller, a Favorite that only
//  Asks() moves. One line changes tonight, and your instructor makes it
//  live in class; you copy the finished file in.
// ═══════════════════════════════════════════════════════════════════

using System.Text.Json.Serialization;

public class Caller
{
    // The name they gave the desk. Set once, when the call comes in.
    public string Name { get; }

    // One per caller, because every caller is a separate object. Read it
    // anywhere; write it nowhere except inside this class.
    [JsonInclude]
    public int CallsTonight { get; private set; }


    // The only thing in the program that moves that number.
    public void Calls()
    {
        CallsTonight++;
    }

    // Song? — a caller who has just rung has not asked for
    // anything yet, and null is the honest answer rather than a made-up
    // song. Asks() is the only way it moves.
    [JsonInclude]
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
