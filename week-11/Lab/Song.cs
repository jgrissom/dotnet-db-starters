// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  Ships finished, and nothing in it changes this week. The Id is the
//  database's own number for this cart's row, and EF Core sets a
//  private set without being told.
// ═══════════════════════════════════════════════════════════════════

public class Song : IScheduleItem
{
    // The database's own number for this cart's row: 0 until it is
    // saved, and SaveChanges fills in the real one.
    public int Id { get; set; }

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

    // What IScheduleItem asks of everything in the hour.
    public string Kind => "SONG";

    public string Cue => $"{Title} - {Artist}";
}
