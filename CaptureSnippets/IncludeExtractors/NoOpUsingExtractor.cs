using System;

namespace CaptureSnippets.IncludeExtractors
{
    public class NoOpUsingExtractor
    {
        public static Func<string, string> Extract = line => null;
    }
}