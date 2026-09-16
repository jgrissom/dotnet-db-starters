// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  Task 2 happens in here. Nothing else in this file is yours tonight.
// ═══════════════════════════════════════════════════════════════════
using System.Text.Json.Serialization;

public class Song : IScheduleItem
{
    // TODO — Task 2. The database needs one column that is different on
    // every row, so it can tell one cart from another. Nothing at the
    // station has such a thing; this is the database's own name for the row.

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

    // TODO — Task 2. This is worked out from Seconds rather than kept, and
    // a table should not store it.
    public string Length => Broadcast.Clock(Seconds);

    // TODO — Task 2. Week 8 needed this attribute, because the JSON
    // serializer would write a private setter out and then refuse to read it
    // back. Find out whether EF Core needs telling too.
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
    //
    // TODO — Task 2. Same question as Length: is either of these a fact this
    // cart KEEPS, or something it works out every time it is asked?
    public string Kind => "SONG";

    public string Cue => $"{Title} - {Artist}";
}
