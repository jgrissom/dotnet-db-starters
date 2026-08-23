// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  WEEK 4, FINISHED — with one thing added at the bottom that isn't.
//
//  You were told for two weeks not to touch this file. Tonight you add
//  two lines to it and change nothing else about it, which is the whole
//  point of Task 2: a class you finished a fortnight ago can keep a
//  promise it had never heard of.
//
//  Look at what it ALREADY answers. IScheduleItem asks for four things
//  and two of them have been sitting here since week 4.
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

    // TODO — Task 2. Two members, and they are the entire cost of keeping
    // the promise. These two compile and they answer nothing.
    //
    //   Kind is one word for the KIND column: this is a SONG.
    //   Cue is what the DJ reads off the screen — the title and who it is by.
    //
    // WeatherBed.cs ships finished and does all four. Read it.
    public string Kind => "?";

    public string Cue => "(nothing to cue)";
}
