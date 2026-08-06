// KDXR 88.1 "The Owl" — the sign-on sequence.
//
// This is the part a DJ sees at the start of every shift. You don't have to
// change anything in this file tonight — but read it, because every line in
// here is something you'll be writing yourself by next week.
//
// Run it with:   dotnet run --project week-01/Lab
// Right now most of it lies, because Broadcast.cs isn't finished. That's the lab.

Console.WriteLine("+------------------------------------------+");
Console.WriteLine("|  KDXR 88.1 FM . THE OWL . overnight desk |");
Console.WriteLine("+------------------------------------------+");
Console.WriteLine();

Console.Write("DJ on duty: ");
// Console.ReadLine() can hand back nothing at all — if the input has ended
// there is no line to read. The `?? "somebody"` means "or use this instead".
// Week 5 explains why C# is so careful about "nothing at all".
string djName = Console.ReadLine() ?? "somebody";

Console.WriteLine();
Console.WriteLine(Broadcast.SignOn(djName));
Console.WriteLine();

// The station clock. Hard-coded tonight — the real one arrives in week 3.
int hour = 2;
int minute = 15;

Console.WriteLine($"Local time is {hour}:{minute} AM.");
Console.WriteLine($"Overnight block: {Broadcast.IsOvernight(hour)}");
Console.WriteLine($"{Broadcast.MinutesUntilSunrise(hour, minute)} minutes until sunrise.");
Console.WriteLine($"{Broadcast.HoursOnAir(330)} hours on air so far tonight.");
Console.WriteLine();
Console.WriteLine("Keep it quiet out there.");
