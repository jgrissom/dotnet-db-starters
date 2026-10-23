// One call to the request line: who rang, and what they asked for.

public class Request
{
    public int Id { get; set; }

    public string Caller { get; set; } = "";

    public string Asked { get; set; } = "";

    // Filled in by the database when the row goes in, in UTC.
    public DateTime RangAt { get; set; }
}
