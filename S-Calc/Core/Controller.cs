using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    /// <summary>
    /// Статический класс, представляющий методы для вычисления выражений, записанных в строке
    /// </summary>
    public static class Controller
    {
        static readonly Evaluator Core;

        static string Clipboard;

        static Controller()
        {
            Core = new Evaluator(4);
            Clipboard = string.Empty;
        }

        /// <summary>
        /// Количество точек после запятой. По умолчанию равно 4, максимальное значение 15
        /// </summary>
        public static byte CalculationPrecision
        {
            get
            {
                return Core.GetDecimals();
            }
            set
            {
                Core.SetDecimals(value);
            }
        }

        /// <summary>
        /// Метод, который должен вызываться после начала работы приложения для того, чтобы
        /// были выполнены статические конструкторы необходимых типа и были десериализованы функции и константы
        /// </summary>
        public static void DoNothing()
        {
            ObjectsStorage.DoNothing();
        }

        /// <summary>
        /// Вычисление математического выражения, записанного в строке System.String. Метод является потокобезопасным.
        /// </summary>
        /// <param name="expression">Математическое выражение.</param>
        /// <param name="result">Строка System.String, содержащая результат вычислений. Это либо число System.Double, переведенное в строковое представление в соответствии с правилами форматирования типа System.Double (строка форматирования G, "общий" формат) и установленной точностью; либо сообщение об ошибке. Возвращается из метода по ссылке.</param>
        /// <param name="success">Переменная типа System.Boolean, принимающая значение True только при успешном вычислении. Возвращается из метода по ссылке.</param>
        public static void Evaluate(string expression, int CursorPosition, out string result, out bool success)
        {
            Core.Evaluate(expression, CursorPosition, out result, out success);
        }

        /// <summary>
        /// Выполнение операции Undo на истории вычисления. Полученное выражение не будет сохранено.
        /// </summary>
        /// <param name="Expression">Выражение, полученное путём операции Undo.</param>
        /// <param name="CursorPostion">Позиция курсора после операции Undo.</param>
        /// <param name="result">Результат вычисления Expression.</param>
        /// <param name="success">Был успех вычисления.</param>
        /// <returns>Успех операции Undo. Успех невозможен, если позиция курсора в буффере истории достигла правой границы.</returns>
        public static bool Undo(out string Expression, out int CursorPostion, out string result, out bool success)
        {
            if (Core.History.IsUndoable)
            {
                CalculationHistory HistoryPoint = Core.History.Undo();
                CursorPostion = HistoryPoint.CursorPosition;
                Expression = string.Copy(HistoryPoint.Expression);
                Core.Evaluate(Expression, CursorPostion, out result, out success, false);
                return false;
            }
            else
            {
                Expression = null;
                CursorPostion = 0;
                result = null;
                success = false;
                return true;
            }
        }

        /// <summary>
        /// Выполнение операции Redo на истории вычисления. Полученное выражение не будет сохранено.
        /// </summary>
        /// <param name="Expression">Выражение, полученное путём операции Redo.</param>
        /// <param name="CursorPostion">Позиция курсора после операции Redo.</param>
        /// <param name="result">Результат вычисления Expression.</param>
        /// <param name="success">Был успех вычисления.</param>
        /// <returns>Успех операции Redo. Успех невозможен, если позиция курсора в буффере истории достигла левой границы.</returns>
        public static bool Redo(out string Expression, out int CursorPostion, out string result, out bool success)
        {
            if (Core.History.IsRedoable)
            {
                CalculationHistory HistoryPoint = Core.History.Redo();
                CursorPostion = HistoryPoint.CursorPosition;
                Expression = string.Copy(HistoryPoint.Expression);
                Core.Evaluate(Expression, CursorPostion, out result, out success, false);
                return false;
            }
            else
            {
                //TODO: redo exception
                Expression = null;
                CursorPostion = 0;
                result = "Can't use redo.";
                success = false;
                return true;
            }
        }

        /// <summary>
        /// Копирование результата во ВНУТРЕННИЙ беффер обмена
        /// </summary>
        /// <param name="Result">Результат, который заносится в буффер обмена.</param>
        /// <returns>Успех операции копирования. Успех невозможен, если отсутствует результат.</returns>
        public static bool Copy(string Result)
        {
            if (string.IsNullOrEmpty(Result))
            {
                return false;
            }
            else
            {
                Clipboard = string.Copy(Result);
                return true;
            }
        }

        /// <summary>
        /// Операция Paste -- копирование строки из буффера обмена в Expression
        /// </summary>
        /// <param name="Expression">Строка, куда будет скопирован буффер обмена</param>
        /// <returns></returns>
        public static bool Paste(out string Expression)
        {
            if (string.IsNullOrEmpty(Clipboard))
            {
                Expression = null;
                return false;
            }
            else
            {
                Expression = string.Copy(Clipboard);
                return true;
            }
        }

        /// <summary>
        /// Асинхронное вычисление математического выражения, записанного в строке System.String. Метод является потокобезопасным.
        /// </summary>
        /// <param name="expression">Математическое выражение.</param>
        /// <returns>Кортеж, содержащий значение System.Boolean, которое принимает значение True только при успешном вычислении выражения, а также строку System.String, содержащую результат вычислений.</returns>
        /// <example>
        /// </example>
        public static Task<Tuple<bool, string>> EvaluateAsync(string expression, int CursorPosition)
        {
            return Core.EvaluateAsync(expression, CursorPosition);
        }

        #region OUTPUT

        /// <summary>
        /// Добавление пользовательской функции.
        /// </summary>
        /// <param name="Name">Имя функции.</param>
        /// <param name="Description">Описание функции. Передайте null для того, чтобы описание функции являлось пустой строкой.</param>
        /// <param name="FunctionText">Тело функции.</param>
        /// <param name="ArgumentNames">Имена аргументов функции. Передайте null если функция не должна иметь аргументов.</param>
        public static void AddFunction(string Name, string Description, string FunctionText, params string[] ArgumentNames)
        {
            ObjectsStorage.AddFunction(Name, Description, FunctionText, ArgumentNames);
        }

        /// <summary>
        /// Удаление пользовательской функции.
        /// </summary>
        /// <param name="Name">Имя удаляемой функции.</param>
        public static void RemoveFunction(string Name)
        {
            ObjectsStorage.RemoveFunction(Name);
        }

        /// <summary>
        /// Изменение существующей пользовательской функции.
        /// </summary>
        /// <param name="OldName">Имя старой функции.</param>
        /// <param name="Name">Имя новой функции.</param>
        /// <param name="Description">Описание новой функции. Передайте null для того, чтобы описание функции являлось пустой строкой.</param>
        /// <param name="FunctionText">Тело новой функции.</param>
        /// <param name="ArgumentNames">Имена аргументов новой функции. Передайте null если новая функция не должна иметь аргументов.</param>
        public static void ChangeFunction(string OldName, string Name, string Description, string FunctionText, params string[] ArgumentNames)
        {
            ObjectsStorage.ChangeFunction(OldName, Name, Description, FunctionText, ArgumentNames);
        }

        /// <summary>
        /// Добавление пользовательской константы.
        /// </summary>
        /// <param name="Name">Имя константы.</param>
        /// <param name="Description">Описание константы.</param>
        /// <param name="Value">Значение константы в строковом представлении.</param>
        public static void AddConstant(string Name, string Description, string Value)
        {
            ObjectsStorage.AddConstant(Name, Description, Value);
        }

        /// <summary>
        /// Удаление пользовательской константы.
        /// </summary>
        /// <param name="Name">Имя константы.</param>
        public static void RemoveConstant(string Name)
        {
            ObjectsStorage.RemoveConstant(Name);
        }

        /// <summary>
        /// Изменение пользовательской константы.
        /// </summary>
        /// <param name="OldName">Имя старой константы.</param>
        /// <param name="Name">Имя новой константы.</param>
        /// <param name="Description">Описание новой константы.</param>
        /// <param name="Value">Значение новой константы в строковом представлении.</param>
        public static void ChangeConstant(string OldName, string Name, string Description, string Value)
        {
            ObjectsStorage.ChangeConstant(OldName, Name, Description, Value);
        }

        /// <summary>
        /// Получения последовательности функций
        /// </summary>
        /// <param name="type">Требуемый тип функций</param>
        /// <param name="OrderByDescending">Сортировать ли функции по рейтингу</param>
        /// <returns></returns>
        public static IEnumerable<Function> GetFunctionList(FunctionType type, bool OrderByDescending = true)
        {
            if (OrderByDescending)
            {
                foreach (var f in ObjectsStorage.Functions.Values.OrderBy(v => v.Rating).Where(v => v.Type == type))
                {
                    yield return f;
                }
            }
            else
            {
                foreach (var f in ObjectsStorage.Functions.Values.Where(v => v.Type == type))
                {
                    yield return f;
                }
            }
        }

        /// <summary>
        /// Получение последовательности констант
        /// </summary>
        /// <param name="type">Требуемый тип констант</param>
        /// <param name="OrderByDescending">Сортировать ли константы по рейтингу</param>
        /// <returns></returns>
        public static IEnumerable<Constant> GetConstantList(ConstantType type, bool OrderByDescending = true)
        {
            if (OrderByDescending)
            {
                foreach (var c in ObjectsStorage.Constants.Values.OrderBy(v => v.Rating).Where(v =>  v.Type ==type))
                {
                    yield return c;
                }
            }
            else
            {
                foreach (var c in ObjectsStorage.Constants.Values.Where(v => v.Type == type))
                {
                    yield return c;
                }
            }
        }

        /// <summary>
        /// Плучение последовательности, что представляет собой историю
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CalculationHistory> GetHistoryList()
        {
            foreach (var h in Core.History)
            {
                yield return h;
            }
        }

        #endregion

    }
}
