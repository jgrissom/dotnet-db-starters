// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  YOURS tonight. In Task 1 a cart gets a list of its airings — one for
//  every time it goes out on air — and Play() adds to it. Everything
//  else ships finished from weeks 4 to 11.
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

    // TODO — Task 1: a list of this cart's airings goes here, and Play()
    // adds one to it every time the cart goes out on air.

    public void Play()
    {
        PlaysTonight++;
    }

    // When this cart last went out on air, for the carts screen's LAST ON
    // AIR column.
    public string LastAired()
    {
        return "-";
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
