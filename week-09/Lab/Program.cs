// KDXR 88.1 "The Owl" — the overnight shift, night eight.
//
// This file ships finished. Nothing in it is yours to write tonight: it
// already calls every method you are about to write, and every method your
// instructor is about to write. Where a method is still empty, the desk
// prints a dash.
//
// Two files hold the night, and both are written at sign-off:
//
//     week-09/switchboard.json   the callers
//     week-09/rotation.json      the carts
//
// Run it with:   dotnet run --project week-09/Lab
//
//     r  take a request        a  put the hour on air
//     h  redraw the hour       c  the switchboard
//     t  the carts, and what each one has played
//     f  find a cart by its title
//     n  the night's numbers
//     q  end the shift
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

// Where the night gets written down. Both paths are relative to where you
// RAN the program, which is the top of your repo — so both files land in
// this week's folder.
string switchboardFile = "week-09/switchboard.json";
string rotationFile = "week-09/rotation.json";

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

// Three carts loaded, same as every week. (A "cart" is one playable thing —
// one song, one jingle, one ad. Short for "cartridge": a case of looped tape
// that played once and rewound itself.)
var nightjar = new Song("Nightjar", "The Lamplighters", 227);
var slackWater = new Song("Slack Water", "Marguerite Vance", 252);
var longWayRound = new Song("Long Way Round", "The Ferrymen", 331);

var rotation = new Rotation();
rotation.Add(nightjar);
rotation.Add(slackWater);
rotation.Add(longWayRound);

// Last night's carts. If they are on disk, they replace the three above:
// same three songs, with whatever the station has already played on them.
rotation.Load(rotationFile);

// Take the carts back OUT of the rotation rather than using the three
// variables above, because after a Load those variables are last night's
// objects and the rotation is holding this night's. The hour and the
// rotation have to hold the same carts, or a play lands on the wrong one.
List<Song> carts = rotation.All();

// The switchboard, and the night up to now.
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

// Last night's switchboard, if it is on disk, replaces the three callers
// above.
switchboard.Load(switchboardFile);

// The hour, as it stands at four in the morning. Four different classes are
// in this one list. It holds them because each one keeps IScheduleItem's
// promise, and for no other reason at all.
var hour = new Hour();

hour.Add(new StationId("KDXR 88.1, The Owl"));
hour.Add(carts[0]);
hour.Add(new Ad("Pham's Bakery", "open at five", 3));
hour.Add(carts[1]);
hour.Add(new WeatherBed("clear, four below, wind out of the northwest"));
hour.Add(carts[2]);

DrawHour();

int next = 0;

while (true)
{
    Console.Write("[r]equest  [h]our  [a]ir  [c] switchboard  [t] carts  [f]ind  [n]umbers  [q]uit: ");
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

        case "h":
            DrawHour();
            break;

        case "a":
            AirTheHour();
            break;

        case "c":
            DrawSwitchboard();
            break;

        case "t":
            DrawCarts();
            break;

        case "f":
            FindACart();
            break;

        case "n":
            DrawNumbers();
            break;

        default:
            AnsiConsole.MarkupLine($"[{Dim}]The desk has eight buttons. That wasn't one.[/]");
            break;
    }

    AnsiConsole.WriteLine();
}

AnsiConsole.MarkupLine($"[{Fg}]{Broadcast.CallSign()} - that's the shift. "
    + $"{switchboard.Count} on the switchboard, {hour.Count} in the hour.[/]");

switchboard.Save(switchboardFile);

rotation.Save(rotationFile);

AnsiConsole.MarkupLine($"[{Dim}]Keep it quiet out there.[/]");

// ── the desk ───────────────────────────────────────────────────────────────

