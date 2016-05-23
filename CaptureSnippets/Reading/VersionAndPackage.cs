using System;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Version={Version}, Package={Package.ValueOrNone}")]
    public class VersionAndPackage
    {

        public readonly bool UseParentVersion;
        public readonly bool UseParentPackage;

        VersionAndPackage(VersionRange version, Package package, bool useParentVersion, bool useParentPackage)
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

        public static VersionAndPackage With(VersionRange version, Package package)
        {
            Guard.AgainstNull(version, "version");
            Guard.AgainstNull(package, "package");
            return new VersionAndPackage(version, package, false, false);
        }

        public static VersionAndPackage WithParentVersion(Package package)
        {
            return new VersionAndPackage(null, package, true, false);
        }

        public static VersionAndPackage WithParentPackage(VersionRange version)
        {
            return new VersionAndPackage(version, null, false, true);
        }

        public static VersionAndPackage WithParent()
        {
            return new VersionAndPackage(null, null, true, true);
        }
    }
}