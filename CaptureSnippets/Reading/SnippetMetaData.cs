using System;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Version={Version}, Package={Package}")]
    public class SnippetMetaData
    {

        public readonly bool UseParentVersion;
        public readonly bool UseParentPackage;

        SnippetMetaData(VersionRange version, Package package, bool useParentVersion, bool useParentPackage)
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

        public static SnippetMetaData With(VersionRange version, Package package)
        {
            Guard.AgainstNull(version, "version");
            Guard.AgainstNull(package, "package");
            return new SnippetMetaData(version, package, false, false);
        }

        public static SnippetMetaData WithParentVersion(Package package)
        {
            return new SnippetMetaData(null, package, true, false);
        }

        public static SnippetMetaData WithParentPackage(VersionRange version)
        {
            return new SnippetMetaData(version, null, false, true);
        }

        public static SnippetMetaData WithParent()
        {
            return new SnippetMetaData(null, null, true, true);
        }
    }
}