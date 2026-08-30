// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the legal ID.
//
//  Task 3. At the top of every hour this station has to say its own call
//  letters out loud. That takes twelve seconds and it is not a song.
//
//  Right now this class keeps no promise at all, which is why the hour
//  will not hold it — the line that puts it there is commented out in
//  Program.cs, and it would not compile if it weren't.
// ═══════════════════════════════════════════════════════════════════

public class StationId
{
    public string Words { get; }

    public StationId(string words)
    {
        Words = words;
    }

    // TODO — Task 3. Two things to do, in this order:
    //
    //   1. Promise. Put `: IScheduleItem` after the class name above.
    //      The build will then tell you exactly what you owe (CS0535,
    //      once per missing member) — treat it as a checklist.
    //   2. Keep it. Four members, and one more of your own: something
    //      that counts how many times the ident has aired tonight,
    //      readable anywhere and moved by Play() alone. You have written
    //      that shape twice already.
    //
    //  Then take the `//` off the StationId line in Program.cs.
}
