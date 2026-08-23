// ═══════════════════════════════════════════════════════════════════
//  ANSWER KEY — KDXR 88.1 "The Owl", somebody who rang the request line.
//
//  Built rung by rung and checked at every one: Task 2 → 2/5, Task 3 →
//  3/5, Task 4 → 4/5, Task 5 → 5/5.
//
//  The starter ships this class with `private static int _calls;` behind
//  CallsTonight — one number for the whole program, so Dorothy, Bex and
//  Teodoro all report the same total. Task 2 deletes the field outright.
//  There is nothing to "make private": an instance auto-property already
//  has a field of its own, one per object, which is the entire lesson.
// ═══════════════════════════════════════════════════════════════════

public class Caller
{
    // The name they gave the desk. Set once, when the call comes in.
    public string Name { get; }

    // Task 2. One per caller, because every caller is a separate object.
    // Read it anywhere; write it nowhere except inside this class.
    public int CallsTonight { get; private set; }

    // Task 2. The only thing in the program that moves that number.
    public void Calls()
    {
        CallsTonight++;
    }

    // Task 5. Song? — a caller who has just rung has not asked for
    // anything yet, and null is the honest answer rather than a made-up
    // song. Asks() is the only way it moves.
    public Song? Favorite { get; private set; }

    // Task 5. Keep the song we were handed — the actual cart in the
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
