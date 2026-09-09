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
//  ⚠️ NEW THIS WEEK: three members, and their signatures are part of the
//  deal the way every dictated name has been since week 4:
//
//      public List<string> Names()
//      public List<YourRecord> Sorted()
//      public List<YourRecord> Matching(string term)
//
//  ⭐ Notice what these checks do NOT do: they never look for a property
//  called Name. They cannot — yours might be Corner or Title or Elevation.
//  So Sorted() is verified by REFERENCE: three records go in, and the
//  checks ask your own Find for each one and compare what came back with
//  what Sorted() put in each position. Names() is verified against the
//  three strings the checks handed NewItem in the first place.
//
//  Week 8's Save and Load are still dictated, and the PATH is still handed
//  in — which is what lets these checks hand your registry a scratch file
//  in the system's temp folder rather than the one your program uses.
//
//  Nothing in here looks for a method by name on YOUR record, and nothing
//  reads a word of what your Kind or your Line() actually SAY. They ask
//  whether the promise is kept — which is the only question an interface
//  ever asks about anything.
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
        + "        public void Save(string path) { ... }      // week 8\n"
        + "        public void Load(string path) { ... }      // week 8\n"
        + "        public List<string> Names() { ... }        // week 9\n"
        + "        public List<Lighthouse> Sorted() { ... }   // week 9\n"
        + "        public List<Lighthouse> Matching(string term) { ... }   // week 9\n"
        + "    }\n"
        + "Lighthouse is my example. Yours is whatever your topic is made of.\n"
        + "(Find, Remove, Kind, Line and Everything are there too, from weeks 5 and 6.)\n";

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

    internal static void Add(object registry, object item)
    {
        var method = RequireMethod("Add", ItemType());
        try
        {
            method.Invoke(registry, new[] { item });
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.Add(...) threw {e.InnerException.GetType().Name} instead of "
                + $"putting a record on the books:\n    {e.InnerException.Message}\n"
                + "⚠️ On an EMPTY registry, which is where this one broke. Add asks Find "
                + "first (week 7's guard), so a Find that cannot cope with finding nothing "
                + "takes Add down with it.\n"
                + "  • It is FirstOrDefault, never First. First OBJECTS to an empty "
                + "sequence — it throws InvalidOperationException — and answering \"nothing "
                + "here\" is half of what Find is for.\n"
                + "  • Same for Last/Single: the ...OrDefault versions hand back null.\n"
                + "👉 Next: read Find. Then run your own program — the first record you add "
                + "goes onto an empty registry too.");
        }
    }

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

    // ── week 5 ─────────────────────────────────────────────────────────────

    internal static Type FindReturnType() => RequireMethod("Find", typeof(string)).ReturnType;

    internal static object? Find(object registry, string name)
    {
        var method = RequireMethod("Find", typeof(string));

        Assert.True(method.ReturnType == ItemType(),
            $"Registry.Find hands back a {method.ReturnType.Name}, and it has to hand back "
            + $"one of your records — a {ItemType().Name}, the same type NewItem makes.\n"
            + $"    public {ItemType().Name}? Find(string name)\n"
            + "The `?` on the end is the important part: it is you telling the compiler that "
            + "this method is allowed to come back empty-handed.");

        try
        {
            return method.Invoke(registry, new object?[] { name });
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.Find(\"{name}\") threw {e.InnerException.GetType().Name} instead of "
                + $"answering:\n    {e.InnerException.Message}\n"
                + "Find has to cope with a name nobody has — answering \"nothing here\" is "
                + "half of what it is for.\n"
                + "⚠️ If you rewrote Find this week: it is FirstOrDefault, never First. "
                + "First OBJECTS to an empty sequence and throws "
                + "InvalidOperationException; FirstOrDefault hands back null, which is what "
                + "`return null;` after the loop used to do.");
        }
    }

    internal static bool Remove(object registry, string name)
    {
        var method = RequireMethod("Remove", typeof(string));

        Assert.True(method.ReturnType == typeof(bool),
            $"Registry.Remove hands back a {method.ReturnType.Name}, and it has to hand back "
            + "a bool — true if it took a record off the books, false if there was nothing by "
            + "that name to take:\n"
            + "    public bool Remove(string name)");

        try
        {
            return (bool)method.Invoke(registry, new object?[] { name })!;
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.Remove(\"{name}\") threw {e.InnerException.GetType().Name} instead of "
                + $"answering:\n    {e.InnerException.Message}\n"
                + "Remove has to cope with a name nobody has. Ask Find first, and when Find "
                + "comes back null there is simply nothing to do — say so and return false.");
        }
    }

    // Properties the outside world can read and cannot write. Week 4 made you
    // seal one; week 5 asks what moves it.
    internal static PropertyInfo[] SealedProperties(Type type) =>
        Properties(type).Where(p => p.CanRead && p.GetSetMethod(nonPublic: false) == null)
                        .ToArray();

    internal static Dictionary<string, object?> Snapshot(object item)
    {
        var shot = new Dictionary<string, object?>();
        foreach (var p in Properties(item.GetType()))
        {
            if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
            try { shot[p.Name] = p.GetValue(item); } catch { /* not our business */ }
        }
        return shot;
    }

    // What changed between two snapshots, by property name.
    internal static string[] Moved(Dictionary<string, object?> before, object item)
    {
        var after = Snapshot(item);
        return before.Where(kv => after.ContainsKey(kv.Key) && !Equals(kv.Value, after[kv.Key]))
                     .Select(kv => kv.Key)
                     .ToArray();
    }

    // Every public method on the record that we could actually call. Property
    // accessors, operators and the ones every object inherits are not verbs.
    internal static string VerbNames(Type type)
    {
        var names = Verbs(type).Select(m => m.Name + "()").ToArray();
        return names.Length == 0 ? "there are none" : string.Join(", ", names);
    }

    internal static MethodInfo[] Verbs(Type type) =>
        type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.IsSpecialName && m.DeclaringType == type)
            .ToArray();

    private static (bool ok, object? value) ValueFor(Type t, bool alternate)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;
        if (t == typeof(string)) return (true, alternate ? "something else" : "something");
        if (t == typeof(int)) return (true, alternate ? 2 : 1);
        if (t == typeof(long)) return (true, alternate ? 2L : 1L);
        if (t == typeof(short)) return (true, alternate ? (short)2 : (short)1);
        if (t == typeof(double)) return (true, alternate ? 2.0 : 1.0);
        if (t == typeof(float)) return (true, alternate ? 2f : 1f);
        if (t == typeof(decimal)) return (true, alternate ? 2m : 1m);
        if (t == typeof(bool)) return (true, !alternate);
        if (t == typeof(char)) return (true, alternate ? 'y' : 'x');
        if (t == typeof(DateTime)) return (true, new DateTime(2026, alternate ? 9 : 8, 18));
        if (t == typeof(DateOnly)) return (true, new DateOnly(2026, alternate ? 9 : 8, 18));
        if (t == typeof(TimeOnly)) return (true, new TimeOnly(3, alternate ? 15 : 14));
        if (t == typeof(TimeSpan)) return (true, TimeSpan.FromMinutes(alternate ? 2 : 1));
        if (t.IsEnum)
        {
            var values = Enum.GetValues(t);
            if (values.Length == 0) return (false, null);
            return (true, values.GetValue(alternate && values.Length > 1 ? 1 : 0));
        }
        return (false, null);
    }

    internal static bool CanCall(MethodInfo m, bool alternate, out object?[] args)
    {
        var parameters = m.GetParameters();
        args = new object?[parameters.Length];
        foreach (var (p, i) in parameters.Select((p, i) => (p, i)))
        {
            if (p.ParameterType.IsByRef) return false;
            var (ok, value) = ValueFor(p.ParameterType, alternate);
            if (!ok) return false;
            args[i] = value;
        }
        return true;
    }

    // Call one method on one record and report which sealed properties moved.
    // Tried twice, and with a second set of arguments, so a flag that only
    // flips one way and a counter both count as movement.
    internal static string[] WhatItMoves(object item, MethodInfo verb)
    {
        var sealedNames = SealedProperties(item.GetType()).Select(p => p.Name).ToHashSet();
        var before = Snapshot(item);

        foreach (var alternate in new[] { false, true, false })
        {
            if (!CanCall(verb, alternate, out var args)) return Array.Empty<string>();
            try { verb.Invoke(item, args); }
            catch (TargetInvocationException) { /* their rule, their call */ }
        }

        return Moved(before, item).Where(sealedNames.Contains).ToArray();
    }

    // The record's own verb: the first public method that moves something the
    // outside world cannot write. Null when there isn't one.
    internal static MethodInfo? TheVerb(object registry)
    {
        foreach (var verb in Verbs(ItemType()))
        {
            var fresh = NewItem(registry, "Something Ordinary");
            if (WhatItMoves(fresh, verb).Length > 0) return verb;
        }
        return null;
    }

    // ── week 6 ─────────────────────────────────────────────────────────────

    internal const string ListedHint =
        "The homework asks for an interface called IListed — the second thing in this\n"
        + "course whose shape is not up to you:\n"
        + "    public interface IListed\n"
        + "    {\n"
        + "        string Kind { get; }\n"
        + "        string Line();\n"
        + "    }\n"
        + "Put it in Project/IListed.cs. It is not a class and there is nothing in it\n"
        + "to make — it is a list of what a thing has to be able to answer.\n";

    internal static Type ListedType()
    {
        var type = TheProject.GetTypes().FirstOrDefault(t => t.Name == "IListed");

        Assert.True(type != null,
            "I can't find anything called IListed in your Project.\n"
            + ListedHint
            + "Spelling and capitals matter for this one name — IListed, exactly, capital I.\n"
            + "👉 Next: make Project/IListed.cs.");

        Assert.True(type!.IsInterface,
            $"IListed exists, but it is a {(type.IsClass ? "class" : "type")} rather than an "
            + "interface.\n"
            + ListedHint
            + "The word is `interface`, not `class`. A class says what something IS and can "
            + "be made with `new`; an interface says only what something can ANSWER, and "
            + "there is never anything to make.");

        Assert.True(type.IsPublic,
            "IListed isn't public, so nothing outside your program can see it — including "
            + "these checks.\n    public interface IListed");

        var kind = type.GetProperty("Kind");
        Assert.True(kind != null && kind.PropertyType == typeof(string) && kind.CanRead,
            "IListed has no readable string property called Kind.\n"
            + ListedHint
            + "Kind is the one word in the left-hand column of your listing. A payphone "
            + "says PAYPHONE; the registry itself says REGISTRY.");

        var line = type.GetMethod("Line");
        Assert.True(line != null
                    && line.ReturnType == typeof(string)
                    && line.GetParameters().Length == 0,
            "IListed has no method `string Line()` that takes nothing.\n"
            + ListedHint
            + "Line() is the rest of the row, and it is the part each kind of thing writes "
            + "for itself.");

        return type;
    }

    internal static string KindOf(object thing)
    {
        var value = ListedType().GetProperty("Kind")!.GetValue(thing);

        Assert.True(value is string,
            $"{thing.GetType().Name}.Kind handed back {(value == null ? "null" : "something that isn't a string")}. "
            + "It is one word for the left-hand column:\n"
            + "    public string Kind => \"PAYPHONE\";");

        return (string)value!;
    }

    internal static string LineOf(object thing)
    {
        object? value;
        try
        {
            value = ListedType().GetMethod("Line")!.Invoke(thing, null);
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"{thing.GetType().Name}.Line() threw {e.InnerException.GetType().Name}:\n"
                + $"    {e.InnerException.Message}\n"
                + "Line() has to work on an ordinary record with ordinary values in it.");
        }

        Assert.True(value is string,
            $"{thing.GetType().Name}.Line() handed back "
            + $"{(value == null ? "null" : "something that isn't a string")}.");

        return (string)value!;
    }

    internal static bool Keeps(Type type) => ListedType().IsAssignableFrom(type);

    internal static System.Collections.IList Everything(object registry)
    {
        var method = RequireMethod("Everything");
        var wanted = typeof(List<>).MakeGenericType(ListedType());

        Assert.True(method.ReturnType == wanted,
            $"Registry.Everything() hands back a {method.ReturnType.Name}, and it has to hand "
            + "back a list of the promise:\n"
            + "    public List<IListed> Everything()\n"
            + "That type in the angle brackets is the whole point. A List<IListed> will hold "
            + "your records AND your registry, because the only thing it asks of anything is "
            + "whether it keeps the promise.");

        object? result;
        try
        {
            result = method.Invoke(registry, null);
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.Everything() threw {e.InnerException.GetType().Name}:\n"
                + $"    {e.InnerException.Message}");
        }

        Assert.True(result is System.Collections.IList,
            "Registry.Everything() handed back null instead of a list.");

        return (System.Collections.IList)result!;
    }

    // ── week 8 ─────────────────────────────────────────────────────────────
    // Save and Load. Both are dictated by signature, and both take the path
    // rather than knowing one.

    internal static void Save(object registry, string path)
    {
        var method = RequireMethod("Save", typeof(string));

        Assert.True(method.ReturnType == typeof(void),
            $"Registry.Save hands back a {method.ReturnType.Name}, and it is dictated to "
            + "hand back nothing:\n"
            + "    public void Save(string path)");

        try
        {
            method.Invoke(registry, new object?[] { path });
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.Save(\"{path}\") threw {e.InnerException.GetType().Name}:\n"
                + $"    {e.InnerException.Message}\n"
                + "Save is handed a full path to a file that may or may not exist yet, in a "
                + "folder that does. Writing to it should be one call — and the path it "
                + "writes to is the one it was HANDED, never a name written inside it.");
        }
    }

    internal static void Load(object registry, string path)
    {
        var method = RequireMethod("Load", typeof(string));

        Assert.True(method.ReturnType == typeof(void),
            $"Registry.Load hands back a {method.ReturnType.Name}, and it is dictated to "
            + "hand back nothing — it fills the registry in rather than answering:\n"
            + "    public void Load(string path)");

        try
        {
            method.Invoke(registry, new object?[] { path });
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.Load(\"{path}\") threw {e.InnerException.GetType().Name}:\n"
                + $"    {e.InnerException.Message}\n"
                + "Load has to cope with a path that has no file at it — that is a first run, "
                + "not a failure. Ask File.Exists(path) before you read anything, and simply "
                + "return when the answer is no.");
        }
    }

    // ── week 9 ─────────────────────────────────────────────────────────────
    // Three queries. Every one is dictated by signature, and not one of them
    // needs these checks to know a single thing about YOUR record's names.

    private static System.Collections.IList AskList(string name, object registry,
        Type wanted, params object?[] args)
    {
        var method = RequireMethod(name, args.Select(a => a?.GetType() ?? typeof(string)).ToArray());

        Assert.True(method.ReturnType == wanted,
            $"Registry.{name} hands back a {Pretty(method.ReturnType)}, and it is dictated "
            + $"to hand back a {Pretty(wanted)}:\n"
            + $"    public {Pretty(wanted)} {name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})\n"
            + "⚠️ The ToList() on the end of a query is what turns it into a list. Without "
            + "it you hand back an IEnumerable — a RECIPE for working the answer out, which "
            + "re-runs every time anybody looks at it.");

        object? result;
        try
        {
            result = method.Invoke(registry, args);
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new Xunit.Sdk.XunitException(
                $"Registry.{name}(...) threw {e.InnerException.GetType().Name} instead of "
                + $"answering:\n    {e.InnerException.Message}\n"
                + "⚠️ Two of this week's rewrites throw where a loop simply answered:\n"
                + "  • First(), Last() and Single() OBJECT to an empty sequence. The\n"
                + "    ...OrDefault versions hand back null instead.\n"
                + "  • MaxBy and MinBy hand back NULL for an empty list, so asking their\n"
                + "    answer for a property straight off dies. Put a ?. in front of it.\n"
                + "👉 Next: run your own program and try the same thing by hand.");
        }

        Assert.True(result is System.Collections.IList,
            $"Registry.{name}(...) handed back null instead of a list. A query that finds "
            + "nothing hands back an EMPTY list — never null, and never an error.");

        return (System.Collections.IList)result!;
    }

    // IEnumerable`1 is not a thing a student can read. Render it the way they
    // would write it.
    private static string Pretty(Type t) =>
        t.IsGenericType
            ? t.Name.Substring(0, t.Name.IndexOf('`'))
              + "<" + string.Join(", ", t.GetGenericArguments().Select(Pretty)) + ">"
            : t.Name;

    internal static List<string> Names(object registry)
    {
        var list = AskList("Names", registry, typeof(List<string>));
        return list.Cast<string?>().Select(s => s ?? "(null)").ToList();
    }

    internal static List<object?> Sorted(object registry)
    {
        var wanted = typeof(List<>).MakeGenericType(ItemType());
        return AskList("Sorted", registry, wanted).Cast<object?>().ToList();
    }

    internal static List<object?> Matching(object registry, string term)
    {
        var wanted = typeof(List<>).MakeGenericType(ItemType());
        return AskList("Matching", registry, wanted, term).Cast<object?>().ToList();
    }

    // A scratch file of this check's own, deleted first so a previous run
    // cannot pass a check for you.
    //
    // ⚠️ Not a plain name: `dotnet test` runs with its working directory inside
    // Project.Checks/bin/Debug/net10.0 while `dotnet run` stands at the top of
    // your repo, so "registry.json" means two different files depending on
    // which command you typed. Measured, not assumed.
    internal static string Scratch(string name)
    {
        var path = Path.Combine(Path.GetTempPath(), $"project-check-{name}");
        File.Delete(path);
        return path;
    }
}
