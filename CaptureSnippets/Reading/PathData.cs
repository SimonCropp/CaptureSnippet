using System;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Version={Version}, Package={Package.ValueOrUndefined}")]
    public class PathData
    {
        public readonly bool UseParentVersion;
        public readonly bool UseParentPackage;

        PathData(VersionRange version, Package package, bool useParentVersion, bool useParentPackage)
        {
            this.version = version;
            this.package = package;
            UseParentVersion = useParentVersion;
            UseParentPackage = useParentPackage;
        }

        Package package;
        public Package Package
        {
            get
            {
                if (UseParentPackage)
                {
                    throw new Exception("Cannot access Version when UseParentPackage.");
                }
                return package;
            }
        }

        VersionRange version;
        public VersionRange Version
        {
            get
            {
                if (UseParentVersion)
                {
                    throw new Exception("Cannot access Version when UseParentVersion.");
                }
                return version;
            }
        }

        public static PathData With(VersionRange version, Package package)
        {
            Guard.AgainstNull(version, "version");
            Guard.AgainstNull(package, "package");
            return new PathData(version, package, false, false);
        }

        public static PathData WithParentVersion(Package package)
        {
            Guard.AgainstNull(package, "package");
            return new PathData(null, package, true, false);
        }

        public static PathData WithParentPackage(VersionRange version)
        {
            Guard.AgainstNull(version, "version");
            return new PathData(version, null, false, true);
        }

        public static PathData WithParent()
        {
            return new PathData(null, null, true, true);
        }
    }
}