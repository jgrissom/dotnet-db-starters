// ═══════════════════════════════════════════════════════════════════
//  KDXR 88.1 "The Owl" — the overnight forecast, read over a music bed.
//
//  FINISHED since week 6, untouched by the update. Nothing in here
//  changes tonight.
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
