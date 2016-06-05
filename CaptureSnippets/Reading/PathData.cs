using System;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Version={Version}, Package={Package.ValueOrUndefined}, Component={Component.ValueOrUndefined}")]
    public class PathData
    {
        public readonly bool UseParentVersion;
        public readonly bool UseParentPackage;
        public readonly bool UseParentComponent;

        PathData(VersionRange version, Package package, Component component, bool useParentVersion, bool useParentPackage, bool useParentComponent)
        {
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

        public static PathData With(VersionRange version, Package package, Component component)
        {
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(package, nameof(package));
            Guard.AgainstNull(component, nameof(component));
            return new PathData(version, package, component, false, false, false);
        }

        public static PathData WithParentVersion(Package package, Component component)
        {
            Guard.AgainstNull(package, nameof(package));
            Guard.AgainstNull(component, nameof(component));
            return new PathData(null, package, component, true, false, false);
        }

        public static PathData WithParentVersionAndComponent(Package package)
        {
            Guard.AgainstNull(package, nameof(package));
            return new PathData(null, package, null, true, false, true);
        }

        public static PathData WithParentComponent(VersionRange version, Package package)
        {
            Guard.AgainstNull(package, nameof(package));
            return new PathData(version, package, null, false, false, true);
        }

        public static PathData WithParentVersionAndPackage(Component component)
        {
            Guard.AgainstNull(component, nameof(component));
            return new PathData(null, null, component, true, true, false);
        }

        public static PathData WithParentPackage(VersionRange version, Component component)
        {
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(component, nameof(component));
            return new PathData(version, null, component, false, true, false);
        }

        public static PathData WithParentPackageAndComponent(VersionRange version)
        {
            Guard.AgainstNull(version, nameof(version));
            return new PathData(version, null, null, false, true, true);
        }

        public static PathData WithParent()
        {
            return new PathData(null, null, null, true, true, true);
        }
    }
}