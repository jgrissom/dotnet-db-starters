// ═══════════════════════════════════════════════════════════════════
//  READ-ONLY. This half is plumbing.
//
//  Here is the thing worth understanding about it, because it is the reason
//  your homework has any fixed rules in it at all:
//
//  I do not know what your project is about. I have never seen your class
//  names. Somebody in this room is building a registry of lighthouses and
//  somebody else is building one of competitive cheese. So these checks
//  know exactly ONE name — Registry — and find everything else THROUGH it:
//
//      Registry.NewItem("...")  hands back one of YOUR records,
//      and its type is whatever you called it.
//
//  That is the whole trick, and it is why NewItem has to be there even
//  though your program barely uses it. It is the door.
//
//  Everything else — what your record is called, what it holds, what your
//  program prints — is yours, this week and every week after it.
//
//  The actual checks are in ProjectChecks.cs next door.
// ═══════════════════════════════════════════════════════════════════
using System.Reflection;

namespace Project.Checks;

internal static class StudentCode
{
    // Your program's assembly, found by the name the homework fixed: Project.
    private static Assembly TheProject => Assembly.Load("Project");

    internal const string RegistryHint =
        "The homework asks for a file called Registry.cs holding a class called Registry —\n"
        + "the one class in your project whose SHAPE is not up to you:\n"
        + "    public class Registry\n"
        + "    {\n"
        + "        private readonly List<Lighthouse> _items = new List<Lighthouse>();\n"
        + "        public static string Topic => \"Lighthouses of the Outer Banks\";\n"
        + "        public Lighthouse NewItem(string name) => new Lighthouse(name);\n"
        + "        public void Add(Lighthouse item) { _items.Add(item); }\n"
        + "        public int Count => _items.Count;\n"
        + "        public List<Lighthouse> All() { return new List<Lighthouse>(_items); }\n"
        + "    }\n"
        + "Lighthouse is my example. Yours is whatever your topic is made of.\n";

    internal static Type RegistryType()
    {
        var type = TheProject.GetTypes().FirstOrDefault(t => t.Name == "Registry");

        Assert.True(type != null,
            "I can't find a class called Registry in your Project.\n"
            + RegistryHint
            + "Spelling and capitals matter for this one name — Registry, exactly.\n"
            + "👉 Next: make Project/Registry.cs and put the class in it.");

        Assert.True(type!.IsPublic,
            "Your Registry class exists but isn't public, so nothing outside your own "
            + "program can see it — including these checks.\n"
            + "    public class Registry\n"
            + "The word public is the whole fix.\n"
            + "👉 Next: add `public` in front of `class Registry`.");

        return type;
    }

    // A new, empty registry. Needs a constructor that takes nothing — which is
    // what you get for free unless you write one that takes something.
    internal static object NewRegistry()
    {
        var type = RegistryType();
        try
        {
            return Activator.CreateInstance(type)!;
        }
        catch (Exception e)
        {
            throw new Xunit.Sdk.XunitException(
                $"`new Registry()` didn't work — it threw {(e.InnerException ?? e).GetType().Name}.\n"
                + "Registry has to be creatable with nothing passed in, because that is how "
                + "your program and these checks both start one:\n"
                + "    var registry = new Registry();\n"
                + "If you wrote a constructor that takes arguments, add one that takes none.\n"
                + "👉 Next: check the constructors on your Registry class.");
        }
    }

    internal static MethodInfo RequireMethod(string name, params Type[] takes)
    {
        var type = RegistryType();
        var candidates = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                             .Where(m => m.Name == name)
                             .ToList();

        Assert.True(candidates.Count > 0,
            $"Registry has no public method called {name}.\n"
            + RegistryHint
            + $"👉 Next: add {name} to Project/Registry.cs.");

        var method = candidates.FirstOrDefault(m =>
            m.GetParameters().Select(p => p.ParameterType).SequenceEqual(takes));

        Assert.True(method != null,
            $"Registry.{name} exists, but not with the parameters the homework asks for.\n"
            + $"    yours:  {name}({string.Join(", ", candidates[0].GetParameters().Select(p => p.ParameterType.Name))})\n"
            + $"    wanted: {name}({string.Join(", ", takes.Select(t => t.Name))})\n"
            + "The types in the brackets are part of a method's identity in C#.");

        return method!;
    }

    // The factory — and the only reason these checks can touch your records at
    // all. Whatever type comes back is YOUR record type, and everything else
    // is worked out from it.
    internal static object NewItem(object registry, string name)
    {
        var method = RequireMethod("NewItem", typeof(string));
        object? made;
        try
        {
            made = method.Invoke(registry, new object?[] { name });
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.NewItem(\"{name}\") threw {e.InnerException.GetType().Name} instead "
                + $"of handing back a record:\n    {e.InnerException.Message}\n"
                + "NewItem has to work for any ordinary name — it is how the rest of your "
                + "program, and these checks, make one of your things.");
        }

        Assert.True(made != null,
            "Registry.NewItem(...) handed back null instead of one of your records.\n"
            + "    public Lighthouse NewItem(string name) => new Lighthouse(name);\n"
            + "One line. It makes a valid, ordinary record and returns it.");

        return made!;
    }

    // Your record type, learned from the contract rather than guessed at.
    internal static Type ItemType() => RequireMethod("NewItem", typeof(string)).ReturnType;

    internal static void Add(object registry, object item) =>
        RequireMethod("Add", ItemType()).Invoke(registry, new[] { item });

    internal static int Count(object registry)
    {
        var property = RegistryType().GetProperty("Count",
            BindingFlags.Public | BindingFlags.Instance);

        Assert.True(property != null && property.PropertyType == typeof(int),
            "Registry has no public int property called Count.\n"
            + "    public int Count => _items.Count;\n"
            + "Ask the list. Counting along in a separate variable is a second number that "
            + "can disagree with the first.\n"
            + "👉 Next: add Count to Project/Registry.cs.");

        return (int)property!.GetValue(registry)!;
    }

    internal static System.Collections.IList All(object registry)
    {
        var result = RequireMethod("All").Invoke(registry, null);

        Assert.True(result is System.Collections.IList,
            "Registry.All() didn't hand back a list.\n"
            + "    public List<Lighthouse> All() { return new List<Lighthouse>(_items); }\n"
            + "It hands back a COPY of what the registry is holding.");

        return (System.Collections.IList)result!;
    }

    // Properties on YOUR record — read by shape, never by name.
    internal static PropertyInfo[] Properties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    internal static FieldInfo[] PublicFields(Type type) =>
        type.GetFields(BindingFlags.Public | BindingFlags.Instance);

    // A deliberately silly value of the right type, for "does anything refuse?"
    internal static object? NonsenseFor(Type t) =>
        t == typeof(string) ? "   "
        : t == typeof(int) ? -1
        : t == typeof(double) ? -1.0
        : t == typeof(decimal) ? -1m
        : null;
}
