// ═══════════════════════════════════════════════════════════════════
//  One call. Who rang, and what went out on air.
//
//  The day shift left you this and it ships finished — you don't have to
//  write or change a line of it. It's here because tonight the desk keeps
//  a list of calls, and a call is two facts that belong together.
//
//  You've written a class like this before. The only new part is what
//  happens to it next door in RequestLog.cs:
//
//      List<Call>
//
//  ...a list of YOUR type, not a list of strings. That's what the angle
//  brackets are for.
// ═══════════════════════════════════════════════════════════════════

public class Call
{
    public string Caller;
    public string OnAir;

    public Call(string caller, string onAir)
    {
        Caller = caller;
        OnAir = onAir;
    }
}
