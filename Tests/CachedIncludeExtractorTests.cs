using System.IO;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class CachedIncludeExtractorTests
{

    [Test]
    public void EnsureErrorsAreReturned()
    {
        var directory = "badincludes".ToCurrentDirectory();
        var extractor = new CachedIncludeExtractor(
            extractIncludeData: path =>
            {
                path = Path.GetFileName(path);
                path = path.Replace(".include", "");
                var split = path.Split('_');
                if (split.Length == 1)
                {
                    return IncludeData.With(path, VersionRange.All, Package.Undefined, Component.Undefined);
                }
                var key = split[0];
                var package = split[1];
                return IncludeData.With(key, VersionRange.All, package, Component.Undefined);
            },
            extractPathData: path => PathData.WithParent());
        var read = extractor.FromDirectory(directory);
        Assert.AreEqual(1, read.GroupingErrors.Count);
    }
}