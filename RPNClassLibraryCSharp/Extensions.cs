using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (ObjectsStorage.AllConstantNames.Contains(s))
            {
                return true;
            }
            else
            {
                return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double tmpResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int FirstIndexOf<T>(this IList<T> seq, T element) where T : IEquatable<T>
        {
            int i = 0;
            foreach (var e in seq)
            {
                if (e.Equals(element)) { return i; }
                i++;
            }
            return -1;
        }

        public static void AddAllKeysToHashSet<T, TValue>(this HashSet<T> collection, IDictionary<T, TValue> dictionary)
        {
            foreach (var e in dictionary.Keys)
            {
                collection.Add(e);
            }
        }

    }
}
