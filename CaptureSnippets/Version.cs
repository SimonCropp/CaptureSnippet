using System;

namespace CaptureSnippets
{

    /// <summary>
    /// A simplified Major.Minor.Patch clone of <see cref="System.Version"/>.
    /// </summary>
    public sealed class Version 
    {

        public readonly int Major;
        public readonly int? Minor;
        public readonly int? Patch;
        public static Version ExplicitNull = new Version();
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

        Version()
        {
        }

        public override string ToString()
        {
            if (IsExplicitNullVersion)
            {
                throw new Exception("Cannot convert a null version to a string.");
            }
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

        public bool IsExplicitNullVersion
        {
            get { return this == ExplicitNull; }
        }
    }
}