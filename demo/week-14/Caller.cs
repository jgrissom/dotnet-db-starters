// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — somebody who rang the request line.
//
//  Your INSTRUCTOR'S. Tonight your instructor's copy gains two columns,
//  live in class. Yours stays exactly as it ships: you read that copy on
//  GitHub, in the starters repo's demo/week-14 folder, and never copy it in.
// ═══════════════════════════════════════════════════════════════════

public class Caller
{
    // The database's own number for this caller's row.
    public int Id { get; set; }
    // The name they gave the desk. Set once, when the call comes in.
    public string Name { get; private set; }

    // Whether the desk takes this caller's calls. Every caller starts out
    // welcome, and nobody has ever been turned away.
    public bool Welcome { get; set; } = true;

    // When this caller first rang the desk. Nobody wrote it down for the
    // callers already in the table, so for them it is nothing at all.
    public DateTime? FirstCalled { get; private set; }

    // The same date, for the switchboard's FIRST CALLED column: a dash when
    // there is nothing to show.
    public string FirstCall()
    {
        if (FirstCalled == null)
        {
            return "-";
        }

        return FirstCalled.Value.ToString("yyyy-MM-dd");
    }


    // One per caller, because every caller is a separate object. Read it
    // anywhere; write it nowhere except inside this class.
    public int CallsTonight { get; private set; }

    // The only thing in the program that moves that number.
    public void Calls()
    {
        CallsTonight++;
    }

    // Every request this caller has made. One caller, many requests.
    public List<Request> Requests { get; private set; } = new List<Request>();

    // ⚠️ The call is NOT counted here. Switchboard.Take counted it when it
    // put them on the line.
    public void Asks(Song song)
    {
        Requests.Add(new Request(song.Title, DateTime.Now));
    }

    // The title of the newest request, for the switchboard's ASKED FOR column.
    public string LastAsked()
    {
        Request? newest = Requests.MaxBy(request => request.At);

        if (newest == null)
        {
            return "-";
        }

        return newest.Title;
    }

    public Caller(string name)
    {
        Name = name;
        FirstCalled = DateTime.Now;
    }
}
