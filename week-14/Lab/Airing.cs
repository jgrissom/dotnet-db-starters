// One time a cart went out on air.

public class Airing
{
    // The database's own number for this airing's row.
    public int Id { get; set; }

    // Which cart it was: the Id of that cart's row in Songs.
    public int SongId { get; set; }

    public DateTime At { get; private set; }

    public Airing(DateTime at)
    {
        At = at;
    }
}
