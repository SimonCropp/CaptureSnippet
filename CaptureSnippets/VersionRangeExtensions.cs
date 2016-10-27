using System;
using System.Linq;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeExtensions
    {
        public static string NextVersion(this SemanticVersion version)
        {
            if (version.IsPrerelease)
            {
                throw new Exception("Cannot increment pre-release version");
            }
            if (version.Patch > 0)
            {
                return $"{version.Major}.{version.Minor}.{version.Patch + 1}";
            }
            if (version.Minor > 0)
            {
                return $"{version.Major}.{version.Minor}.1";
            }
            return $"{version.Major}.1";
        }

        public static string PreviousVersion(this SemanticVersion version)
        {
            if (version.IsPrerelease)
            {
                throw new Exception("Cannot decrement pre-release version");
            }
            if (version.Patch > 0)
            {
                return $"{version.Major}.{version.Minor}.{version.Patch - 1}";
            }
            if (version.Minor > 0)
            {
                return $"{version.Major}.{version.Minor - 1}";
            }
            return $"{version.Major - 1}.x";
        }
        static SemanticVersion PreviousVersion2(this SemanticVersion version)
        {
            if (version.IsPrerelease)
            {
                throw new Exception("Cannot decrement pre-release version");
            }
            if (version.Patch > 0)
            {
                return new SemanticVersion(version.Major,version.Minor,version.Patch - 1);
            }
            if (version.Minor > 0)
            {
                return new SemanticVersion(version.Major, version.Minor-1, 0);
            }
            return new SemanticVersion(version.Major-1, 0, 0);
        }

        public static string SimplePrint(this NuGetVersion version)
        {
            if (version.Patch > 0)
            {
                if (version.IsPrerelease)
                {
                    return $"{version.Major}.{version.Minor}.{version.Patch}-{version.ReleaseLabels.First()}";
                }
                return $"{version.Major}.{version.Minor}.{version.Patch}";
            }
            if (version.Minor > 0)
            {
                if (version.IsPrerelease)
                {
                    return $"{version.Major}.{version.Minor}-{version.ReleaseLabels.First()}";
                }
                return $"{version.Major}.{version.Minor}";
            }
            if (version.IsPrerelease)
            {
                return $"{version.Major}-{version.ReleaseLabels.First()}";
            }
            return $"{version.Major}.x";
        }

        public static bool IsPreRelease(this VersionRange range)
        {
            if (range.MinVersion != null && range.MinVersion.IsPrerelease)
            {
                return true;
            }
            if (range.MaxVersion != null && range.MaxVersion.IsPrerelease)
            {
                return true;
            }
            return false;
        }

        public static string SimplePrint(this VersionRange range)
        {
            if (range.Equals(VersionRange.All) || range.Equals(VersionRange.AllStable))
            {
                return "All";
            }
            if (range.Equals(VersionRange.None))
            {
                return "None";
            }
            var minVersion = range.MinVersion;
            var maxVersion = range.MaxVersion;

            if (range.HasUpperBound && maxVersion.IsPrerelease)
            {
                var message = $"Pre release not allowed on upper bound '{range}'.";
                throw new Exception(message);
            }
            if (range.HasLowerBound && minVersion.IsPrerelease && !range.IsMinInclusive)
            {
                var message = $"Pre release not allowed on non-inclusive lower bound '{range}'.";
                throw new Exception(message);
            }
            if (range.HasLowerAndUpperBounds)
            {
                if (minVersion.Minor == 0 &&
                    minVersion.Patch == 0 &&
                    maxVersion.Minor == 0 &&
                    maxVersion.Patch == 0)
                {
                    if (range.IsMinInclusive &&
                        !range.IsMaxInclusive &&
                        minVersion.Major + 1 == maxVersion.Major)
                    {
                        return minVersion.SimplePrint();
                    }
                }


                // single version
                if (maxVersion.Equals(minVersion) && range.IsMinInclusive && range.IsMaxInclusive)
                {
                    return minVersion.SimplePrint();
                }
                // TODO:
                //if (minVersion.Equals(maxVersion.p) && range.IsMinInclusive && range.IsMaxInclusive)
                //{
                //    return minVersion.SimplePrint();
                //}
            }

            var builder = StringBuilderCache.Acquire();
            // normal range

            if (range.HasLowerBound)
            {
                if (range.IsMinInclusive)
                {
                    builder.Append(minVersion.SimplePrint());
                }
                else
                {
                    builder.Append(minVersion.NextVersion());
                }
            }
            else
            {
                builder.Append("N");
            }


            if (range.HasUpperBound)
            {
                if (range.IsMaxInclusive)
                {
                    builder.Append(" - ");
                    builder.Append(maxVersion.SimplePrint());
                }
                else
                {
                    var previousVersion2 = maxVersion.PreviousVersion2();
                    if (previousVersion2 > minVersion)
                    {
                        builder.Append(" - ");
                        builder.Append(maxVersion.PreviousVersion());
                    }
                }
            }
            else
            {
                builder.Append(" - ");
                builder.Append("N");
            }

            return StringBuilderCache.GetStringAndRelease(builder);
        }

        public static string ToFriendlyString(this VersionRange version)
        {
            if (version.Equals(VersionRange.All))
            {
                return "all";
            }
            if (version.Equals(VersionRange.None))
            {
                return "none";
            }
            return version.PrettyPrint()
                .TrimStart('(')
                .TrimEnd(')');
        }

    }
}