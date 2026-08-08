// KDXR 88.1 "The Owl" — the overnight shift, night three.
//
// You don't have to change anything in this file. You DO have to run it after
// every task, because this is where tonight actually happens: the board is
// built out of Song.cs and Rotation.cs, and right now both of them are
// holes with a class around them.
//
// Run it with:   dotnet run --project week-04/Lab
//
//     p  play the next one        a  add a song to the rotation
//     g  ???                      q  end the shift
//
// ⚠️ Spectre reads [square brackets] as formatting instructions, so anything
//    a human typed goes through Markup.Escape first. Same as last week.

using Spectre.Console;

const string Violet = "#c792ea";
const string Coral = "#f78c6c";
const string Dim = "#5c6370";
const string Fg = "#d7dae0";

// The owl sits in consts on purpose: written inline, C# would read {o,o} as an
// interpolation hole and refuse to build (CS0103, twice).
const string OwlTop = "{o,o}";
const string OwlMid = "|)__)";
const string OwlBot = "-\"-\"-";

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

// Three carts loaded before the shift starts. (A "cart" is one playable thing:
// one song, one jingle, one ad. Short for "cartridge" — a case of looped tape
// that played once and rewound itself. The tape is gone; the word stayed.)
var rotation = new Rotation();
rotation.Add(new Song("Nightjar", "The Lamplighters", 227));
rotation.Add(new Song("Slack Water", "Marguerite Vance", 252));
rotation.Add(new Song("Long Way Round", "The Ferrymen", 331));

DrawBoard();

int next = 0;

while (true)
{
    Console.Write("[p]lay  [a]dd  [g]  [q]uit: ");
    string? key = Console.ReadLine();

    // q is the DJ going home; null is the line going dead. Either ends it.
    if (key == null || key.Trim().ToLower() == "q")
    {
        break;
    }

    switch (key.Trim().ToLower())
    {
        case "p":
            PlayNext();
            break;

        case "a":
            AddOne();
            break;

        case "g":
            Gremlin();
            break;

        default:
            AnsiConsole.MarkupLine($"[{Dim}]The desk has four buttons. That wasn't one.[/]");
            break;
    }

    AnsiConsole.WriteLine();
    DrawBoard();
}

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine($"[{Fg}]{Broadcast.CallSign()} - that's the shift. "
    + $"{rotation.Count} in rotation.[/]");
AnsiConsole.MarkupLine($"[{Dim}]Keep it quiet out there.[/]");

// ── the desk ───────────────────────────────────────────────────────────────

void PlayNext()
{
    List<Song> songs = rotation.All();
    if (songs.Count == 0)
    {
        AnsiConsole.MarkupLine($"[{Dim}]Nothing in the rotation. Dead air is a bad look.[/]");
        return;
    }

    Song song = songs[next % songs.Count];
    song.Play();
    next++;

    AnsiConsole.MarkupLine($"  [{Coral}]On air:[/] [{Fg}]{Markup.Escape(song.Title)}[/] "
        + $"[{Dim}]-[/] [{Fg}]{Markup.Escape(song.Artist)}[/] [{Dim}]({song.Length})[/]");
}

void AddOne()
{
    Console.Write("  Title: ");
    string? title = Console.ReadLine();
    Console.Write("  Artist: ");
    string? artist = Console.ReadLine();
    Console.Write("  Length in seconds: ");
    string? secondsTyped = Console.ReadLine();

    // Week 2's habit, still earning its keep: ask once, answer gracefully.
    int.TryParse(secondsTyped, out int seconds);

    rotation.Add(new Song(title ?? "", artist ?? "", seconds));
    AnsiConsole.MarkupLine($"[{Dim}]Filed. Whether it kept what you typed is up to your "
        + "setters.[/]");
}

// 03:14. The automation has a moment, the way it does most nights. It reaches
// straight into the rotation and writes whatever it likes.
//
// In a class made of public fields, all three of these land. That is not a bug
// in this method — it is what a public field IS.
void Gremlin()
{
    AnsiConsole.MarkupLine($"[{Coral}]03:14 automation glitch[/]");

    List<Song> songs = rotation.All();
    if (songs.Count == 0)
    {
        AnsiConsole.MarkupLine($"[{Dim}]  ...nothing loaded to chew on. Lucky.[/]");
        return;
    }

    Song victim = songs[0];

    string titleBefore = victim.Title;
    victim.Title = "";
    Report("title", "\"\"", titleBefore, victim.Title);

    string secondsBefore = victim.Seconds.ToString();
    victim.Seconds = -400;
    Report("seconds", "-400", secondsBefore, victim.Seconds.ToString());

    // And the big one: ask for everything, then throw it on the floor.
    int countBefore = rotation.Count;
    rotation.All().Clear();
    Report("the whole rotation", "wiped", countBefore.ToString(), rotation.Count.ToString());
}

void Report(string what, string tried, string before, string after)
{
    bool held = before == after;
    string verdict = held
        ? $"[{Violet}]refused - still {Markup.Escape(before)}[/]"
        : $"[{Coral}]got in - now {Markup.Escape(after)}[/]";
    AnsiConsole.MarkupLine($"  [{Dim}]tried[/] [{Fg}]{Markup.Escape(what)}[/] "
        + $"[{Dim}]->[/] [{Fg}]{Markup.Escape(tried)}[/]   {verdict}");
}

void DrawBoard()
{
    AnsiConsole.Write(new Rule($"[{Violet}]in rotation[/]")
        .RuleStyle(Style.Parse(Dim))
        .LeftJustified());

    var board = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.FromHex(Dim))
        .AddColumn($"[{Dim}]#[/]")
        .AddColumn($"[{Dim}]TITLE[/]")
        .AddColumn($"[{Dim}]ARTIST[/]")
        .AddColumn($"[{Dim}]LENGTH[/]")
        .AddColumn($"[{Dim}]PLAYS[/]");

    int number = 1;
    foreach (Song song in rotation.All())
    {
        board.AddRow(
            $"[{Dim}]{number}[/]",
            $"[{Fg}]{Markup.Escape(song.Title)}[/]",
            $"[{Fg}]{Markup.Escape(song.Artist)}[/]",
            $"[{Violet}]{Markup.Escape(song.Length)}[/]",
            $"[{Coral}]{song.PlaysTonight}[/]");
        number++;
    }
    AnsiConsole.Write(board);
    AnsiConsole.MarkupLine($"[{Dim}]{rotation.Count} in rotation.[/]");
    AnsiConsole.WriteLine();
}
