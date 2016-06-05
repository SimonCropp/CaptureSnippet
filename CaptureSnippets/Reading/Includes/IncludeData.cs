using System;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Key={Key}, Version={Version}, Package={Package.ValueOrUndefined}, Component={Component.ValueOrUndefined}")]
    public class IncludeData
    {

        public readonly bool UseParentVersion;
        public readonly bool UseParentPackage;
        public readonly bool UseParentComponent;
        public readonly string Key;

        IncludeData(string key, VersionRange version, Package package, Component component, bool useParentVersion, bool useParentPackage, bool useParentComponent)
        {
            Key = key;
            this.version = version;
            this.package = package;
            this.component = component;
            UseParentVersion = useParentVersion;
            UseParentPackage = useParentPackage;
            UseParentComponent = useParentComponent;
        }

        Package package;

        public Package Package
        {
            get
            {
                if (UseParentPackage)
                {
                    throw new Exception("Cannot access Package when UseParentPackage.");
                }
                return package;
            }
        }

        Component component;

        public Component Component
        {
            get
            {
                if (UseParentComponent)
                {
                    throw new Exception("Cannot access Component when UseParentComponent.");
                }
                return component;
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

        public static IncludeData With(string key, VersionRange version, Package package, Component component)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(package, nameof(package));
            Guard.AgainstNull(component, nameof(component));
            return new IncludeData(key, version, package, component, false, false, false);
        }

        public static IncludeData WithParentVersion(string key, Package package, Component component)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(package, nameof(package));
            Guard.AgainstNull(component, nameof(component));
            return new IncludeData(key, null, package, component, true, false, false);
        }

        public static IncludeData WithParentVersionAndComponent(string key, Package package)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(package, nameof(package));
            return new IncludeData(key, null, package, null, true, false, true);
        }

        public static IncludeData WithParentPackage(string key, VersionRange version, Component component)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(component, nameof(component));
            return new IncludeData(key, version, null, component, false, true, false);
        }

        public static IncludeData WithParentPackageAndComponent(string key, VersionRange version)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(version, nameof(version));
            return new IncludeData(key, version, null, null, false, true, true);
        }

        public static IncludeData WithParentPackageAndVersion(string key, Component component)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(component, nameof(component));
            return new IncludeData(key, null, null, component, true, true, false);
        }

        public static IncludeData WithParent(string key)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            return new IncludeData(key, null, null, null, true, true, true);
        }
    }
}