// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  Weeks 4 and 6, FINISHED. Exactly one line of this file changes
//  tonight, and it is a line you ADD rather than one you repair.
// ═══════════════════════════════════════════════════════════════════

public class Song : IScheduleItem
{
    private string _title = "(untitled)";
    private string _artist = "(unknown)";

    public string Title
    {
        get { return _title; }
        set { if (!string.IsNullOrWhiteSpace(value)) { _title = value; } }
    }

    public string Artist
    {
        get { return _artist; }
        set { if (!string.IsNullOrWhiteSpace(value)) { _artist = value; } }
    }

    private int _seconds;

    public int Seconds
    {
        get { return _seconds; }
        set { if (value >= 1) { _seconds = value; } }
    }

    public string Length => Broadcast.Clock(Seconds);

    // TODO — Task 4: this number goes into the file and does not come back
    // out of it. One line above this one is what changes that. (check 4)
    public int PlaysTonight { get; private set; }

    public void Play()
    {
        PlaysTonight++;
    }

    public Song(string title, string artist, int seconds)
    {
        Title = title;
        Artist = artist;
        Seconds = seconds;
    }

    // Week 6's promise, kept in two lines.
    public string Kind => "SONG";

    public string Cue => $"{Title} - {Artist}";
}
