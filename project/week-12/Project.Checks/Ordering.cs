using System.Text.RegularExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

// Without this the runner reports checks in whatever order they happen to finish
// — never task order, and it shifts as your code changes — so the list stops
// reading as the sequence of tasks you are asked to work through.
// Check1..Check5 then sorts into task order on its own.
//
// The number is parsed and compared AS A NUMBER, not as text. Sorting these
// names as text works fine up to nine and then quietly puts Check10 between
// Check1 and Check2 — a wrong order with nothing to warn you. Anything that
// isn't Check<number> sorts last, alphabetically, rather than throwing.
[assembly: TestCaseOrderer("Project.Checks.ByCheckNumber", "Project.Checks")]

namespace Project.Checks;

public class ByCheckNumber : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> cases)
        where TTestCase : ITestCase
        => cases
            .OrderBy(c => NumberOf(c.TestMethod.Method.Name))
            .ThenBy(c => c.TestMethod.Method.Name, StringComparer.Ordinal);

    private static int NumberOf(string name)
    {
        Match m = Regex.Match(name, @"^Check(\d+)");
        return m.Success ? int.Parse(m.Groups[1].Value) : int.MaxValue;
    }
}
