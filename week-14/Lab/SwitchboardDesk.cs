// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the keys that talk to a person about the callers.
//
//  FINISHED. Nothing in this file changes tonight.
// ═══════════════════════════════════════════════════════════════════

using Spectre.Console;
using System.Text.Json;

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
        while (true)
        {
            Console.Write("  Day list file (Enter to give up): ");
            string? typed = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(typed))
            {
                return;
            }

            string path = typed.Trim();

            try
            {
                int added = switchboard.Import(path);
                switchboard.Save(db);

                AnsiConsole.MarkupLine($"  [{Coral}]Day list:[/] [{Fg}]{added} new[/] "
                    + $"[{Dim}]- {switchboard.Count} on the switchboard.[/]");
                return;
            }
            catch (IOException)
            {
                AnsiConsole.MarkupLine($"  [{Dim}]No file at[/] [{Fg}]\"{Markup.Escape(path)}\"[/][{Dim}].[/]");
            }
            catch (JsonException)
            {
                AnsiConsole.MarkupLine($"  [{Coral}]Refused:[/] [{Fg}]that file is damaged.[/] [{Dim}]Nothing was added.[/]");
                return;
            }
            catch (InvalidDataException problem)
            {
                AnsiConsole.MarkupLine($"  [{Coral}]Refused:[/] [{Fg}]{Markup.Escape(problem.Message)}[/] [{Dim}]Nothing was added.[/]");
                return;
            }
        }
    }
}
