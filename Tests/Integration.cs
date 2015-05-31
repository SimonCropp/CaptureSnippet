using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[Explicit]
public class Integration
{
    [Test]
    public async void Foo()
    {
        var extractor = new SnippetExtractor(InferVersion);
        var snippets = await extractor.FromFiles(Directory.EnumerateFiles(@"C:\Code\docs.particular.net\Snippets", "*.cs",SearchOption.AllDirectories));

        Debug.WriteLine(snippets.ToList());
        
    }

    public static VersionRange InferVersion(string path)
    {
        var extension = Path.GetExtension(path);
        path = path.Substring(0, path.Length - extension.Length);
        while (true)
        {
            var name = Path.GetFileName(path);
            if (name == "Snippets")
            {
                return VersionRange.All;
            }
            if (name == "Snippets_Misc")
            {
                return VersionRange.All;
            }
            VersionRange version;
            if (TryParseVersion(path, out version))
            {
                return version;
            }

            var parent = Directory.GetParent(path);
            if (parent == null)
            {
                break;
            }
            path = parent.FullName;
        }

        return VersionRange.All;
    }

    public static bool TryParseVersion(string directory, out VersionRange version)
    {
        string pretext = null;
        var preReleaseFilePath = Path.Combine(directory, "prerelease.txt");
        if (File.Exists(preReleaseFilePath))
        {
            pretext = "pre";
            var fileContents = File.ReadAllText(preReleaseFilePath).Trim();
            if (!string.IsNullOrWhiteSpace(fileContents))
            {
                pretext = fileContents;
            }
        }
        var value = directory.Split('_').Last();
        int majorPart;
        if (int.TryParse(value, out majorPart))
        {
            if (pretext == null)
            {
                version = new VersionRange(
                    minVersion: new SemanticVersion(majorPart, 0, 0),
                    includeMinVersion: true,
                    maxVersion: new SemanticVersion(majorPart + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
            version = new VersionRange(
                minVersion: new SemanticVersion(majorPart, 0, 0, new[]
                {
                    pretext
                }, null),
                includeMinVersion: true);
            return true;
        }

        NuGetVersion minVersion;
        if (pretext == null)
        {
            if (NuGetVersion.TryParse(value, out minVersion))
            {
                version = new VersionRange(
                    minVersion: minVersion,
                    includeMinVersion: true,
                    maxVersion: new SemanticVersion(minVersion.Major + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
        }
        else
        {
            var valueWithPre = value + "-" + pretext;
            if (!NuGetVersion.TryParse(valueWithPre, out minVersion))
            {
                var message = string.Format("Could not use prerelease.txt to parse a SemanticVersion. Value attempted:'{0}'.", valueWithPre);
                throw new Exception(message);
            }
            version = new VersionRange(
                minVersion: minVersion,
                includeMinVersion: true);
            return true;
        }
        VersionRange versionRange;
        if (VersionRange.TryParse(value, out versionRange))
        {
            version = versionRange;
            return true;
        }
        version = null;
        return false;
    }
}