void TakeRequest()
{
    Console.Write("  Caller: ");
    string? typed = Console.ReadLine();
    string who = string.IsNullOrWhiteSpace(typed) ? "somebody who didn't say" : typed.Trim();

    List<Song> songs = rotation.All();

    for (int i = 0; i < songs.Count; i++)
    {
        AnsiConsole.MarkupLine($"    [{Dim}]{i + 1}.[/] "
            + $"[{Violet}]{Markup.Escape(songs[i].Title)}[/] "
            + $"[{Dim}]- {Markup.Escape(songs[i].Artist)}[/]");
    }

    Console.Write($"  Song (1-{songs.Count}, Enter for whatever's next): ");
    string? picked = Console.ReadLine();

    Song song;

    // A request line where the caller cannot name the song is not a request
    // line, so anything that is not a number in range gets whatever is next.
    if (int.TryParse((picked ?? "").Trim(), out int number)
        && number >= 1 && number <= songs.Count)
    {
        song = songs[number - 1];
    }
    else
    {
        song = songs[next % songs.Count];
        next++;
    }

    // One door: Take finds the regular or signs up the stranger, and
    // hands back whichever it was.
    Caller caller = switchboard.Take(who);
    caller.Asks(song);

    // And it goes in the hour, because a request nobody plays is a
    // conversation. Same object: the song on the board is the song on air.
    hour.Add(song);

    AnsiConsole.MarkupLine($"  [{Coral}]Line 1:[/] [{Fg}]{Markup.Escape(caller.Name)}[/] "
        + $"[{Dim}]asks for[/] [{Violet}]{Markup.Escape(song.Title)}[/]");
}

void AirTheHour()
{
    // One loop, written in Hour.cs, and it has never heard of a song.
    foreach (string line in hour.Run())
    {
        AnsiConsole.MarkupLine($"  [{Coral}]ON AIR[/]  [{Fg}]{Markup.Escape(line)}[/]");
    }
}

void DrawHour()
{
    AnsiConsole.Write(new Rule($"[{Violet}]04:00 - the hour[/]")
        .RuleStyle(Style.Parse(Dim))
        .LeftJustified());

    var sheet = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.FromHex(Dim))
        .AddColumn($"[{Dim}]KIND[/]")
        .AddColumn($"[{Dim}]CUE[/]")
        .AddColumn($"[{Dim}]LENGTH[/]");

    // One loop, four classes, the same three questions.
    foreach (IScheduleItem item in hour.All())
    {
        sheet.AddRow(
            $"[{Coral}]{Markup.Escape(item.Kind)}[/]",
            $"[{Fg}]{Markup.Escape(item.Cue)}[/]",
            $"[{Dim}]{Broadcast.Clock(item.Seconds)}[/]");
    }

    AnsiConsole.Write(sheet);
    AnsiConsole.MarkupLine($"[{Dim}]{hour.Count} items - "
        + $"{Broadcast.Clock(hour.TotalSeconds)} on the clock.[/]");
    AnsiConsole.WriteLine();
}

// The carts in rotation, and what each one has been out on air.
void DrawCarts()
{
    AnsiConsole.Write(new Rule($"[{Violet}]the carts[/]")
        .RuleStyle(Style.Parse(Dim))
        .LeftJustified());

    var sheet = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.FromHex(Dim))
        .AddColumn($"[{Dim}]TITLE[/]")
        .AddColumn($"[{Dim}]ARTIST[/]")
        .AddColumn($"[{Dim}]LENGTH[/]")
        .AddColumn($"[{Dim}]PLAYED[/]");

    foreach (Song song in rotation.All())
    {
        sheet.AddRow(
            $"[{Violet}]{Markup.Escape(song.Title)}[/]",
            $"[{Fg}]{Markup.Escape(song.Artist)}[/]",
            $"[{Dim}]{song.Length}[/]",
            $"[{Coral}]{song.PlaysTonight}[/]");
    }

    AnsiConsole.Write(sheet);
    AnsiConsole.MarkupLine($"[{Dim}]{rotation.Count} carts loaded.[/]");
    AnsiConsole.WriteLine();
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
        board.AddRow(
            $"[{Fg}]{Markup.Escape(caller.Name)}[/]",
            $"[{Coral}]{caller.CallsTonight}[/]",
            $"[{Dim}]{Markup.Escape(caller.Favorite?.Title ?? "-")}[/]");
    }

    AnsiConsole.Write(board);
    AnsiConsole.MarkupLine($"[{Dim}]{switchboard.Count} on the switchboard.[/]");
    AnsiConsole.WriteLine();
}

