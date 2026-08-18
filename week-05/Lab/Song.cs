// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  WEEK 4, FINISHED. This is the file you closed hole by hole last week:
//  private fields with doors on them, a Length that is worked out rather
//  than stored, and a play count nothing outside this class can forge.
//
//  If you missed a week you are not behind — it ships complete, and
//  tonight's work is all in Caller.cs and Switchboard.cs.
//
//  Nothing in here needs touching. Check 1 goes red if it changes.
// ═══════════════════════════════════════════════════════════════════

public class Song
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

    public string Length => $"{Seconds / 60}:{Seconds % 60:00}";

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
}
