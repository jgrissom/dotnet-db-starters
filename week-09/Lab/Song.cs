// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  ANSWER KEY — weeks 4 and 6, with Task 4's one-line fix in.
//
//  A serializer WRITES anything with a public getter and READS BACK only
//  what it can set. PlaysTonight has a private setter, so the count went
//  into the file and never came out of it. [JsonInclude] is the sentence
//  "yes, this one too" — and it is a decision about what should survive,
//  not a repair.
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

    // Task 4. Written to the file either way; read back only because of this.
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

    // Week 6's promise, kept in two lines.
    public string Kind => "SONG";

    public string Cue => $"{Title} - {Artist}";
}