// One cart, asked for by title. Rotation.Find answers with the cart or with
// nothing at all, and the desk has a line for each.
void FindACart()
{
    Console.Write("  Title: ");
    string title = (Console.ReadLine() ?? "").Trim();

    Song? found = rotation.Find(title);

    if (found == null)
    {
        AnsiConsole.MarkupLine($"  [{Dim}]No cart called[/] [{Fg}]\"{Markup.Escape(title)}\"[/] "
            + $"[{Dim}]in the rotation.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"  [{Coral}]Found:[/] [{Violet}]{Markup.Escape(found.Title)}[/] "
            + $"[{Dim}]- {Markup.Escape(found.Artist)} ({found.Length})[/]");
    }
}

// The night's numbers. Every line asks one of tonight's methods one question.
// The top four and the running order are your instructor's; the middle three
// are yours. A line shows a dash until the method behind it is written.
void DrawNumbers()
{
    AnsiConsole.Write(new Rule($"[{Violet}]the night's numbers[/]")
        .RuleStyle(Style.Parse(Dim))
        .LeftJustified());

    AnsiConsole.MarkupLine($"[{Dim}]  the switchboard[/]      "
        + $"[{Fg}]{switchboard.TotalCalls} calls from {switchboard.Count} people[/]");
    AnsiConsole.MarkupLine($"[{Dim}]  rang more than once[/]  "
        + CallerNames(switchboard.CalledMoreThan(1), false));
    AnsiConsole.MarkupLine($"[{Dim}]  busiest two[/]          "
        + CallerNames(switchboard.Busiest(2), true));
    AnsiConsole.MarkupLine($"[{Dim}]  the regular[/]          "
        + Listed(new List<string> { switchboard.TheRegular() }));
    AnsiConsole.WriteLine();

    AnsiConsole.MarkupLine($"[{Dim}]  over four minutes[/]    "
        + CartTitles(rotation.LongerThan(240)));
    AnsiConsole.MarkupLine($"[{Dim}]  the titles[/]           "
        + Listed(rotation.Titles()));
    AnsiConsole.MarkupLine($"[{Dim}]  in order by title[/]    "
        + CartTitles(rotation.ByTitle()));
    AnsiConsole.WriteLine();

    AnsiConsole.MarkupLine($"[{Dim}]  the running order:[/]");

    foreach (string line in hour.RunningOrder())
    {
        AnsiConsole.MarkupLine($"    [{Fg}]{Markup.Escape(line)}[/]");
    }
}

// The three helpers below only turn a list into one line of text for the
// screen. They are loops on purpose, so nothing in this file gives away a
// line you are about to write.
string Listed(List<string> words)
{
    string line = "";

    foreach (string word in words)
    {
        if (word.Length == 0)
        {
            continue;
        }

        line += (line.Length == 0 ? "" : ", ") + Markup.Escape(word);
    }

    return line.Length == 0 ? $"[{Dim}]-[/]" : $"[{Fg}]{line}[/]";
}

string CartTitles(List<Song> songs)
{
    List<string> words = new List<string>();

    foreach (Song song in songs)
    {
        words.Add(song.Title);
    }

    return Listed(words);
}

string CallerNames(List<Caller> callers, bool withCalls)
{
    List<string> words = new List<string>();

    foreach (Caller caller in callers)
    {
        words.Add(withCalls ? $"{caller.Name} ({caller.CallsTonight})" : caller.Name);
    }

    return Listed(words);
}
