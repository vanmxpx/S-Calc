using System;
using System.Globalization;
using System.Linq;

namespace S_Calc.Core
{
    /// <summary>
    /// Тип константы
    /// </summary>
    public enum ConstantType
    {
        /// <summary>
        /// Внетренняя
        /// </summary>
        Internal,
        /// <summary>
        /// Пользовательская
        /// </summary>
        Custom
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class Constant : IEquatable<Constant>, IComparable<Constant>, ICalculatorUnit
    {
        readonly double value;

        readonly bool removable;

        public bool IsRemovable => removable;

        public double Value
        {
            get
            {
                rating++;
                if (rating.Equals(UInt32.MaxValue))
                {
                    ObjectsStorage.ResetConstantsRating();
                }
                return value;
            }
        }

        readonly string name;

        public string Name => name;

        readonly string description;

        public string Description => description;

        public readonly ConstantType Type;

        UInt32 rating;

        public UInt32 Rating => rating;

        public Constant(string Name, string Description, double Value, bool IsRemovable, ConstantType type)
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
            this.name = Name;
            if (string.IsNullOrEmpty(Description))
            {
                this.description = string.Empty;
            }
            else
            {
                this.description = Description;
            }
            this.value = Value;
            this.Type = type;
            this.rating = 0;
            this.removable = IsRemovable;
        }

        public Constant(string Name, string Description, string Value, bool IsRemovable, ConstantType type) : this(Name, Description, double.Parse(Value, CultureInfo.InvariantCulture), IsRemovable, type)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Constant ? Equals((Constant)obj) : false;
        }

        public bool Equals(Constant other)
        {
            return string.Equals(name, other.name) && value == other.value;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() ^ value.GetHashCode();
        }

        public override string ToString()
        {
            return name +"; " + value.ToString(CultureInfo.InvariantCulture);
        }

        public int CompareTo(Constant other)
        {
            return rating.CompareTo(other.rating);
            //int i1 = string.Compare(Name, other.Name);
            //if (i1 == 0)
            //{
            //    return value.CompareTo(other.value);
            //}
            //else
            //{
            //    return i1;
            //}
        }

        public void ResetRating()
        {
            rating = 0;
        }

    }
}
