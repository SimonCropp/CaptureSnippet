using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
public class VersionRangeExtensionsTests
{

    [Test]
    public void Soft_upper_joining_soft_lower_should_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0]");
        var version2 = VersionRange.Parse("[2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.ShouldMerge(version1, version2, out newVersion));
        Assert.AreEqual("(1.0.0, 3.0.0)", newVersion.ToString());
        Assert.IsTrue(VersionRangeExtensions.ShouldMerge(version2, version1, out newVersion));
        Assert.AreEqual("(1.0.0, 3.0.0)", newVersion.ToString());
    }
    [Test]
    public void Soft_upper_overlapping_soft_lower_should_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.1]");
        var version2 = VersionRange.Parse("[2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.ShouldMerge(version1, version2, out newVersion));
        Assert.AreEqual("(1.0.0, 3.0.0)", newVersion.ToString());
        Assert.IsTrue(VersionRangeExtensions.ShouldMerge(version2, version1, out newVersion));
        Assert.AreEqual("(1.0.0, 3.0.0)", newVersion.ToString());
    }

    [Test]
    public void Hard_upper_joining_hard_lower_should_not_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0)");
        var version2 = VersionRange.Parse("(2.0,3.0)");
        VersionRange newVersion;
        Assert.IsFalse(VersionRangeExtensions.ShouldMerge(version1, version2, out newVersion));
        Assert.IsFalse(VersionRangeExtensions.ShouldMerge(version2, version1, out newVersion));
    }

    [Test]
    public void Hard_upper_overlapping_hard_lower_should_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.1)");
        var version2 = VersionRange.Parse("(2.0,3.0)");
        VersionRange newVersion;
        Assert.IsTrue(VersionRangeExtensions.ShouldMerge(version1, version2, out newVersion));
        Assert.AreEqual("(1.0.0, 3.0.0)", newVersion.ToString());
        Assert.IsTrue(VersionRangeExtensions.ShouldMerge(version2, version1, out newVersion));
        Assert.AreEqual("(1.0.0, 3.0.0)", newVersion.ToString());
    }

    [Test]
    public void Soft_upper_not_joining_hard_lower_should_not_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0]");
        var version2 = VersionRange.Parse("[2.1,3.0)");
        VersionRange newVersion;
        Assert.IsFalse(VersionRangeExtensions.ShouldMerge(version1, version2, out newVersion));
        Assert.IsFalse(VersionRangeExtensions.ShouldMerge(version2, version1, out newVersion));
    }

    [Test]
    public void Hard_upper_not_joining_hard_lower_should_not_merge()
    {
        var version1 = VersionRange.Parse("(1.0,2.0)");
        var version2 = VersionRange.Parse("(2.1,3.0)");
        VersionRange newVersion;
        Assert.IsFalse(VersionRangeExtensions.ShouldMerge(version1, version2, out newVersion));
        Assert.IsFalse(VersionRangeExtensions.ShouldMerge(version2, version1, out newVersion));
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
    }
}