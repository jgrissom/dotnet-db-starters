// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — what it takes to go in the hour.
//
//  This is not a class. There is nothing here to make — you will never
//  write `new IScheduleItem(...)`, because there is no such thing.
//
//  It is a PROMISE. Anything that can answer these four can be scheduled,
//  and the hour does not care what else it is.
// ═══════════════════════════════════════════════════════════════════

public interface IScheduleItem
{
    // One word for the log: SONG, IDENT, AD, WEATHER.
    string Kind { get; }

    // What the DJ reads off the screen when it comes up.
    string Cue { get; }

    // How long it runs. The hour has to add up.
    int Seconds { get; }

    // It just went out over the air. Every kind counts that differently,
    // and that difference is tonight.
    void Play();
}
