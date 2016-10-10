using NUnit.Framework;

public static class Scrubber
{
    public static string Scrub(string s)
    {
        var testDirectory = TestContext.CurrentContext.TestDirectory;
        return s.Replace(@"\\", @"\")
            .ReplaceCaseless(testDirectory, @"root\");
    }
}

