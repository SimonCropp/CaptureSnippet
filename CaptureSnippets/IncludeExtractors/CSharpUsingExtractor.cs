using System;

namespace CaptureSnippets.IncludeExtractors
{
    public static class CSharpUsingExtractor
    {
        const string Pattern = "using ";

        /// <summary>
        /// Extracts the include parts from each line of snippet
        /// </summary>
        /// <param name="line">single line in the snippet</param>
        /// <returns>Returns the include, or Null if nothing is found.</returns>
        public static Func<string, string> Extract = line =>
        {
            var trimmedLine = line.Trim();
            var usingIndex = trimmedLine.IndexOf(Pattern, StringComparison.InvariantCultureIgnoreCase);
            
            if (!trimmedLine.StartsWith("//") && usingIndex > -1 && trimmedLine.EndsWith(";", StringComparison.Ordinal))
            {
                string result;

                var ns = trimmedLine.Substring(usingIndex + Pattern.Length);
                var aliasIndex = ns.IndexOf("=", StringComparison.Ordinal);
                
                if (aliasIndex > -1)
                {
                    result = ns.Substring(aliasIndex + 1).Trim();
                }
                else
                {
                    result = ns;
                }

                return result.Substring(0, result.Length - 1); //skip the ending semicolon
            }

            return null;
        };
    }
}