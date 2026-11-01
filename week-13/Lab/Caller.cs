// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — somebody who rang the request line.
//
//  FINISHED. Nothing in this file changes tonight.
// ═══════════════════════════════════════════════════════════════════

public class Caller
{
    // The database's own number for this caller's row.
    public int Id { get; set; }
    // The name they gave the desk. Set once, when the call comes in.
    public string Name { get; private set; }

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
    }
}
