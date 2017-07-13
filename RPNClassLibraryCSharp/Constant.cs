using System;
using System.Globalization;
using System.Linq;

namespace RPNClassLibraryCSharp
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    struct Constant : IEquatable<Constant>, IComparable<Constant>
    {
        public readonly double Value;

        public readonly string Name;

        public readonly string Description;

        public Constant(string Name, string Description, double Value)
        {
            if (string.IsNullOrEmpty(Name) || char.IsDigit(Name[0]))
            {
                throw new ArgumentException("Wrong constant name.");
            }
            else
            {
                Name = Name.ToLowerInvariant();
                if (ObjectsStorage.AllFunctionNames.Contains(Name) || ObjectsStorage.AllConstantNames.Contains(Name))
                {
                    throw new ArgumentException("Function or constant with the same name is already exists.");
                }
            }
            this.Name = Name;
            if (string.IsNullOrEmpty(Description))
            {
                this.Description = string.Empty;
            }
            else
            {
                this.Description = Description;
            }
            this.Value = Value;
        }

        public Constant(string Name, string Description, string Value) : this(Name, Description, double.Parse(Value, CultureInfo.InvariantCulture))
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Constant ? Equals((Constant)obj) : false;
        }

        public bool Equals(Constant other)
        {
            return string.Equals(Name, other.Name) && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Value.GetHashCode();
        }

        public override string ToString()
        {
            return Name +"; " + Value.ToString(CultureInfo.InvariantCulture);
        }

        public int CompareTo(Constant other)
        {
            int i1 = string.Compare(Name, other.Name);
            if (i1 == 0)
            {
                return Value.CompareTo(other.Value);
            }
            else
            {
                return i1;
            }
        }
    }
}
