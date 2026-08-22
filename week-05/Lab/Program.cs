// KDXR 88.1 "The Owl" — the overnight shift, night four.
//
// You don't have to change anything in this file. You DO have to run it
// after every task, because this is where tonight actually happens: the
// switchboard is drawn out of Caller.cs and Switchboard.cs, and right now
// one of them cannot tell three people apart and the other treats every
// regular as a stranger.
//
// Run it with:   dotnet run --project week-05/Lab
//
//     r  take a request        c  redraw the switchboard
//     p  play the next cart    q  end the shift
//
// ⚠️ Spectre reads [square brackets] as formatting instructions, so
//    anything a human typed goes through Markup.Escape first.

using Spectre.Console;

const string Violet = "#c792ea";
const string Coral = "#f78c6c";
const string Dim = "#5c6370";
const string Fg = "#d7dae0";

// The owl sits in consts on purpose: written inline, C# would read {o,o}
// as an interpolation hole and refuse to build (CS0103, twice).
const string OwlTop = "{o,o}";
const string OwlMid = "|)__)";
const string OwlBot = "-\"-\"-";

// New from this week: the desk clears down before it signs on, so the
// shift starts on a clean screen instead of halfway down a build log.
AnsiConsole.Clear();

AnsiConsole.Write(new Panel(
        $"[{Coral}]{OwlTop}[/]  [{Violet} bold]KDXR 88.1 FM[/]\n"
      + $"[{Coral}]{OwlMid}[/]  [{Violet} bold]THE OWL[/]\n"
      + $"[{Coral}]{OwlBot}[/]  [{Dim}]overnight desk[/]")
    .Border(BoxBorder.Rounded)
    .BorderColor(Color.FromHex(Dim)));
AnsiConsole.WriteLine();

Console.Write("DJ on duty: ");
string djName = Console.ReadLine() ?? "somebody";
AnsiConsole.MarkupLine($"[{Fg}]{Markup.Escape(Broadcast.SignOn(djName))}[/]");
AnsiConsole.WriteLine();

// Three carts loaded, same as last week. (A "cart" is one playable thing —
// one song, one jingle, one ad. Short for "cartridge": a case of looped tape
// that played once and rewound itself.) Rotation and Song both ship finished.
var rotation = new Rotation();
rotation.Add(new Song("Nightjar", "The Lamplighters", 227));
rotation.Add(new Song("Slack Water", "Marguerite Vance", 252));
rotation.Add(new Song("Long Way Round", "The Ferrymen", 331));

// The switchboard, and the night up to now. Three people, five calls
// between them: Dorothy three times the way she does, Bex once for the
// one artist she ever asks for, Teodoro once at 03:20 for Junie.
var switchboard = new Switchboard();

var dorothy = new Caller("Dorothy");
var bex = new Caller("Bex");
var teodoro = new Caller("Teodoro");

switchboard.Add(dorothy);
switchboard.Add(bex);
switchboard.Add(teodoro);

dorothy.Calls();
dorothy.Calls();
dorothy.Calls();
bex.Calls();
teodoro.Calls();

DrawSwitchboard();

int next = 0;

while (true)
{
    Console.Write("[r]equest  [p]lay  [c] switchboard  [q]uit: ");
    string? key = Console.ReadLine();

    // q is the DJ going home; null is the line going dead. Either ends it.
    if (key == null || key.Trim().ToLower() == "q")
    {
        break;
    }

    switch (key.Trim().ToLower())
    {
        case "r":
            TakeRequest();
            break;

        case "p":
            PlayNext();
            break;

        case "c":
            // Nothing to do — the redraw at the bottom of the loop is the
            // whole point of this key.
            break;

        default:
            AnsiConsole.MarkupLine($"[{Dim}]The desk has four buttons. That wasn't one.[/]");
            break;
    }

    AnsiConsole.WriteLine();
    DrawSwitchboard();
}

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine($"[{Fg}]{Broadcast.CallSign()} - that's the shift. "
    + $"{switchboard.Count} on the switchboard.[/]");
AnsiConsole.MarkupLine($"[{Dim}]Keep it quiet out there.[/]");

// ── the desk ───────────────────────────────────────────────────────────────

void TakeRequest()
{
    Console.Write("  Caller: ");
    string? typed = Console.ReadLine();
    string who = string.IsNullOrWhiteSpace(typed) ? "somebody who didn't say" : typed.Trim();

    List<Song> songs = rotation.All();
    Song song = songs[next % songs.Count];
    next++;

    // One door: Take finds the regular or signs up the stranger, and
    // hands back whichever it was.
    Caller caller = switchboard.Take(who);
    caller.Asks(song);

    AnsiConsole.MarkupLine($"  [{Coral}]Line 1:[/] [{Fg}]{Markup.Escape(caller.Name)}[/] "
        + $"[{Dim}]asks for[/] [{Violet}]{Markup.Escape(song.Title)}[/]");
}

void PlayNext()
{
    List<Song> songs = rotation.All();
    Song song = songs[next % songs.Count];
    song.Play();
    next++;

    AnsiConsole.MarkupLine($"  [{Coral}]On air:[/] [{Fg}]{Markup.Escape(song.Title)}[/] "
        + $"[{Dim}]-[/] [{Fg}]{Markup.Escape(song.Artist)}[/] [{Dim}]({song.Length})[/]");
}

void DrawSwitchboard()
{
    AnsiConsole.Write(new Rule($"[{Violet}]switchboard[/]")
        .RuleStyle(Style.Parse(Dim))
        .LeftJustified());

    var board = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.FromHex(Dim))
        .AddColumn($"[{Dim}]CALLER[/]")
        .AddColumn($"[{Dim}]CALLS[/]")
        .AddColumn($"[{Dim}]ASKED FOR[/]");

    foreach (Caller caller in switchboard.All())
    {
        // Favorite is a Song? — there may not be one. `?.` asks for the
        // title only if there is a song to ask, and `??` supplies a dash
        // when there isn't.
        board.AddRow(
            $"[{Fg}]{Markup.Escape(caller.Name)}[/]",
            $"[{Coral}]{caller.CallsTonight}[/]",
            $"[{Dim}]{Markup.Escape(caller.Favorite?.Title ?? "-")}[/]");
    }

    AnsiConsole.Write(board);
    AnsiConsole.MarkupLine($"[{Dim}]{switchboard.Count} on the switchboard.[/]");
    AnsiConsole.WriteLine();
}
