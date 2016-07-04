using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class LoopStateTests
{
    [Test]
    public void TrimIndentation()
    {
        var loopState = new LoopState();
        loopState.AppendLine("   Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        Approvals.Verify(loopState.GetLines());
    }

    [Test]
    public void ExcludeEmptyPaddingLines()
    {
        var loopState = new LoopState();
        loopState.AppendLine("   ");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   ");
        Approvals.Verify(loopState.GetLines());
    }

    [Test]
    public void TrimIndentation_with_mis_match()
    {
        var loopState = new LoopState();
        loopState.AppendLine("      Line2");
        loopState.AppendLine("   ");
        loopState.AppendLine("     Line4");
        Approvals.Verify(loopState.GetLines());
    }

    [Test]
    public void ExcludeEmptyPaddingLines_empty_list()
    {
        var loopState = new LoopState();
        Approvals.Verify(loopState.GetLines());
    }

    [Test]
    public void ExcludeEmptyPaddingLines_whitespace_list()
    {
        var loopState = new LoopState();
        loopState.AppendLine("");
        loopState.AppendLine("  ");
        Approvals.Verify(loopState.GetLines());
    }

    [Test]
    public void TrimIndentation_no_initial_padding()
    {
        var loopState = new LoopState();
        loopState.AppendLine("Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        Approvals.Verify(loopState.GetLines());
    }
}