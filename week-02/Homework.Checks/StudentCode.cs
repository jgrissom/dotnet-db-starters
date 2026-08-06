// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY. This half of the homework checks is plumbing: it finds YOUR
//  classes and YOUR methods, and complains in plain English when one of
//  them isn't there yet.
//
//  Same trick as week 1, one week wider: it now looks up TWO classes —
//  Station (carried forward from week 1) and Switchboard (new tonight).
//  It never calls either one directly; if it did, this project wouldn't
//  COMPILE until you'd written every method, and you'd get a screen of
//  compiler errors instead of a message telling you what to write next.
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
                + "This week that usually means input was BELIEVED instead of asked — "
                + "int.Parse on something that isn't a number, or .Trim() on a null. "
                + "TryParse and IsNullOrWhiteSpace are the tools that never throw.");
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
        "Station is your week 1 class, carried forward — copy Station.cs from your "
        + "week-01/Homework folder into week-02/Homework. (Skipped week 1? The homework "
        + "has a minimum Station to type in; it's four lines.)\n";

    internal const string SwitchboardHint =
        "The homework asks for a file called Switchboard.cs holding a class called "
        + "Switchboard — your station's request line:\n"
        + "    public static class Switchboard\n"
        + "    {\n"
        + "        public static string Greeting() { ... }\n"
        + "        ...\n"
        + "    }\n";

    private static string Pretty(Type t) =>
        t == typeof(string) ? "string"
        : t == typeof(int) ? "int"
        : t == typeof(double) ? "double"
        : t == typeof(bool) ? "bool"
        : t.Name;

    private static string Signature(Type[] takes) =>
        string.Join(", ", takes.Select((t, i) => $"{Pretty(t)} arg{i + 1}"));
}
