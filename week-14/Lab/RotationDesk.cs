// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the keys that talk to a person about the carts.
//
//  FINISHED. This class talks to the person at the desk, and Rotation
//  does the thinking. Nothing in this file changes tonight.
// ═══════════════════════════════════════════════════════════════════

using Spectre.Console;
using System.Text.Json;

public static class RotationDesk
{
    private const string Violet = "#c792ea";
    private const string Coral = "#f78c6c";
    private const string Dim = "#5c6370";
    private const string Fg = "#d7dae0";

    public static void PullACart(Rotation rotation, DeskContext db)
    {
        while (true)
        {
            Console.Write("  Pull which cart? Title (Enter to give up): ");
            string? typed = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(typed))
            {
                return;
            }

            string title = typed.Trim();

            if (rotation.Remove(title))
            {
                rotation.Save(db);
                AnsiConsole.MarkupLine($"  [{Coral}]Pulled:[/] [{Violet}]{Markup.Escape(title)}[/] "
                    + $"[{Dim}]- {rotation.Count} {(rotation.Count == 1 ? "cart" : "carts")} left in the rotation.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"  [{Dim}]No cart called[/] [{Fg}]\"{Markup.Escape(title)}\"[/] "
                + $"[{Dim}]in the rotation.[/]");
        }
    }

    public static void TakeDelivery(Rotation rotation, DeskContext db)
    {
        while (true)
        {
            Console.Write("  Delivery file (Enter to give up): ");
            string? typed = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(typed))
            {
                return;
            }

            string path = typed.Trim();

            try
            {
                int added = rotation.Import(path);
                rotation.Save(db);

                AnsiConsole.MarkupLine($"  [{Coral}]Delivered:[/] [{Fg}]{added} new[/] "
                    + $"[{Dim}]- {rotation.Count} carts in the rotation.[/]");
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
