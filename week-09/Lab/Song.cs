// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  FINISHED. Nothing in here changes tonight. Tonight's methods ask
//  questions ABOUT songs — how long, what title — and never change one.
// ═══════════════════════════════════════════════════════════════════
using System.Text.Json.Serialization;

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

    // Written to the file either way; read back only because of [JsonInclude].
    [JsonInclude]
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
