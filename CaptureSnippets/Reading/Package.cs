using System;

namespace CaptureSnippets
{
    public struct Package:IComparable<Package>
    {

        public string ValueOrUndefined => this == Undefined ? "Undefined" : Value;

        string value;

        public int CompareTo(Package other) => ValueOrUndefined.CompareTo(other.ValueOrUndefined);

        public override string ToString()
        {
            return Value;
        }

        public string Value
        {
            get
            {
                if (this == Undefined)
                {
                    throw new Exception("Cannot access Value when is Package.Undefined.");
                }
                return value;
            }
        }

        public static Package Undefined;

        public Package(string value)
        {
            Guard.AgainstNullAndEmpty(value, nameof(value));
            if (value == "Undefined")
            {
                throw new ArgumentException("'Undefined' is not an allowed value.", nameof(value));
            }
            this.value = value;
        }


        static Package()
        {
            Undefined = new Package();
        }

        public static bool operator ==(Package a, Package b)
        {
            return a.value == b.value;
        }


        public static bool operator !=(Package a, Package b)
        {
            return a.value != b.value;
        }

        public static implicit operator string(Package package)
        {
            if (package.value == null)
            {
                throw new Exception("Cannot convert Package.Undefined to a string.");
            }
            return package.Value;
        }

        public static implicit operator Package(string value)
        {
            Guard.AgainstNull(value, nameof(value));
            return new Package(value);
        }

    }
}