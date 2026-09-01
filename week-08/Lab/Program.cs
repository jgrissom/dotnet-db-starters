// KDXR 88.1 "The Owl" — the overnight shift, night seven.
//
// Nothing in here is yours to change tonight. This file already calls
// everything you are about to write — which is why running it is how you
// see each task land. Tonight's work happens in Rotation.cs, Song.cs,
// Broadcast.cs and Lab.Tests.
//
// Two files, and they are not the same kind of thing:
//
//     week-08/rotation.json  the carts, written fresh every sign-off
//     week-08/air-log.txt    one line per shift, added to and never rewritten
//
// Both paths are RELATIVE, so they are worked out from wherever you were
// standing when you ran the program. You run from the top of your repo, so
// they land in this week's folder — which is why the week is in the name.
//
// Run it with:   dotnet run --project week-08/Lab
//
//     r  take a request        a  put the hour on air
//     h  redraw the hour       c  the switchboard
//     t  the carts, and what each one has played
//     q  end the shift
//
// ⚠️ Spectre reads [square brackets] as formatting instructions, so
//    anything a human typed goes through Markup.Escape first.

using NetCoreAudio;
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

// Where the two files live. See the note at the top of this file: relative to
// where you RAN the program, which is the top of your repo.
const string RotationFile = "week-08/rotation.json";
const string AirLogFile = "week-08/air-log.txt";

AnsiConsole.Clear();

AnsiConsole.Write(new Panel(
        $"[{Coral}]{OwlTop}[/]  [{Violet} bold]KDXR 88.1 FM[/]\n"
      + $"[{Coral}]{OwlMid}[/]  [{Violet} bold]THE OWL[/]\n"
      + $"[{Coral}]{OwlBot}[/]  [{Dim}]overnight desk[/]")
    .Border(BoxBorder.Rounded)
    .BorderColor(Color.FromHex(Dim)));
AnsiConsole.WriteLine();

// Who had the desk before you. On a log nobody has ever signed off, this
// comes back empty — and the desk says so rather than inventing somebody.
string previous = Broadcast.LastShift(AirLogFile);

AnsiConsole.MarkupLine(previous.Length == 0
    ? $"[{Dim}]Nothing on the desk. First shift on this log.[/]"
    : $"[{Dim}]Last on this desk: {Markup.Escape(previous)}[/]");
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

// If last night's carts are on disk, they replace the three above — same
// three songs, with whatever the station has already played on them.
rotation.Load(RotationFile);

// Take the carts back OUT of the rotation rather than using the three
// variables above, because after a Load those variables are last night's
// objects and the rotation is holding this night's. The hour and the
// rotation have to hold the same carts, or a play lands on the wrong one.
List<Song> carts = rotation.All();

// The switchboard, and the night up to now. Yours, from week 5.
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

// One player for the whole shift, so a second `a` can stop the first ident
// instead of talking over it.
Player identPlayer = new Player();

int next = 0;

while (true)
{
    Console.Write("[r]equest  [h]our  [a]ir  [c] switchboard  [t] carts  [q]uit: ");
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

        default:
            AnsiConsole.MarkupLine($"[{Dim}]The desk has six buttons. That wasn't one.[/]");
            break;
    }

    AnsiConsole.WriteLine();
}

AnsiConsole.MarkupLine($"[{Fg}]{Broadcast.CallSign()} - that's the shift. "
    + $"{switchboard.Count} on the switchboard, {hour.Count} in the hour.[/]");

// Clocking out is when the desk writes the night down. Two files, two jobs:
// the carts are rewritten, and the air log gets one more line.
rotation.Save(RotationFile);
Broadcast.LogShift(AirLogFile,
    $"{djName} signed off - {hour.Count} in the hour, {switchboard.Count} on the switchboard.");

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

    // Week 2's guard, doing the job it was written for: a request line where
    // the caller cannot name the song is not a request line.
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
    // Is there an ident in this hour? The hour holds IScheduleItems, and an
    // IScheduleItem cannot tell you it is a StationId — so `is` asks.
    foreach (IScheduleItem item in hour.All())
    {
        if (item is StationId)
        {
            PlayIdent();
            break;
        }
    }

    // One loop, written in Hour.cs, and it has never heard of a song.
    foreach (string line in hour.Run())
    {
        AnsiConsole.MarkupLine($"  [{Coral}]ON AIR[/]  [{Fg}]{Markup.Escape(line)}[/]");
    }
}

// The station saying its own name out loud.
//
// ⚠️ The file has to be found from wherever you ran the program, and you run
// this one from the top of your repo — so the path is worked out from where
// the BUILT program sits, never written as "kdxr.wav" on its own.
void PlayIdent()
{
    try
    {
        string wav = Path.Combine(AppContext.BaseDirectory, "kdxr.wav");

        if (File.Exists(wav))
        {
            // Already talking? Cut it off rather than stack a second one on top.
            if (identPlayer.Playing)
            {
                identPlayer.Stop().Wait();
            }

            identPlayer.Play(wav).Wait();
        }
    }
    catch
    {
        // No sound on this machine, no problem. Nothing here is graded, and a
        // silent shift is still a shift.
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

// The carts in rotation, and what each one has been out on air. PLAYED is the
// number this week is really about: it is the one the desk keeps for itself.
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
