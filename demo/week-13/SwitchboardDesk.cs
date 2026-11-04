// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the keys that talk to a person about the callers.
//
//  This file is your INSTRUCTOR'S, not a task. It changes live in class,
//  and you copy the finished file in at the start of each task. Read it
//  as you work: every change made here is the one you are about to make
//  in RotationDesk.cs.
// ═══════════════════════════════════════════════════════════════════

using Spectre.Console;

public static class SwitchboardDesk
{
    private const string Coral = "#f78c6c";
    private const string Dim = "#5c6370";
    private const string Fg = "#d7dae0";

    public static void TakeACallerOff(Switchboard switchboard, DeskContext db)
    {
        while (true)
        {
            Console.Write("  Take which caller off the board? Name (Enter to give up): ");
            string? typed = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(typed))
            {
                return;
            }

            string name = typed.Trim();

            if (switchboard.Remove(name))
            {
                switchboard.Save(db);
                AnsiConsole.MarkupLine($"  [{Coral}]Off the board:[/] [{Fg}]{Markup.Escape(name)}[/] "
                    + $"[{Dim}]- {switchboard.Count} left on the switchboard.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"  [{Dim}]Nobody called[/] [{Fg}]\"{Markup.Escape(name)}\"[/] "
                + $"[{Dim}]on the switchboard.[/]");
        }
    }

    public static void TakeDayList(Switchboard switchboard, DeskContext db)
    {
        Console.Write("  Day list file: ");
        string path = (Console.ReadLine() ?? "").Trim();

        int added = switchboard.Import(path);
        switchboard.Save(db);

        AnsiConsole.MarkupLine($"  [{Coral}]Day list:[/] [{Fg}]{added} new[/] "
            + $"[{Dim}]- {switchboard.Count} on the switchboard.[/]");
    }
}
