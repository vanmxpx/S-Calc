using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    /// <summary>
    /// Математические функции.
    /// </summary>
    static class MathematicalFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Factorial(int n)
        {
            if (n == 0) { return 1; }
            else if (n < 0)
            {
                throw new ArgumentOutOfRangeException("Internal function error: argument must not be negative.");
            }
            else
            {
                double res = 1;
                for (int k = 1; k <= n; k++)
                {
                    res *= k;
                }
                return res;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DoubleFactorial(int n)
        {
            return n % 2 == 0 ? Math.Pow(2, n / 2) * Factorial(n / 2) : Factorial(n) / (Math.Pow(2, (n - 1) / 2) * Factorial((n - 1) / 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arsh(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x + 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arch(double x)
        {
            return Math.Log(x + Math.Sqrt(x + 1) * Math.Sqrt(x - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arth(double x)
        {
            return 0.5 * Math.Log((1 + x) / (1 - x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arcth(double x)
        {
            return 0.5 * Math.Log((x + 1) / (x - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arsech(double x)
        {
            return Math.Log((1 / x) + Math.Sqrt((1 / x) + 1) * Math.Sqrt((1 / x) - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arcsch(double x)
        {
            return Math.Log((1 / x) + Math.Sqrt((1 / (x * x)) + 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random()
        {
            RandomNumberGenerator rnd = RandomNumberGenerator.Create();
            byte[] bytes = new byte[8];
            rnd.GetBytes(bytes);
            UInt64 val = BitConverter.ToUInt64(bytes, 0);
            double res = (val / (double)ulong.MaxValue);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 RandomPositiveInteger(UInt64 from, UInt64 to)
        {
            if (to <= from)
            {
                throw new ArgumentOutOfRangeException("Internal function error: first argument must be less than second argument.");
            }
            RandomNumberGenerator rnd = RandomNumberGenerator.Create();
            byte[] bytes = new byte[8];
            rnd.GetBytes(bytes);
            UInt64 val = BitConverter.ToUInt64(bytes, 0);
            UInt64 res = val % (to - from + 1) + from;
            return res;
        }
    }
}
