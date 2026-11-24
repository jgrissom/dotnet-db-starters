// Your instructor's fact about the Welcome column, written before the fix.

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Lab.Migrations;

namespace Lab.Tests;

public class WelcomeTests
{
    [Fact]
    public void TheOldCallersAreWelcome()
    {
        Migration migration = new CallersAreWelcome();

        AddColumnOperation welcome = migration.UpOperations
            .OfType<AddColumnOperation>()
            .Single(op => op.Name == "Welcome");

        Assert.Equal(true, welcome.DefaultValue);
    }
}
