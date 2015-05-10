using System;

namespace CaptureSnippets
{
    /// <summary>
    /// A simplified Major.Minor.Patch clone of <see cref="System.Version"/>.
    /// </summary>
    public class Version 
    {
        int major;
        int? minor;
        int? patch;
        public static Version ExplicitEmpty = new Version();
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
            if (IsExplicitEmptyVersion)
            {
                throw new Exception("Cannot convert an explicit empty version to a string.");
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

        public bool IsExplicitEmptyVersion
        {
            get { return this == ExplicitEmpty; }
        }

        public int? Minor
        {
            get
            {
                ThrowForExplcitEmpty(); return minor;
            }
            private set { minor = value; }
        }

        public int Major
        {
            get
            {
                ThrowForExplcitEmpty(); return major;
            }
            private set { major = value; }
        }

        public int? Patch
        {
            get
            {
                ThrowForExplcitEmpty();
                return patch;
            }
            private set { patch = value; }
        }

        void ThrowForExplcitEmpty()
        {
            if (IsExplicitEmptyVersion)
            {
                throw new Exception("Cannot access this property for an ExplicitEmpty.");
            }
        }
    }
}