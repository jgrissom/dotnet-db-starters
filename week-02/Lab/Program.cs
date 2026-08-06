// KDXR 88.1 "The Owl" — the overnight shift.
//
// The software on this desk is older than the DJ, and the station likes it
// that way. You don't have to change anything in this file — but DO run it,
// because this desk can be crashed from the keyboard, and finding out how
// is Task 1.
//
// Run it with:   dotnet run --project week-02/Lab
// The shift lasts as long as you want. q ends it.

Console.WriteLine("+------------------------------------------+");
Console.WriteLine("|  KDXR 88.1 FM . THE OWL . overnight desk |");
Console.WriteLine("+------------------------------------------+");
Console.WriteLine();

Console.Write("DJ on duty: ");
string djName = Console.ReadLine() ?? "somebody";
Console.WriteLine(Broadcast.SignOn(djName));
Console.WriteLine();

int callsTaken = 0;

while (true)
{
    Console.WriteLine("--- the phone lights up ---");
    Console.Write("Who's calling? (Enter if they won't say · q ends the shift): ");
    string? caller = Console.ReadLine();

    // q is the DJ going home; null is the line going dead. Either ends the shift.
    if (caller == null || caller.Trim() == "q")
    {
        break;
    }

    if (caller.Trim().ToLower() == "ray")
    {
        Console.Write("  It's Ray. Where's he at? ");
        string? marker = Console.ReadLine();
        Console.WriteLine($"  Log: {CallerLine.WhereIsRay(marker)}");
    }
    else
    {
        Console.Write("  What do they want to hear? ");
        string? request = Console.ReadLine();
        Console.WriteLine($"  On air: {CallerLine.TakeRequest(caller, request)}");
    }

    callsTaken++;
    Console.WriteLine();
}

Console.WriteLine();
Console.WriteLine($"{Broadcast.CallSign()} - end of shift. Calls taken: {callsTaken}.");
Console.WriteLine("The log? There is no log. The night just happened.");
Console.WriteLine("Keep it quiet out there.");
