// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  FINISHED. Nothing in this file changes tonight.
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

    // Every time this cart has gone out on air. One cart, many airings.
    public List<Airing> Airings { get; private set; } = new List<Airing>();

    public void Play()
    {
        PlaysTonight++;
        Airings.Add(new Airing(DateTime.Now));
    }

    // When this cart last went out on air, for the carts screen's LAST ON
    // AIR column.
    public string LastAired()
    {
        Airing? newest = Airings.MaxBy(airing => airing.At);

        if (newest == null)
        {
            return "-";
        }

        return newest.At.ToString("HH:mm");
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
