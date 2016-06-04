using System;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Key={Key}, Version={Version}, Package={Package.ValueOrUndefined}")]
    public class IncludeData
    {

        public readonly bool UseParentVersion;
        public readonly bool UseParentPackage;
        public readonly string Key;

        IncludeData(string key, VersionRange version, Package package, bool useParentVersion, bool useParentPackage)
        {
            Key = key;
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

        public static IncludeData With(string key,VersionRange version, Package package)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(package, nameof(package));
            return new IncludeData(key, version, package, false, false);
        }

        public static IncludeData WithParentVersion(string key,Package package)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(package, nameof(package));
            return new IncludeData(key, null, package, true, false);
        }

        public static IncludeData WithParentPackage(string key, VersionRange version)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(version, nameof(version));
            return new IncludeData(key, version, null, false, true);
        }

        public static IncludeData WithParent(string key)
        {
            return new IncludeData(key, null, null, true, true);
        }
    }
}