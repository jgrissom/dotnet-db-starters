// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the legal ID.
//
//  Your week 6 work, FINISHED, untouched by the update. Twelve seconds,
//  counts its own airings, still has a voice — nothing in here changes
//  tonight.
// ═══════════════════════════════════════════════════════════════════

public class StationId : IScheduleItem
{
    public string Words { get; }

    // Readable anywhere, moved by Play() and nothing else. Last week's shape.
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
