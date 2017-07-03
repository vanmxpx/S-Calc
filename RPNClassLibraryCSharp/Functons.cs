using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RPNClassLibraryCSharp
{
    /// <summary>
    /// 
    /// </summary>
    static class Functions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Factorial(int n)
        {
            if (n == 0) { return 1; }
            else if (n < 0)
            {
                throw new ArgumentOutOfRangeException("Argument must not be negative");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DoubleFactorial(int n)
        {
            return n % 2 == 0 ? Math.Pow(2, n / 2) * Factorial(n / 2) : Factorial(n) / (Math.Pow(2, (n - 1) / 2) * Factorial((n - 1) / 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arsh(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x + 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arch(double x)
        {
            return Math.Log(x + Math.Sqrt(x + 1) * Math.Sqrt(x - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arth(double x)
        {
            return 0.5 * Math.Log((1 + x) / (1 - x));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arcth(double x)
        {
            return 0.5 * Math.Log((x + 1) / (x - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arsech(double x)
        {
            return Math.Log((1 / x) + Math.Sqrt((1 / x) + 1) * Math.Sqrt((1 / x) - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Arcsch(double x)
        {
            return Math.Log((1 / x) + Math.Sqrt((1 / (x * x)) + 1));
        }
    }
}
