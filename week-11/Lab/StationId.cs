// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the legal ID.
//
//  FINISHED. Twelve seconds, and it counts its own airings. Nothing in
//  here changes tonight.
// ═══════════════════════════════════════════════════════════════════

public class StationId : IScheduleItem
{
    public string Words { get; }

    // Readable anywhere, moved by Play() and nothing else.
    public int TimesAired { get; private set; }

    public string Kind => "IDENT";

    public string Cue => Words;

    public int Seconds => 12;

    public StationId(string words)
    {
        Words = words;
    }

    public void Play()
    {
        TimesAired++;
    }
}
