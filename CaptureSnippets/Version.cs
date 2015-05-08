using System;
using System.Runtime.InteropServices;

namespace CaptureSnippets
{

    /// <summary>
    /// A simplified Major.Minor.Patch clone of <see cref="System.Version"/>.
    /// </summary>
    [ComVisible(true)]
    public sealed class Version : IComparable, IComparable<Version>, IEquatable<Version>
    {

        public readonly int Major;
        public readonly int? Minor;
        public readonly int? Patch;

        public Version(int major, int minor, int patch)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException("major");
            if (minor < 0)
                throw new ArgumentOutOfRangeException("minor");
            if (patch < 0)
                throw new ArgumentOutOfRangeException("patch");
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public Version(int major, int minor)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException("major");
            if (minor < 0)
                throw new ArgumentOutOfRangeException("minor");
            Major = major;
            Minor = minor;
        }

        public Version(int major)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException("major");
            Major = major;
        }

        public static bool operator ==(Version v1, Version v2)
        {
            if (ReferenceEquals(v1, null))
                return ReferenceEquals(v2, null);
            return v1.Equals(v2);
        }

        public static bool operator !=(Version v1, Version v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(Version v1, Version v2)
        {
            if (v1 == null)
                throw new ArgumentNullException("v1");
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(Version v1, Version v2)
        {
            if (v1 == null)
                throw new ArgumentNullException("v1");
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >(Version v1, Version v2)
        {
            return v2 < v1;
        }

        public static bool operator >=(Version v1, Version v2)
        {
            return v2 <= v1;
        }

        public int CompareTo(object version)
        {
            if (version == null)
                return 1;
            var version1 = version as Version;
            if (version1 == null)
                throw new ArgumentException();
            if (Major != version1.Major)
                return Major > version1.Major ? 1 : -1;
            if (Minor != version1.Minor)
                return Minor > version1.Minor ? 1 : -1;
            if (Patch == version1.Patch)
                return 0;
            return Patch > version1.Patch ? 1 : -1;
        }

        public int CompareTo(Version value)
        {
            return VersionComparer.Compare(value,this);
        }

        public override bool Equals(object obj)
        {
            var version = obj as Version;
            return !(version == null) && Major == version.Major && (Minor == version.Minor) && Patch == version.Patch;
        }

        public bool Equals(Version obj)
        {
            return !(obj == null) && Major == obj.Major && (Minor == obj.Minor) && Patch == obj.Patch;
        }

        public override int GetHashCode()
        {
            return 0 | (Major & 15) << 28 | (Minor.GetValueOrDefault(-1) & byte.MaxValue) << 20 | Patch.GetValueOrDefault(-1) & 4095;
        }

        public override string ToString()
        {
            if (Patch == null)
            {
                if (Minor == null)
                {
                    return Major.ToString();
                }

                return Major + "." + Minor;
            }

            return Major + "." + Minor+ "." + Patch;
        }
    }
}