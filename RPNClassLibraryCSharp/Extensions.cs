using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RPNClassLibraryCSharp
{
    /// <summary>
    /// 
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this string s)
        {
            switch (s)
            {
                case "pi":
                case "e":
                case "gamma": //постоянная Эйлера-Маскерони
                case "phi": return true; //золотое сечение
                default:
                    return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double tmpResult);
            }
        }
    }
}
