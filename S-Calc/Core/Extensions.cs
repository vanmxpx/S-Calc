using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    /// <summary>
    /// Методы расширений.
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Может ли строка определять число.
        /// </summary>
        /// <param name="s">Строка, часть математического выражения, токен.</param>
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
        /// Получения индекса первого вхождения элемента типа Т в коллекцию, реализующую IList of T. Возвращает -1, если элементы не найдены.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции.</typeparam>
        /// <param name="list">Коллекция, в который выполняется поиск индекса.</param>
        /// <param name="element">Элемент, который ищества в коллекции.</param>
        /// <returns></returns>
        public static int FirstIndexOf<T>(this IList<T> list, T element) where T : IEquatable<T>
        {
            int i = 0;
            foreach (var e in list)
            {
                if (e.Equals(element)) { return i; }
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Добавление всех ключей словаря в хэштаблицу.
        /// </summary>
        /// <typeparam name="T">Тип элементов хэштаблицы и ключей словаря.</typeparam>
        /// <typeparam name="TValue">Тип значений словаря.</typeparam>
        /// <param name="hashtable">Хэштаблица.</param>
        /// <param name="dictionary">Словарь.</param>
        public static void AddAllKeysToHashSet<T, TValue>(this HashSet<T> hashtable, IDictionary<T, TValue> dictionary)
        {
            foreach (var e in dictionary.Keys)
            {
                hashtable.Add(e);
            }
        }

    }
}
