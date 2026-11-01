// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the keys that talk to a person about the carts.
//
//  Yours. Tasks 1, 2 and 3 happen here. This class talks to the person
//  at the desk. Rotation does the thinking and never prints a word.
// ═══════════════════════════════════════════════════════════════════

using Spectre.Console;

public static class RotationDesk
{
    private const string Violet = "#c792ea";
    private const string Coral = "#f78c6c";
    private const string Dim = "#5c6370";
    private const string Fg = "#d7dae0";

    // TODO — Task 1: a title nobody has should ask again, and Enter gives up.
    public static void PullACart(Rotation rotation, DeskContext db)
    {
        Console.Write("  Pull which cart? Title: ");
        string title = (Console.ReadLine() ?? "").Trim();

        if (rotation.Remove(title))
        {
            rotation.Save(db);
            AnsiConsole.MarkupLine($"  [{Coral}]Pulled:[/] [{Violet}]{Markup.Escape(title)}[/] "
                + $"[{Dim}]- {rotation.Count} {(rotation.Count == 1 ? "cart" : "carts")} left in the rotation.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"  [{Dim}]No cart called[/] [{Fg}]\"{Markup.Escape(title)}\"[/] "
                + $"[{Dim}]in the rotation.[/]");
        }
    }

    // TODO — Task 2: a file that isn't there, or a damaged one, crashes the desk.
    public static void TakeDelivery(Rotation rotation, DeskContext db)
    {
        Console.Write("  Delivery file: ");
        string path = (Console.ReadLine() ?? "").Trim();

        int added = rotation.Import(path);
        rotation.Save(db);

        AnsiConsole.MarkupLine($"  [{Coral}]Delivered:[/] [{Fg}]{added} new[/] "
            + $"[{Dim}]- {rotation.Count} carts in the rotation.[/]");
    }
}
