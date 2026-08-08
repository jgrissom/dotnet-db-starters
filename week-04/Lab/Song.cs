// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — one track in the overnight rotation.
//
//  ⚠️ READ THIS ONE. It ships in EXACTLY the shape you wrote last week:
//  a class that is four public fields and a constructor. It compiles, it
//  runs, and the board draws fine.
//
//  It is also completely defenceless. Anything, anywhere in the program,
//  can reach in and write anything it likes into any of these — a blank
//  title, a negative duration, a play count that never happened. Nothing
//  in C# will stop it and nothing will warn you.
//
//  Run the shift and press `g` if you want to watch that happen.
//
//  Tonight you close the holes, one task at a time.
// ═══════════════════════════════════════════════════════════════════

public class Song
{
    // TODO — Task 3. Title and Artist.
    // Make these two fields PRIVATE (rename them _title and _artist — the
    // underscore is just the usual way of saying "this is the private one"),
    // and give each a public PROPERTY that the rest of the program uses
    // instead:
    //
    //     private string _title = "(untitled)";
    //
    //     public string Title
    //     {
    //         get { return _title; }
    //         set { ... }          // ← a blank title never gets stored
    //     }
    //
    // The setter is the whole point: it is a method that runs on the way IN.
    // A blank or spaces-only title must leave the old value alone — the desk
    // keeps "(untitled)" rather than an empty line on the board.
    public string Title;
    public string Artist;

    // TODO — Task 4. Seconds, and the length that reads off it.
    // Same treatment: private field, public property, and a setter that
    // refuses anything under 1. A song cannot be zero seconds long, and a
    // negative one would run the board's arithmetic backwards.
    public int Seconds;

    // TODO — Task 4.
    // The length, as a human reads it: 227 seconds is "3:47".
    // Nothing stores this. It is worked out from Seconds every time somebody
    // asks, which is why it has a `get` and no `set` at all:
    //
    //     public string Length => $"{Seconds / 60}:{Seconds % 60:00}";
    //
    // Whole ÷ whole is whole — week 1's lesson, finally being useful on
    // purpose. The :00 pads the seconds so 3:7 comes out as 3:07.
    public string Length => "0:00";

    // TODO — Task 5. PlaysTonight.
    // This one is different, and it is the point of the whole night:
    //
    //     public int PlaysTonight { get; private set; }
    //
    // `private set` means the outside world can READ it and cannot WRITE it.
    // The only way the number moves is Play(), below — so the count can never
    // say something that did not happen.
    public int PlaysTonight;

    // TODO — Task 5.
    // One play. Add one to PlaysTonight — and once PlaysTonight has a
    // `private set`, this method is the ONLY thing in the program that can.
    public void Play()
    {
    }

    public Song(string title, string artist, int seconds)
    {
        // Once Title, Artist and Seconds are properties, these three lines
        // stop being direct writes and start going through your setters —
        // so a bad value handed to the constructor gets refused too, and you
        // do not have to write the rule twice.
        Title = title;
        Artist = artist;
        Seconds = seconds;
    }
}
