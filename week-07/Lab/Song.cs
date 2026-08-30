// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  Weeks 4 and 6, FINISHED, and the update did not touch it. Nothing in
//  here changes tonight — but Task 2's test leans on Length, so it is
//  worth thirty seconds to see where Length gets its clock from.
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
