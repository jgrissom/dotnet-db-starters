// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — somebody who rang the request line.
//
//  Your INSTRUCTOR'S file, not a task. Tonight a caller gets a request
//  history of their own, live in class, and you copy the finished file in
//  at the start of Task 1. Read it as you work: every change made here is
//  the one you are about to make in Song.cs.
// ═══════════════════════════════════════════════════════════════════

using System.ComponentModel.DataAnnotations.Schema;

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

    // What they asked for last. [NotMapped]: a whole Song has no column to
    // go in, so it is never saved, and after a restart it is empty again.
    [NotMapped]
    public Song? Favorite { get; private set; }

    // ⚠️ The call is NOT counted here. Switchboard.Take counted it when it
    // put them on the line.
    public void Asks(Song song)
    {
        Favorite = song;
    }

    // The title for the switchboard's ASKED FOR column.
    public string LastAsked()
    {
        return Favorite?.Title ?? "-";
    }

    public Caller(string name)
    {
        Name = name;
    }
}
