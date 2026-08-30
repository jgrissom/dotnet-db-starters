// ═══════════════════════════════════════════════════════════════════
//  YOURS. First file in this course that tests instead of being tested.
//
//  One fact ships written — the worked example. Read it before you write
//  anything: those four lines are the whole anatomy, and every check
//  that has ever graded you is this same shape with more to say.
//
//  Your four go below it, one per task. Name each one after the rule it
//  proves — the name is the first thing you read when it fails.
// ═══════════════════════════════════════════════════════════════════

namespace Lab.Tests;

public class DeskTests
{
    // The worked example. Three parts, always these three, always in this
    // order: set the scene, do the thing, check the answer. Here the scene
    // is free (Broadcast is static), the doing and the checking share a
    // line — your four below will need all three spelled out.
    //
    // [Fact] is what makes it a test: the runner finds every method marked
    // with it and calls them all, every time, in under a second.
    [Fact]
    public void TheStationKnowsItsOwnName()
    {
        Assert.Equal("KDXR", Broadcast.CallSign());
    }

    // TODO — Task 2: the clock. One assert per value you feed it, and feed
    // it at least one time whose seconds are under ten.

    // TODO — Task 3: the ad buy. Make a one-run Ad, air it more times than
    // it has runs, and pin down where Remaining is allowed to stop.

    // TODO — Task 4: the switchboard. Put a caller on the board, Take that
    // name, and prove what comes back — Assert.Same is the question to ask.

    // TODO — Task 5: the hour. Air an hour holding an Ad and prove the line
    // the desk prints shows the buy AFTER the airing, not before.
}
