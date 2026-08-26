using Xunit.Abstractions;
using Xunit.Sdk;

// Without this the runner reports checks in whatever order they happen to finish
// — never task order, and it shifts as your code changes — so the list stops
// reading as the sequence of tasks you are asked to work through.
// Check1..Check5 then sorts into task order on its own.
[assembly: TestCaseOrderer("Project.Checks.ByCheckNumber", "Project.Checks")]

namespace Project.Checks;

public class ByCheckNumber : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> cases)
        where TTestCase : ITestCase
        => cases.OrderBy(c => c.TestMethod.Method.Name, StringComparer.Ordinal);
}
