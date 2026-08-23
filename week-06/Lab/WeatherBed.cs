// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight forecast, read over a music bed.
//
//  SHIPS FINISHED, and it is the worked example. It is the whole shape,
//  done once: a class, `: IScheduleItem` after its name, and the four
//  members the promise asks for.
//
//  Read it before Task 2 and again before Task 4. Don't change it —
//  check 1 goes red if you do.
// ═══════════════════════════════════════════════════════════════════

public class WeatherBed : IScheduleItem
{
    public string Forecast { get; }

    // Nothing to count. It either went out tonight or it didn't.
    public bool Aired { get; private set; }

    public string Kind => "WEATHER";

    public string Cue => Aired ? $"{Forecast} (read)" : Forecast;

    public int Seconds => 45;

    public WeatherBed(string forecast)
    {
        Forecast = forecast;
    }

    public void Play()
    {
        Aired = true;
    }
}
