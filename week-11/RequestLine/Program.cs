// KDXR 88.1 "The Owl" — the request line.
//
// Your INSTRUCTOR'S program. You run it; you never edit it.
//
//     dotnet run --project week-11/RequestLine
//
// It reads YOUR connection string, the one the desk uses, and swaps in the
// name of your instructor's database. So you ring the desk as yourself,
// with your own login, and the database writes down who you are.
//
// Your instructor runs it three more ways:
//
//     dotnet run --project week-11/RequestLine -- --open    open the line to the room
//     dotnet run --project week-11/RequestLine -- --board   the board, on the projector
//     dotnet run --project week-11/RequestLine -- --close   close it again
//
// ⚠️ Spectre reads [square brackets] as formatting instructions, so
//    anything a human typed goes through Markup.Escape first.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

const string Violet = "#c792ea";
const string Coral = "#f78c6c";
const string Dim = "#5c6370";
const string Fg = "#d7dae0";

const string OwlTop = "{o,o}";
const string OwlMid = "|)__)";
const string OwlBot = "-\"-\"-";

string mode = args.Length > 0 ? args[0] : "";

IConfiguration config = new ConfigurationBuilder()
    .AddUserSecrets<LineContext>()
    .Build();

string? mine = config["ConnectionStrings:Desk"];

if (mode != "--board")
{
    Ident();
}

if (string.IsNullOrWhiteSpace(mine))
{
    AnsiConsole.MarkupLine($"[{Coral}]No connection string.[/] [{Fg}]This program uses the one you "
        + "set for the desk in the lab's Task 1. Set that, then ring again.[/]");
    return;
}

if (mode == "--open")
{
    Open(mine);
    return;
}

if (mode == "--close")
{
    Close(mine);
    return;
}

if (mode == "--board")
{
    Board(mine);
    return;
}

// ── a caller ───────────────────────────────────────────────────────────────

Console.Write("Your instructor's initials: ");
string initials = (Console.ReadLine() ?? "").Trim().ToUpper();

var builder = new SqlConnectionStringBuilder(mine)
{
    InitialCatalog = $"DB_KDXR_156101411_{initials}",
};

string line = builder.ConnectionString;

AnsiConsole.MarkupLine($"[{Dim}]Ringing {Markup.Escape(builder.InitialCatalog)} ...[/]");
AnsiConsole.WriteLine();

if (!Show(line, 8))
{
    return;
}

while (true)
{
    Console.Write("Your name, as the DJ should say it (Enter to refresh, q to hang up): ");
    string? name = Console.ReadLine();

    if (name == null || name.Trim().ToLower() == "q")
    {
        break;
    }

    if (name.Trim().Length == 0)
    {
        Show(line, 8);
        continue;
    }

    Console.Write("What would you like to hear? A song, or a dedication: ");
    string? asked = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(asked))
    {
        AnsiConsole.MarkupLine($"[{Dim}]Nothing asked for, so nothing sent.[/]");
        AnsiConsole.WriteLine();
        continue;
    }

    if (Ring(line, Cut(name.Trim(), 40), Cut(asked.Trim(), 80)))
    {
        Show(line, 8);
    }
}

AnsiConsole.MarkupLine($"[{Dim}]Thanks for calling KDXR.[/]");

// ── the pieces ─────────────────────────────────────────────────────────────

