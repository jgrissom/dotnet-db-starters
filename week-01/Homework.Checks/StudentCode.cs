// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY. This half of the homework checks is plumbing: it finds YOUR
//  Station class and YOUR methods, and complains in plain English when one
//  of them isn't there yet.
//
//  Why it's written this way, since it looks odd: it never mentions
//  Station.CallSign() directly. If it did, this project wouldn't COMPILE
//  until you'd written every method — and you'd get a screen of compiler
//  errors instead of a message telling you what to write next. Looking the
//  methods up by name means the checks always run, and always explain.
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

    internal static Type RequireStation()
    {
        var type = Project.GetTypes().FirstOrDefault(t => t.Name == "Station");

        Assert.True(type != null,
            "I can't find a class called Station in your Homework project.\n"
            + "The homework asks for a file called Station.cs holding a class called Station, "
            + "with your station's details in it:\n"
            + "    public static class Station\n"
            + "    {\n"
            + "        public static string CallSign() { return \"KRVN\"; }   // <- yours, not mine\n"
            + "        ...\n"
            + "    }\n"
            + "Spelling and capitals matter for the CLASS name — Station, not station or "
            + "MyStation. What's inside it is yours to invent.");

        Assert.True(type!.IsPublic,
            "Your Station class exists but isn't public, so nothing outside your own project "
            + "can see it — including these checks.\n"
            + "    public static class Station\n"
            + "The word public is the whole fix. (Leave it off and C# assumes the most private "
            + "option available, which is the opposite of what you want here.)");

        return type;
    }

    // Finds a public static method by name and checks its shape before anybody
    // tries to call it, so a wrong signature is explained rather than crashed.
    internal static MethodInfo RequireMethod(string name, Type returns, params Type[] takes)
    {
        var station = RequireStation();
        var candidates = station.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                .Where(m => m.Name == name)
                                .ToList();

        Assert.True(candidates.Count > 0,
            $"Station has no public static method called {name}.\n"
            + $"    public static {Pretty(returns)} {name}({Signature(takes)})\n"
            + "Check the spelling and the capital letter at the start. If you wrote it without "
            + "the word static, add that too — static means \"you can call this without making "
            + "a Station object first\", and objects aren't until week 4.");

        var method = candidates.FirstOrDefault(m =>
            m.GetParameters().Select(p => p.ParameterType).SequenceEqual(takes));

        Assert.True(method != null,
            $"Station.{name} exists, but not with the parameters the homework asks for.\n"
            + $"    yours:  {Pretty(candidates[0].ReturnType)} {name}("
            + $"{Signature(candidates[0].GetParameters().Select(p => p.ParameterType).ToArray())})\n"
            + $"    wanted: {Pretty(returns)} {name}({Signature(takes)})\n"
            + "The types in the brackets are part of the method's identity in C# — the same "
            + "name with different parameters is a different method.");

        Assert.True(method!.ReturnType == returns,
            $"Station.{name} gives back a {Pretty(method.ReturnType)}, and it should give back a "
            + $"{Pretty(returns)}.\n"
            + "The type written in front of the method name is what comes out of it. If yours "
            + $"says {Pretty(method.ReturnType)}, change it to {Pretty(returns)} and make sure "
            + "every return inside it matches.");

        return method;
    }

    // Calls it, and unwraps the exception so a crash inside a student's method
    // reads as that method crashing rather than as reflection noise.
    internal static object Call(MethodInfo method, params object[] args)
    {
        try
        {
            return method.Invoke(null, args)!;
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Station.{method.Name}(...) threw {e.InnerException.GetType().Name} instead of "
                + $"returning a value:\n    {e.InnerException.Message}\n"
                + "Run your program with `dotnet run --project week-01/Homework` and you should see the "
                + "same crash. Fix it there first, then come back to the checks.");
        }
    }

    private static string Pretty(Type t) =>
        t == typeof(string) ? "string"
        : t == typeof(int) ? "int"
        : t == typeof(double) ? "double"
        : t == typeof(bool) ? "bool"
        : t.Name;

    private static string Signature(Type[] takes) =>
        string.Join(", ", takes.Select((t, i) => $"{Pretty(t)} arg{i + 1}"));
}
