// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY. This half of the homework checks is plumbing: it finds YOUR
//  classes, YOUR methods and YOUR collections, and complains in plain
//  English when one of them isn't there yet.
//
//  Same trick as weeks 1 and 2, one week wider. It now looks up THREE
//  classes — Station and Switchboard (carried forward) and Playlist (new
//  tonight) — and, new this week, it can look at a public static FIELD,
//  because this week's subject is a collection rather than a return value.
//
//  It never calls or touches any of them directly; if it did, this project
//  wouldn't COMPILE until you'd written everything, and you'd get a screen
//  of compiler errors instead of a message telling you what to write next.
//
//  The actual checks are in HomeworkChecks.cs next door. That's the file
//  worth reading.
// ═══════════════════════════════════════════════════════════════════
using System.Reflection;

namespace Homework.Checks;

internal static class StudentCode
{
    // Your project's assembly, found by the name the homework fixed: Homework.
    private static Assembly Project => Assembly.Load("Homework");

    internal static Type RequireClass(string className, string hint)
    {
        var type = Project.GetTypes().FirstOrDefault(t => t.Name == className);

        Assert.True(type != null,
            $"I can't find a class called {className} in your Homework project.\n"
            + hint
            + $"Spelling and capitals matter for the CLASS name — {className}, exactly.");

        Assert.True(type!.IsPublic,
            $"Your {className} class exists but isn't public, so nothing outside your own "
            + "project can see it — including these checks.\n"
            + $"    public static class {className}\n"
            + "The word public is the whole fix.");

        return type;
    }

    // Finds a public static method by name and checks its shape before anybody
    // tries to call it, so a wrong signature is explained rather than crashed.
    internal static MethodInfo RequireMethod(string className, string hint,
        string name, Type returns, params Type[] takes)
    {
        var type = RequireClass(className, hint);
        var candidates = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                             .Where(m => m.Name == name)
                             .ToList();

        Assert.True(candidates.Count > 0,
            $"{className} has no public static method called {name}.\n"
            + $"    public static {Pretty(returns)} {name}({Signature(takes)})\n"
            + "Check the spelling and the capital letter at the start, and that it says "
            + "static.");

        var method = candidates.FirstOrDefault(m =>
            m.GetParameters().Select(p => p.ParameterType).SequenceEqual(takes));

        Assert.True(method != null,
            $"{className}.{name} exists, but not with the parameters the homework asks for.\n"
            + $"    yours:  {Pretty(candidates[0].ReturnType)} {name}("
            + $"{Signature(candidates[0].GetParameters().Select(p => p.ParameterType).ToArray())})\n"
            + $"    wanted: {Pretty(returns)} {name}({Signature(takes)})\n"
            + "The types in the brackets are part of the method's identity in C#. (A string? "
            + "parameter counts as string here — the ? is a promise about null, not a "
            + "different type.)");

        Assert.True(method!.ReturnType == returns,
            $"{className}.{name} gives back a {Pretty(method.ReturnType)}, and it should give "
            + $"back a {Pretty(returns)}.\n"
            + "The type written in front of the method name is what comes out of it.");

        return method;
    }

    // New this week. A collection isn't something a method hands back — it's
    // something the class HOLDS, so the checks have to look at the field itself.
    internal static T RequireCollection<T>(string className, string hint,
        string fieldName, string declaration) where T : class
    {
        var type = RequireClass(className, hint);
        var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);

        Assert.True(field != null,
            $"{className} has no public static field called {fieldName}.\n"
            + $"    {declaration}\n"
            + "This one is a field rather than a method because it's a thing your class "
            + "KEEPS, not a thing it works out and hands back. It has to be public and "
            + "static, and spelled exactly that way.");

        Assert.True(field!.FieldType == typeof(T),
            $"{className}.{fieldName} is a {Pretty(field.FieldType)}, and it needs to be a "
            + $"{Pretty(typeof(T))}.\n"
            + $"    {declaration}\n"
            + "The type in the angle brackets is what the collection is allowed to hold, "
            + "and it's part of what the collection IS.");

        var value = field.GetValue(null) as T;

        Assert.True(value != null,
            $"{className}.{fieldName} is null — declared, but never given a collection to be.\n"
            + $"    {declaration}\n"
            + "Both halves matter: the left says what it is, the `= new ...()` on the right "
            + "is what makes an actual empty collection for it to point at.");

        return value!;
    }

    // Empties a collection if it's there and is the right type, and says
    // nothing at all if it isn't.
    //
    // ⚠️ Deliberately silent. Every check has to start from an empty desk, but
    // a check about the LIST must not fail because the DICTIONARY is the wrong
    // type — that would cost a student four points for a mistake in a
    // different method. The check that actually needs a collection is the one
    // that demands it, with a message about that collection.
    internal static void ClearIfPresent<T>(string className, string fieldName) where T : class
    {
        var type = Project.GetTypes().FirstOrDefault(t => t.Name == className);
        var field = type?.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        if (field?.FieldType != typeof(T)) return;

        switch (field.GetValue(null))
        {
            case List<string> list: list.Clear(); break;
            case Dictionary<string, int> map: map.Clear(); break;
        }
    }

    // Calls it, and unwraps the exception so a crash inside a student's method
    // reads as that method crashing rather than as reflection noise.
    internal static object Call(MethodInfo method, params object?[] args)
    {
        try
        {
            return method.Invoke(null, args)!;
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"{method.DeclaringType?.Name}.{method.Name}(...) threw "
                + $"{e.InnerException.GetType().Name} instead of returning a value:\n"
                + $"    {e.InnerException.Message}\n"
                + "This week the usual cause is asking a Dictionary for a key it hasn't got "
                + "— that's a KeyNotFoundException, and TryGetValue is the one that asks "
                + "first. (Last week's causes are still around too: .Trim() on a null, "
                + "int.Parse on something that isn't a number.)");
        }
    }

    // Like Call, but hands the exception back instead of failing, so a check
    // can say specifically WHICH input crashed and why that matters.
    internal static Exception? CallExpectingSurvival(MethodInfo method, params object?[] args)
    {
        try
        {
            method.Invoke(null, args);
            return null;
        }
        catch (TargetInvocationException e)
        {
            return e.InnerException ?? e;
        }
    }

    internal const string StationHint =
        "Station is your week 1 class, carried forward again — copy Station.cs from your "
        + "week-02/Homework folder into week-03/Homework.\n";

    internal const string SwitchboardHint =
        "Switchboard is your week 2 class, carried forward — copy Switchboard.cs from your "
        + "week-02/Homework folder into week-03/Homework. Tonight's Playlist calls it, so "
        + "it has to make the trip.\n";

    internal const string PlaylistHint =
        "The homework asks for a file called Playlist.cs holding a class called Playlist — "
        + "your station's memory for the night:\n"
        + "    public static class Playlist\n"
        + "    {\n"
        + "        public static List<string> Tonight = new List<string>();\n"
        + "        public static Dictionary<string, int> Regulars = new Dictionary<string, int>();\n"
        + "        ...\n"
        + "    }\n";

    private static string Pretty(Type t) =>
        t == typeof(string) ? "string"
        : t == typeof(int) ? "int"
        : t == typeof(double) ? "double"
        : t == typeof(bool) ? "bool"
        : t == typeof(List<string>) ? "List<string>"
        : t == typeof(Dictionary<string, int>) ? "Dictionary<string, int>"
        : t.Name;

    private static string Signature(Type[] takes) =>
        string.Join(", ", takes.Select((t, i) => $"{Pretty(t)} arg{i + 1}"));
}
