using System;
using System.Collections.Generic;
using System.Linq;
using CaptureSnippets;

static class SnippetExtensions
{
    public static Dictionary<string, IReadOnlyList<Snippet>> ToDictionary(this IEnumerable<Snippet> value)
    {
        //TODO: throw if mixing
        //if (package == Package.Undefined)
        //{
        //    if (!string.IsNullOrWhiteSpace(targetPackage))
        //    {
        //        throw new Exception("Cannot mix non-empty targetPackage with a snippet that is Package.Undefined.");
        //    }
        //    packageText = "";
        //}
        return value
            .GroupBy(_ => _.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: _ => _.Key,
                elementSelector: _ => _.ToReadonlyList(),
                comparer: StringComparer.OrdinalIgnoreCase);
    }
}