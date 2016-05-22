using System;

namespace CaptureSnippets
{
    public class Package:IComparable<Package>
    {

        public string ValueOrNone => this == None ? "None" : Value;

        string value;

        public int CompareTo(Package other) => ValueOrNone.CompareTo(other.ValueOrNone);

        public override string ToString()
        {
            return Value;
        }

        public string Value
        {
            get
            {
                if (this == None)
                {
                    throw new Exception("Cannot access Value when is Package.None.");
                }
                return value;
            }
        }

        public static Package None;

        public Package(string value)
        {
            Guard.AgainstNullAndEmpty(value, "package");
            if (value == "None")
            {
                throw new ArgumentException("'None' is not an allowed value.", nameof(value));
            }
            this.value = value;
        }

        Package()
        {

        }

        static Package()
        {
            None = new Package();
        }

        public static implicit operator string(Package package)
        {
            if (package == None)
            {
                throw new Exception("Cannot convert Package.None to a string.");
            }
            return package.Value;
        }

        public static implicit operator Package(string value)
        {
            return new Package(value);
        }

    }
}