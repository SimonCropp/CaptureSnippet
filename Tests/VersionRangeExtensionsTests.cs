using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class VersionRangeExtensionsTests
{

    [Test]
    public void Soft_upper_joining_soft_lower_can_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0]");
        var version2 = VersionRange.Parse("[2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.AreEqual("> 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
        Assert.AreEqual("> 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
    }

    [Test]
    public void Soft_upper_joining_hard_lower_can_merge()
    {
        var version1 = VersionRange.Parse("[1.0,2.0)");
        var version2 = VersionRange.Parse("[2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.AreEqual("≥ 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
        Assert.AreEqual("≥ 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
    }

    [Test]
    public void Soft_lower_joining_hard_upper_can_merge()
    {
        var version1 = VersionRange.Parse("[1.0,2.0]");
        var version2 = VersionRange.Parse("(2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.AreEqual("≥ 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
        Assert.AreEqual("≥ 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
    }

    [Test]
    public void Pre_cant_merge()
    {
        var version1 = VersionRange.Parse("[1.0,2.0]");
        var version2 = VersionRange.Parse("(2.0-pre,3.0)");
        VersionRange newVersion;
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
    }

    [Test]
    public void All_should_override()
    {
        var version = VersionRange.Parse("[2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.CanMerge(VersionRange.All, version, out newVersion));
        Assert.AreEqual("all", newVersion.ToFriendlyString());
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version, VersionRange.All, out newVersion));
        Assert.AreEqual("all", newVersion.ToFriendlyString());
    }

    [Test]
    public void Soft_upper_overlapping_soft_lower_can_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.1]");
        var version2 = VersionRange.Parse("[2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.AreEqual("> 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
        Assert.AreEqual("> 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
    }

    [Test]
    public void Soft_upper_overlapping_soft_lower_with_missing_version_can_merge()
    {
        var version1 = VersionRange.Parse("(,2.1]");
        var version2 = VersionRange.Parse("[2.0,)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.AreEqual("all", newVersion.ToFriendlyString());
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
        Assert.AreEqual("all", newVersion.ToFriendlyString());
    }


    [Test]
    public void Hard_upper_joining_hard_lower_can_not_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0)");
        var version2 = VersionRange.Parse("(2.0,3.0)");
        VersionRange newVersion;
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
    }

    [Test]
    public void Hard_upper_overlapping_hard_lower_can_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.1)");
        var version2 = VersionRange.Parse("(2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.AreEqual("> 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
        Assert.IsTrue(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
        Assert.AreEqual("> 1.0.0 && < 3.0.0", newVersion.ToFriendlyString());
    }

    [Test]
    public void Soft_upper_not_joining_hard_lower_should_not_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0]");
        var version2 = VersionRange.Parse("[2.1,3.0)");
        VersionRange newVersion;
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
    }

    [Test]
    public void Hard_upper_not_joining_hard_lower_can_not_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0)");
        var version2 = VersionRange.Parse("(2.1,3.0)");
        VersionRange newVersion;
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version1, version2, out newVersion));
        Assert.IsFalse(VersionRangeExtensions.CanMerge(version2, version1, out newVersion));
    }

    [Test]
    public void ToFriendlyString()
    {
        Assert.AreEqual("> 1.0.0 && < 2.0.0", VersionRange.Parse("(1.0,2.0)").ToFriendlyString());
        Assert.AreEqual("all", VersionRange.All.ToFriendlyString());
        Assert.AreEqual("none", VersionRange.None.ToFriendlyString());
    }

    [Test]
    public void MaxVersion()
    {
        bool isMaxInclusive;
        NuGetVersion maxVersion;

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(1.0,2.0)"), VersionRange.Parse("(2.1,)"), out maxVersion, out isMaxInclusive);
        Assert.IsNull(maxVersion);
        Assert.IsFalse(isMaxInclusive);

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(1.0,2.0)"), VersionRange.Parse("(2.1,]"), out maxVersion, out isMaxInclusive);
        Assert.IsNull(maxVersion);
        Assert.IsFalse(isMaxInclusive);

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(2.1,)"), VersionRange.Parse("(1.0,2.0)"), out maxVersion, out isMaxInclusive);
        Assert.IsNull(maxVersion);
        Assert.IsFalse(isMaxInclusive);

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(2.1,]"), VersionRange.Parse("(1.0,2.0)"), out maxVersion, out isMaxInclusive);
        Assert.IsNull(maxVersion);
        Assert.IsFalse(isMaxInclusive);

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(1.0,2.0)"), VersionRange.Parse("(2.1,3.0)"), out maxVersion, out isMaxInclusive);
        Assert.AreEqual("3.0", maxVersion.ToString());
        Assert.IsFalse(isMaxInclusive);

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(1.0,2.0)"), VersionRange.Parse("(2.1,3.0]"), out maxVersion, out isMaxInclusive);
        Assert.AreEqual("3.0", maxVersion.ToString());
        Assert.IsTrue(isMaxInclusive);

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(2.1,3.0)"), VersionRange.Parse("(1.0,2.0)"), out maxVersion, out isMaxInclusive);
        Assert.AreEqual("3.0", maxVersion.ToString());
        Assert.IsFalse(isMaxInclusive);

        VersionRangeExtensions.MaxVersion(VersionRange.Parse("(2.1,3.0]"), VersionRange.Parse("(1.0,2.0)"), out maxVersion, out isMaxInclusive);
        Assert.AreEqual("3.0", maxVersion.ToString());
        Assert.IsTrue(isMaxInclusive);
    }

    [Test]
    public void MinVersion()
    {
        bool isMinInclusive;
        NuGetVersion minVersion;

        VersionRangeExtensions.MinVersion(VersionRange.Parse("(1.0,2.0)"), VersionRange.Parse("(,2.1)"), out minVersion, out isMinInclusive);
        Assert.IsNull(minVersion);
        Assert.IsFalse(isMinInclusive);

        VersionRangeExtensions.MinVersion(VersionRange.Parse("(1.0,2.0)"), VersionRange.Parse("(,2.1]"), out minVersion, out isMinInclusive);
        Assert.IsNull(minVersion);
        Assert.IsFalse(isMinInclusive);

        VersionRangeExtensions.MinVersion(VersionRange.Parse("(,2.1)"), VersionRange.Parse("(1.0,2.0)"), out minVersion, out isMinInclusive);
        Assert.IsNull(minVersion);
        Assert.IsFalse(isMinInclusive);

        VersionRangeExtensions.MinVersion(VersionRange.Parse("(,2.1]"), VersionRange.Parse("(1.0,2.0)"), out minVersion, out isMinInclusive);
        Assert.IsNull(minVersion);
        Assert.IsFalse(isMinInclusive);

        VersionRangeExtensions.MinVersion(VersionRange.Parse("(1.0,2.0)"), VersionRange.Parse("(2.1,3.0)"), out minVersion, out isMinInclusive);
        Assert.AreEqual("1.0", minVersion.ToString());
        Assert.IsFalse(isMinInclusive);

        VersionRangeExtensions.MinVersion(VersionRange.Parse("[1.0,2.0)"), VersionRange.Parse("(2.1,3.0)"), out minVersion, out isMinInclusive);
        Assert.AreEqual("1.0", minVersion.ToString());
        Assert.IsTrue(isMinInclusive);

        VersionRangeExtensions.MinVersion(VersionRange.Parse("(2.1,3.0)"), VersionRange.Parse("(1.0,2.0)"), out minVersion, out isMinInclusive);
        Assert.AreEqual("1.0", minVersion.ToString());
        Assert.IsFalse(isMinInclusive);

        VersionRangeExtensions.MinVersion(VersionRange.Parse("(2.1,3.0)"), VersionRange.Parse("[1.0,2.0)"), out minVersion, out isMinInclusive);
        Assert.AreEqual("1.0", minVersion.ToString());
        Assert.IsTrue(isMinInclusive);
    }

    [Test]
    public void Overlaps()
    {
        Assert.IsFalse(VersionRange.Parse("(1.0,2.0)").OverlapsWith(VersionRange.Parse("(2.1,3.0)")));
        Assert.IsFalse(VersionRange.Parse("(2.1,3.0)").OverlapsWith(VersionRange.Parse("(1.0,2.0)")));
        Assert.IsFalse(VersionRange.Parse("(1.0,2.0)").OverlapsWith(VersionRange.Parse("(2.0,3.0)")));
        Assert.IsFalse(VersionRange.Parse("(2.0,3.0)").OverlapsWith(VersionRange.Parse("(1.0,2.0)")));
        Assert.IsTrue(VersionRange.Parse("(1.0,2.1)").OverlapsWith(VersionRange.Parse("(2.0,3.0)")));
        Assert.IsTrue(VersionRange.Parse("(2.0,3.0)").OverlapsWith(VersionRange.Parse("(1.0,2.1)")));
        Assert.IsTrue(VersionRange.Parse("(1.0,2.1)").OverlapsWith(VersionRange.Parse("(2.0,3.0)")));
        Assert.IsTrue(VersionRange.Parse("(2.0,3.0)").OverlapsWith(VersionRange.Parse("(1.0,2.1)")));
        Assert.IsTrue(VersionRange.Parse("(2.0,)").OverlapsWith(VersionRange.Parse("(,2.1)")));
        Assert.IsTrue(VersionRange.Parse("(,2.1)").OverlapsWith(VersionRange.Parse("(2.0,)")));
        Assert.IsFalse(VersionRange.Parse("(2.0,)").OverlapsWith(VersionRange.Parse("(,2.0)")));
        Assert.IsFalse(VersionRange.Parse("(,2.0)").OverlapsWith(VersionRange.Parse("(2.0,)")));
        Assert.IsFalse(VersionRange.Parse("(2.1,)").OverlapsWith(VersionRange.Parse("(,2.0)")));
        Assert.IsTrue(VersionRange.Parse("[2.1.0, )").OverlapsWith(VersionRange.Parse("[2.2.0, )")));
    }

    [Test]
    public void PrettyPrintVersion()
    {
        Assert.AreEqual("4-pre", NuGetVersion.Parse("4.0-pre").SimplePrint());
    }

    [Test]
    public void PrettyPrintVersionRange()
    {
        //TODO:
       // Assert.AreEqual("0.2.x", VersionRange.Parse("[0.2,0.3)").SimplePrint());
        Assert.AreEqual("All", VersionRange.All.SimplePrint());
        Assert.AreEqual("None", VersionRange.None.SimplePrint());
        Assert.AreEqual("1.1.x - 3.0.x", VersionRange.Parse("(1.0,3.1)").SimplePrint());
        Assert.AreEqual("1.x - 3.0.x", VersionRange.Parse("[1.0,3.1)").SimplePrint());
        Assert.AreEqual("1.x - 3.1.x", VersionRange.Parse("[1.0,3.1]").SimplePrint());
        Assert.AreEqual("1.1.x - 3.1.x", VersionRange.Parse("(1.0,3.1]").SimplePrint());

        Assert.AreEqual("1.1.x - 2.x", VersionRange.Parse("(1.0,3.0)").SimplePrint());
        Assert.AreEqual("1.x - 2.x", VersionRange.Parse("[1.0,3.0)").SimplePrint());
        Assert.AreEqual("1.x - 3.x", VersionRange.Parse("[1.0,3.0]").SimplePrint());
        Assert.AreEqual("1.1.x - 3.x", VersionRange.Parse("(1.0,3.0]").SimplePrint());

        Assert.AreEqual("1.1.x - 1.x", VersionRange.Parse("(1.0,2.0)").SimplePrint());
        Assert.AreEqual("1.x", VersionRange.Parse("[1.0,2.0)").SimplePrint());
        Assert.AreEqual("1.x - 2.x", VersionRange.Parse("[1.0,2.0]").SimplePrint());
        Assert.AreEqual("1.1.x - 2.x", VersionRange.Parse("(1.0,2.0]").SimplePrint());

        Assert.AreEqual("1-pre - N", VersionRange.Parse("[1.0-pre,)").SimplePrint());
        Assert.AreEqual("1-alpha - N", VersionRange.Parse("[1.0-alpha,)").SimplePrint());

        Assert.AreEqual("1.1.x - 2.0.x", VersionRange.Parse("(1.0,2.1)").SimplePrint());
        Assert.AreEqual("1.x - 2.0.x", VersionRange.Parse("[1.0,2.1)").SimplePrint());
        Assert.AreEqual("1.x - 2.1.x", VersionRange.Parse("[1.0,2.1]").SimplePrint());
        Assert.AreEqual("1.1.x - 2.1.x", VersionRange.Parse("(1.0,2.1]").SimplePrint());

        Assert.AreEqual("1.1.x - N", VersionRange.Parse("(1.0,]").SimplePrint());
        Assert.AreEqual("1.x - N", VersionRange.Parse("[1.0,]").SimplePrint());
        Assert.AreEqual("N - 2.0.x", VersionRange.Parse("[,2.1)").SimplePrint());
        Assert.AreEqual("N - 2.1.x", VersionRange.Parse("[,2.1]").SimplePrint());
    }
}