void Ident()
{
    AnsiConsole.Write(new Panel(
            $"[{Coral}]{OwlTop}[/]  [{Violet} bold]KDXR 88.1 FM[/]\n"
          + $"[{Coral}]{OwlMid}[/]  [{Violet} bold]THE OWL[/]\n"
          + $"[{Coral}]{OwlBot}[/]  [{Dim}]request line[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.FromHex(Dim)));
    AnsiConsole.WriteLine();
}

string Cut(string text, int most) => text.Length <= most ? text : text.Substring(0, most);

// One row in, then the database has it. A new context for every call, so
// every call sees whatever everybody else has sent since.
bool Ring(string connection, string caller, string asked)
{
    try
    {
        using LineContext db = new LineContext(connection);
        db.Requests.Add(new Request { Caller = caller, Asked = asked });
        db.SaveChanges();
        AnsiConsole.MarkupLine($"[{Coral}]On the line.[/] [{Dim}]The desk has your request.[/]");
        AnsiConsole.WriteLine();
        return true;
    }
    catch (Exception e)
    {
        Trouble(e);
        return false;
    }
}

// The last few calls, oldest at the top.
bool Show(string connection, int last)
{
    List<Request> rows;

    try
    {
        using LineContext db = new LineContext(connection);
        rows = db.Requests.OrderBy(request => request.Id).ToList();
    }
    catch (Exception e)
    {
        Trouble(e);
        return false;
    }

    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.FromHex(Dim))
        .AddColumn($"[{Dim}]RANG AT[/]")
        .AddColumn($"[{Dim}]CALLER[/]")
        .AddColumn($"[{Dim}]ASKED FOR[/]");

    foreach (Request request in rows.Skip(Math.Max(0, rows.Count - last)))
    {
        table.AddRow(
            $"[{Dim}]{request.RangAt.ToLocalTime():HH:mm}[/]",
            $"[{Fg}]{Markup.Escape(request.Caller)}[/]",
            $"[{Violet}]{Markup.Escape(request.Asked)}[/]");
    }

    AnsiConsole.Write(new Rule($"[{Violet}]line 1 - requests tonight[/]")
        .RuleStyle(Style.Parse(Dim))
        .LeftJustified());
    AnsiConsole.Write(table);
    AnsiConsole.MarkupLine($"[{Dim}]{rows.Count} {(rows.Count == 1 ? "request" : "requests")} on the line.[/]");
    AnsiConsole.WriteLine();
    return true;
}

// Say what went wrong in words a caller can act on.
void Trouble(Exception e)
{
    string message = e.InnerException?.Message ?? e.Message;

    for (Exception? x = e; x != null; x = x.InnerException)
    {
        if (x is SqlException)
        {
            message = x.Message;
        }
    }

    if (message.Contains("Cannot open database"))
    {
        AnsiConsole.MarkupLine($"[{Coral}]Nobody answered.[/] [{Fg}]Either the line isn't open, "
            + "or those aren't your instructor's initials.[/]");
    }
    else if (message.Contains("permission was denied"))
    {
        AnsiConsole.MarkupLine($"[{Coral}]The line is closed.[/] [{Fg}]Your instructor hasn't "
            + "opened it to the room, or has closed it for the night.[/]");
    }
    else if (message.Contains("Login failed"))
    {
        AnsiConsole.MarkupLine($"[{Coral}]Your login failed.[/] [{Fg}]This program uses the desk's "
            + "connection string, so the desk can't connect either. The lab's Task 1 sets it.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[{Coral}]The line dropped.[/] [{Fg}]{Markup.Escape(message.Split('\n')[0])}[/]");
    }

    AnsiConsole.WriteLine();
}

// ── your instructor's three ────────────────────────────────────────────────

// The board, on the projector. No prompt: it asks the table again every
// three seconds, until somebody presses Ctrl+C.
void Board(string connection)
{
    while (true)
    {
        AnsiConsole.Clear();
        Ident();
        Show(connection, 14);
        AnsiConsole.MarkupLine($"[{Dim}]Asks again every three seconds. Ctrl+C to stop.[/]");
        Thread.Sleep(3000);
    }
}

// One table, opened to every login on the server: anybody can read it and
// add a row, nobody can change or delete one, and the trigger writes the
// login of whoever rang onto every row. Safe to run twice.
void Open(string connection)
{
    string[] steps =
    {
        "IF OBJECT_ID('dbo.Requests') IS NULL "
            + "CREATE TABLE dbo.Requests ("
            + "Id int IDENTITY(1,1) PRIMARY KEY, "
            + "Caller nvarchar(40) NOT NULL, "
            + "Asked nvarchar(80) NOT NULL, "
            + "RangAt datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), "
            + "Login sysname NOT NULL DEFAULT ORIGINAL_LOGIN())",
        "IF OBJECT_ID('dbo.Requests_StampLogin') IS NULL "
            + "EXEC('CREATE TRIGGER dbo.Requests_StampLogin ON dbo.Requests AFTER INSERT AS "
            + "SET NOCOUNT ON; "
            + "UPDATE r SET Login = ORIGINAL_LOGIN() FROM dbo.Requests r JOIN inserted i ON i.Id = r.Id')",
        "IF DATABASE_PRINCIPAL_ID('request_line') IS NULL CREATE ROLE request_line",
        "GRANT SELECT, INSERT ON dbo.Requests TO request_line",
        "ALTER ROLE request_line ADD MEMBER guest",
        "GRANT CONNECT TO guest",
    };

    try
    {
        using LineContext db = new LineContext(connection);

        foreach (string step in steps)
        {
            db.Database.ExecuteSqlRaw(step);
        }

        string where = new SqlConnectionStringBuilder(connection).InitialCatalog;
        AnsiConsole.MarkupLine($"[{Coral}]The line is open[/] [{Fg}]on {Markup.Escape(where)}.[/] "
            + $"[{Dim}]Every login on this server can read the requests and add one.[/]");
    }
    catch (Exception e)
    {
        Trouble(e);
    }
}

void Close(string connection)
{
    try
    {
        using LineContext db = new LineContext(connection);
        db.Database.ExecuteSqlRaw("REVOKE CONNECT FROM guest");
        AnsiConsole.MarkupLine($"[{Coral}]The line is closed.[/] [{Dim}]The requests stay in the "
            + "table; nobody but you can reach them now.[/]");
    }
    catch (Exception e)
    {
        Trouble(e);
    }
}
