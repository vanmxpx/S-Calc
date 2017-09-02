using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    [Serializable]
    public class CalculationHistory : ICloneable, IEquatable<CalculationHistory>
    {
        public readonly string Expression;

        public readonly string Result;

        public readonly int CursorPosition;

        public readonly DateTime Time;

        public CalculationHistory(string Expression, string Result, int CursorPosition, DateTime Time)
        {
            this.Expression = string.Copy(Expression);
            this.Result = string.Copy(Result);
            this.CursorPosition = CursorPosition;
            this.Time = Time;
        }

        public CalculationHistory(string Expression, string Result, int CursorPosition)
        {
            this.Expression = string.Copy(Expression);
            this.Result = string.Copy(Result);
            this.CursorPosition = CursorPosition;
            this.Time = DateTime.Now;
        }

        public CalculationHistory(string Expression, string Result)
        {
            this.Expression = string.Copy(Expression);
            this.Result = string.Copy(Result);
            CursorPosition = this.Expression.Length;
            this.Time = DateTime.Now;
        }

        public object Clone()
        {
            return new CalculationHistory(Expression, Result, CursorPosition, Time);
        }

        public override bool Equals(object obj)
        {
            return obj is CalculationHistory ? Equals((CalculationHistory)obj) : false;
        }

        public bool Equals(CalculationHistory other)
        {
            return Time == other.Time;
        }

        public override int GetHashCode()
        {
            return Time.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Expression}}}{{{Result}}}{{{Time}}}";
        }

    }
}
