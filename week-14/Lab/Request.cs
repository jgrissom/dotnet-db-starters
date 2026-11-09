// One request: a title, asked for by one caller, at one time.

public class Request
{
    // The database's own number for this request's row.
    public int Id { get; set; }

    // Which caller made it: the Id of that caller's row in Callers.
    public int CallerId { get; set; }

    public string Title { get; private set; }

    public DateTime At { get; private set; }

    public Request(string title, DateTime at)
    {
        Title = title;
        At = at;
    }
}
