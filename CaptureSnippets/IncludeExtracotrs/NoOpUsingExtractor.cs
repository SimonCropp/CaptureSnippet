namespace CaptureSnippets.IncludeExtracotrs
{
    public class NoOpUsingExtractor : IIncludeExtractor
    {
        public string Extract(string line)
        {
            return null;
        }
    }
}