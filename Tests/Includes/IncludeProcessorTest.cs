using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(DiffReporter), typeof(AllFailingTestsClipboardReporter))]
public class IncludeProcessorTest
{

    string source = Path.Combine(TestContext.CurrentContext.TestDirectory, "Includes");

    [Test]
    public async Task Simple()
    {
        var result = await Process(@"
include: key
");
        Approvals.Verify(result);
    }

    async Task<string> Process(string markdownContent)
    {
        var versionAndPackage = VersionAndPackage.With(VersionRange.All, Package.None);
        var extractor = new IncludeExtractor(path => versionAndPackage);
        var readIncludes = extractor.FromDirectory(source);
        var includeGroups = IncludeGrouper.Group(readIncludes);
        var processor = new MarkdownProcessor(
            snippets: new List<SnippetGroup>(),
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup,
            includes: includeGroups,
            appendIncludeGroup: SimpleIncludeMarkdownHandling.AppendGroup);
        var stringBuilder = new StringBuilder();
        string result;
        using (var reader = new StringReader(markdownContent))
        using (var writer = new StringWriter(stringBuilder))
        {
            await processor.Apply(reader, writer);
            result = stringBuilder.ToString();
        }
        return result;
    }

    [Test]
    public async Task MixedCaseInclude()
    {
        var result = await Process(@"
inClUde: mIXedcaSe
");
        Approvals.Verify(result);
    }

    [Test]
    public async Task MissingKey()
    {
        var result = await Process(
            @"
line1
include: missingkey
line2
");
        Approvals.Verify(result);
    }

    [Test]
    public async Task WrappedWithOtherContent()
    {
        var result = await Process(@"
before
include: key
after
");
        Approvals.Verify(result);
    }

    [Test]
    public async Task Nested()
    {
        var result = await Process(@"
include: nested.parent
");
        Approvals.Verify(result);
    }

    //    [Test]
    //    public void Infinite()
    //    {
    //        var processor = new IncludeProcessor(new Inputs
    //        {
    //            SourceDirectory = source
    //        }, null);
    //        processor.ReadIncludes().GetAwaiter().GetResult();
    //        var exception = Assert.Throws<Exception>(() => processor.Process("include: infinite"));
    //        Approvals.Verify(exception.Message);
    //    }

}