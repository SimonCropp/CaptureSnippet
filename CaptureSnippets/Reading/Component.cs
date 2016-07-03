using System;

namespace CaptureSnippets
{
    public struct Component:IComparable<Component>
    {

        public string ValueOrUndefined => this == Undefined ? "Undefined" : Value;

        string value;

        public int CompareTo(Component other) => ValueOrUndefined.CompareTo(other.ValueOrUndefined);

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(Component a, Component b)
        {
            return a.value == b.value;
        }


        public static bool operator !=(Component a, Component b)
        {
            return a.value != b.value;
        }

        public string Value
        {
            get
            {
                if (this == Undefined)
                {
                    throw new Exception("Cannot access Value when is Package.All.");
                }
                return value;
            }
        }

        public static Component Undefined;

        public Component(string value)
        {
            Guard.AgainstNullAndEmpty(value, "package");
            if (value == "Undefined")
            {
                throw new ArgumentException("'Undefined' is not an allowed value.", nameof(value));
            }
            this.value = value;
        }

        static Component()
        {
            Undefined = new Component();
        }

        public static implicit operator string(Component package)
        {
            if (package.value == null)
            {
                throw new Exception("Cannot convert Component.Undefined to a string.");
            }
            return package.Value;
        }

        public static implicit operator Component(string value)
        {
            Guard.AgainstNull(value, nameof(value));
            return new Component(value);
        }

    }
}