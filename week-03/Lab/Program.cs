// KDXR 88.1 "The Owl" — the overnight shift, night two.
//
// You don't have to change anything in this file. You DO have to run it,
// after every single task, because this is where tonight actually happens:
// the board at the end of the shift is built out of the methods you write
// in RequestLog.cs, and right now it comes up empty.
//
// Run it with:   dotnet run --project week-03/Lab
// The shift lasts as long as you want. q ends it.
//
// ⚠️ One thing worth knowing, since it's in here and not in your file:
//    Spectre reads [square brackets] as formatting instructions, so anything
//    a CALLER typed goes through Markup.Escape first. Otherwise the night
//    somebody rings in asking for "[hold music]" takes the desk down — which
//    is last week's lesson wearing a new shirt.

using Spectre.Console;

const string Violet = "#c792ea";
const string Coral = "#f78c6c";
const string Dim = "#5c6370";
const string Fg = "#d7dae0";

// The station ident. The owl sits in consts on purpose: written inline, C# would
// read {o,o} as an interpolation hole and refuse to build (CS0103, twice).
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

while (true)
{
    AnsiConsole.MarkupLine($"[{Dim}]--- the phone lights up ---[/]");
    Console.Write("Who's calling? (Enter if they won't say - q ends the shift): ");
    string? caller = Console.ReadLine();

    // q is the DJ going home; null is the line going dead. Either ends the shift.
    if (caller == null || caller.Trim() == "q")
    {
        break;
    }

    Console.Write("  What do they want to hear? ");
    string? request = Console.ReadLine();

    string onAir = RequestLog.Log(caller, request);
    AnsiConsole.MarkupLine($"  [{Coral}]On air:[/] [{Fg}]{Markup.Escape(onAir)}[/]");

    int calls = RequestLog.TimesCalled(caller);
    if (calls > 1)
    {
        AnsiConsole.MarkupLine($"  [{Dim}]that's call number {calls} tonight from "
            + $"{Markup.Escape(CallerLine.CallerName(caller))}.[/]");
    }

    AnsiConsole.WriteLine();
}

// ── end of shift: the board ────────────────────────────────────────────────

AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule($"[{Violet}]the night so far[/]")
    .RuleStyle(Style.Parse(Dim))
    .LeftJustified());

var night = new Table()
    .Border(TableBorder.Rounded)
    .BorderColor(Color.FromHex(Dim))
    .AddColumn($"[{Dim}]#[/]")
    .AddColumn($"[{Dim}]CALLER[/]")
    .AddColumn($"[{Dim}]ON AIR[/]");

int number = 1;
foreach (Call call in RequestLog.Tonight)
{
    night.AddRow(
        $"[{Dim}]{number}[/]",
        $"[{Fg}]{Markup.Escape(call.Caller)}[/]",
        $"[{Coral}]{Markup.Escape(call.OnAir)}[/]");
    number++;
}
AnsiConsole.Write(night);

var regulars = new Table()
    .Border(TableBorder.Rounded)
    .BorderColor(Color.FromHex(Dim))
    .AddColumn($"[{Dim}]WHO[/]")
    .AddColumn($"[{Dim}]CALLS[/]");

foreach (KeyValuePair<string, int> entry in RequestLog.Regulars)
{
    regulars.AddRow($"[{Fg}]{Markup.Escape(entry.Key)}[/]", $"[{Violet}]{entry.Value}[/]");
}
AnsiConsole.Write(regulars);

AnsiConsole.MarkupLine($"[{Dim}]most calls tonight:[/] "
    + $"[{Violet}]{Markup.Escape(RequestLog.TheRegular())}[/]");
AnsiConsole.WriteLine();
AnsiConsole.MarkupLine($"[{Fg}]{Broadcast.CallSign()} - {Markup.Escape(RequestLog.SignOff())}[/]");
AnsiConsole.MarkupLine($"[{Dim}]Keep it quiet out there.[/]");
