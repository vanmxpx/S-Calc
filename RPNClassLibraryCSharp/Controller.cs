using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RPNClassLibraryCSharp
{
    /// <summary>
    /// Статический класс, представляющий методы для вычисления выражений, записанных в строке
    /// </summary>
    public static class Controller
    {
        /// <summary>
        /// Количество точек после запятой.
        /// </summary>
        static byte decimals = 4;

        /// <summary>
        /// Строка форматирования.
        /// </summary>
        const string formatString = ":G}";

        /// <summary>
        /// Количество точек после запятой при выводе. Допускаются значения от 0 до 15 включительно.
        /// </summary>
        public static byte Decimals
        {
            get
            {
                return decimals;
            }
            set
            {
                decimals = value <= 15 ? value : decimals;
            }
        }

        readonly static object locker = new object();

        /// <summary>
        /// Запись выражения в постфиксную нотацию (обратную польскую нотацию).
        /// </summary>
        /// <param name="l">Список токенов.</param>
        /// <param name="s">Стек.</param>
        /// <param name="q">Очередь.</param>
        internal static void Write(List<string> l, Stack<string> s, Queue<string> q)
        {
            foreach (string str in l)
            {
                if (str.IsNumber()) { q.Enqueue(str); }
                else if (ObjectsStorage.AllFunctionNames.Contains(str)) { s.Push(str); }
                else if (str == ",")
                {
                    try
                    {
                        while (s.Peek() != "(")
                        {
                            q.Enqueue(s.Pop());
                        }
                    }
                    catch
                    {
                        throw new ArgumentException("The separator was misplaced or \"(\" were mismatched.");
                    }
                }
                else if (str != "!" && ObjectsStorage.OperatorPriority.ContainsKey(str))
                {
                    while (s.Count > 0 && ObjectsStorage.OperatorPriority.ContainsKey(s.Peek()) && ObjectsStorage.OperatorPriority[str] <= ObjectsStorage.OperatorPriority[s.Peek()])
                    {
                        q.Enqueue(s.Pop());
                    }
                    s.Push(str);
                }
                else if (str == "!" && ObjectsStorage.OperatorPriority.ContainsKey(str))
                {
                    while (s.Count > 0 && ObjectsStorage.OperatorPriority.ContainsKey(s.Peek()) && ObjectsStorage.OperatorPriority[str] < ObjectsStorage.OperatorPriority[s.Peek()])
                    {
                        q.Enqueue(s.Pop());
                    }
                    s.Push(str);
                }
                else if (str == "(") { s.Push(str); }
                else if (str == ")")
                {
                    try
                    {
                        while (s.Peek() != "(")
                        {
                            q.Enqueue(s.Pop());
                        }
                    }
                    catch
                    {
                        throw new ArgumentException("Expect \"(\".");
                    }
                    s.Pop();
                    if (s.Count > 0 && ObjectsStorage.AllFunctionNames.Contains(s.Peek())) { q.Enqueue(s.Pop()); }
                }
                else
                {
                    throw new ArgumentException("Suspected error in expression.");
                }
            }
            while (s.Count > 0)
            {
                if (s.Peek() == "(")
                {
                    throw new ArgumentException("Mismatched parentheses.");
                }
                q.Enqueue(s.Pop());
            }
        }

        /// <summary>
        /// Вычисления на стеке.
        /// </summary>
        /// <param name="s">Стек.</param>
        /// <param name="q">Очередь.</param>
        /// <returns>Результат вычисления, число System.Double.</returns>
        internal static double Evaluate(Stack<string> s, Queue<string> q)
        {
            if (q.Count == 0) { throw new ArgumentException("Invalid expression."); }
            string tmp;
            while (q.Count() > 0)
            {
                tmp = q.Peek();
                if (tmp.IsNumber())
                {
                    if (ObjectsStorage.InternalConstants.ContainsKey(tmp))
                    {
                        s.Push(ObjectsStorage.InternalConstants[tmp].Value.ToString());
                        q.Dequeue();
                    }
                    else if (ObjectsStorage.CustomConstants.ContainsKey(tmp))
                    {
                        s.Push(ObjectsStorage.CustomConstants[tmp].Value.ToString());
                        q.Dequeue();
                    }
                    else
                    {
                        s.Push(q.Dequeue());
                    }
                }
                else if (ObjectsStorage.OperatorPriority.ContainsKey(tmp))
                {
                    if (tmp != "!" && tmp != "_df")
                    {
                        double operand2 = double.Parse(s.Pop(), NumberStyles.Float);
                        double operand1 = double.Parse(s.Pop(), NumberStyles.Float);
                        s.Push(ObjectsStorage.ArithmeticActionsBinary[tmp](operand1, operand2));
                        q.Dequeue();
                    }
                    else
                    {
                        double operand = double.Parse(s.Pop(), NumberStyles.Float);
                        s.Push(ObjectsStorage.ArithmeticActionsUnary[tmp](operand));
                        q.Dequeue();
                    }
                }
                else if (ObjectsStorage.AllFunctionNames.Contains(tmp))
                {
                    Function sourceFuncRef = null;
                    if (ObjectsStorage.InternalFunctions.ContainsKey(tmp))
                    {
                        sourceFuncRef = ObjectsStorage.InternalFunctions[tmp];
                    }
                    else
                    {
                        sourceFuncRef = ObjectsStorage.CustomFunctions[tmp];
                    }
                    double[] operandArray = new double[sourceFuncRef.NumberOfArguments];
                    switch (sourceFuncRef.NumberOfArguments)
                    {
                        case 0: break;
                        case 1: operandArray[0] = double.Parse(s.Pop(), NumberStyles.Float); break;
                        case 2:
                            operandArray[1] = double.Parse(s.Pop(), NumberStyles.Float);
                            operandArray[0] = double.Parse(s.Pop(), NumberStyles.Float);
                            break;
                        case 3:
                            operandArray[2] = double.Parse(s.Pop(), NumberStyles.Float);
                            operandArray[1] = double.Parse(s.Pop(), NumberStyles.Float);
                            operandArray[0] = double.Parse(s.Pop(), NumberStyles.Float);
                            break;
                        default:
                            for (int i = sourceFuncRef.NumberOfArguments - 1; i >= 0; i--)
                            {
                                operandArray[i] = double.Parse(s.Pop(), NumberStyles.Float);
                            }
                            break;
                    }
                    s.Push(sourceFuncRef[operandArray]);
                    q.Dequeue();
                }
            }
            if (s.Count != 1) { throw new ArgumentException("An error occurred."); }
            double res = double.Parse(s.Pop(), NumberStyles.Float);
            return res;
        }

        /// <summary>
        /// Вычисление математического выражения, записанного в строке System.String. Метод является потокобезопасным.
        /// </summary>
        /// <param name="expression">Математическое выражение.</param>
        /// <param name="result">Строка System.String, содержащая результат вычислений. Это либо число System.Double, переведенное в строковое представление в соответствии с правилами форматирования типа System.Double (строка форматирования G, "общий" формат) и установленной точностью; либо сообщение об ошибке. Возвращается из метода по ссылке.</param>
        /// <param name="success">Переменная типа System.Boolean, принимающая значение True только при успешном вычислении. Возвращается из метода по ссылке.</param>
        public static void Evaluate(string expression, out string result, out bool success)
        {
            Monitor.Enter(locker);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            result = string.Empty;
            success = true;
            Stack<string> s = new Stack<string>(expression.Length);
            Queue<string> q = new Queue<string>(expression.Length);
            try
            {
                Write(Tokenize(expression), s, q);
                double res = Evaluate(s, q);
                res = Math.Round(res, decimals);
                result = string.Format("{0" + formatString, res).Replace("E+000", string.Empty);
            }
            catch (ArgumentNullException)
            {
                success = false;
                result = "Invalid token list.";
            }
            catch (Exception e)
            {
                success = false;
                result = e.Message;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
                Monitor.Exit(locker);
            }
        }

        /// <summary>
        /// Асинхронное вычисление математического выражения, записанного в строке System.String. Метод является потокобезопасным.
        /// </summary>
        /// <param name="expression">Математическое выражение.</param>
        /// <returns>Кортеж, содержащий значение System.Boolean, которое принимает значение True только при успешном вычислении выражения, а также строку System.String, содержащую результат вычислений.</returns>
        /// <example>
        /// </example>
        public static Task<Tuple<bool, string>> EvaluateAsync(string expression)
        {
            return Task.Run(() =>
            {
                Monitor.Enter(locker);
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Stack<string> s = new Stack<string>(expression.Length);
                Queue<string> q = new Queue<string>(expression.Length);
                try
                {
                    Write(Tokenize(expression), s, q);
                    double res = Evaluate(s, q);
                    res = Math.Round(res, decimals);
                    return Tuple.Create(true, string.Format("{0" + formatString, res).Replace("E+000", string.Empty));
                }
                catch (ArgumentNullException)
                {
                    return Tuple.Create(false, "Invalid token list.");
                }
                catch (Exception e)
                {
                    return Tuple.Create(false, e.Message);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
                    Monitor.Exit(locker);
                }
            });
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
        /// Получение последовательности встроенных функций.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetListOfInternalFunctions()
        {
            foreach (var f in ObjectsStorage.InternalFunctions.Values)
            {
                yield return f.ToString();
            }
        }

        /// <summary>
        /// Получение последовательности пользовательских функций.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetListOfCustomFunctions()
        {
            foreach (var f in ObjectsStorage.CustomFunctions.Values)
            {
                yield return f.ToString();
            }
        }

        /// <summary>
        /// Получение последовательности встроенных констант.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetListOfCustomConstants()
        {
            foreach (var c in ObjectsStorage.CustomConstants.Values)
            {
                yield return c.ToString();
            }
        }

        /// <summary>
        /// Получение последовательности пользовательских констант.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetListOfInternalConstants()
        {
            foreach (var c in ObjectsStorage.InternalConstants.Values)
            {
                yield return c.ToString();
            }
        }

        #endregion

        /// <summary>
        /// Разбиение строки выражения на список токенов.
        /// </summary>
        /// <param name="primeExpression">Строка, содержащая в себе выражение.</param>
        /// <returns>Список токенов.</returns>
        unsafe internal static List<string> Tokenize(string primeExpression)
        {
            TokenListPrimaryCheckingAndInterpreting(ref primeExpression);
            int len = primeExpression.Length;
            List<string> res = new List<string>(len);
            int i = 0;
            fixed (char* pointer = primeExpression)
            {
                char* s = pointer;
                char cp1, cm1;
                for (; i < len; i++, s++)
                {
                    cp1 = *(s + 1);
                    cm1 = *(s - 1);
                    if ((*s >= 40 && *s <= 43) || *s == '-' || *s == '%' || *s == '!' || *s == '/' || *s == '^' || *s == ',') { res.Add(new string(*s, 1)); continue; }
                    else if (char.IsDigit(*s))
                    {
                        res.Add(string.Empty);
                        do
                        {
                            res[res.Count - 1] += *s;
                            i++;
                            s++;
                            if (i >= len) { break; }
                        } while ((*s >= 48 && *s <= 57) || *s == '.');
                        i--;
                        s--;
                        continue;
                    }
                    else if ((*s >= 65 && *s <= 90) || (*s >= 97 && *s <= 122) || (*s == '_'))
                    {
                        res.Add(string.Empty);
                        do
                        {
                            res[res.Count - 1] += *s;
                            i++;
                            s++;
                            if (i >= len) { break; }
                        } while ((*s >= 48 && *s <= 57) || (*s >= 65 && *s <= 90) || (*s >= 97 && *s <= 122) || (*s == '_'));
                        i--;
                        s--;
                        continue;
                    }
                    else { throw new ArgumentException("Syntactic analysis has been failed, check your expression."); }
                }
            }
            return res;
        }

        /// <summary>
        /// Первичная проверка и обработка выражения.
        /// </summary>
        /// <param name="primeExpression">Математическое выражение.</param>
        unsafe static void TokenListPrimaryCheckingAndInterpreting(ref string primeExpression)
        {
            primeExpression = primeExpression.Replace(" ", string.Empty)
                .Replace("√", "sqrt")
                .Replace("×", "*")
                .Replace("π", "pi")
                .Replace("γ", "gamma")
                .Replace("φ", "phi")
                .ToLowerInvariant();
            primeExpression = Regex.Replace(primeExpression, @"(\d|[)])mod(\d|[(])", "${1}%${2}");
            primeExpression = Regex.Replace(primeExpression, @"([(])([+-])(\d)", "${1}0${2}${3}");
            primeExpression = Regex.Replace(primeExpression, @"([)])(\d)", "${1}*${2}");
            primeExpression = Regex.Replace(primeExpression, @"([\d|)])([\p{L}|(])", "${1}*${2}");
            if (primeExpression[0] == '-')
            {
                primeExpression = primeExpression.Insert(0, "0");
            }
            else if (primeExpression[0] == '+')
            {
                primeExpression = primeExpression.Remove(0, 1);
            }
            primeExpression = primeExpression.Replace("!!", "_df");
            if (primeExpression.Contains("_df!") || primeExpression.Contains("!_df") || primeExpression.Contains("_df_df"))
            {
                int errorIndex = 0;
                if (primeExpression.Contains("_df!"))
                {
                    errorIndex = primeExpression.IndexOf("_df!");
                }
                else if (primeExpression.Contains("!_df"))
                {
                    errorIndex = primeExpression.IndexOf("!_df");
                }
                else
                {
                    errorIndex = primeExpression.IndexOf("_df_df");
                }
                throw new ArgumentException($"Invalid token near {errorIndex + 1} symbol: factorial must be \'!\' or \'!!\'.");
            }
            int i = 0;
            int len = primeExpression.Length;
            fixed (char* pointer = primeExpression)
            {
                char* s = pointer;
                char c, cp1, cm1;
                for (; i < len; i++, s++)
                {
                    c = *s;
                    cp1 = *(s + 1);
                    cm1 = *(s - 1);
                    if (((c >= 43 && c <= 47) || (c >= 33 && c <= 39)) && ((cp1 >= 43 && cp1 <= 47) || (c >= 33 && c <= 39)))
                    {
                        throw new ArgumentException($"Invalid token near {i + 1} symbol: it is not allowed to repeat operators and separators.");
                    }
                }
            }
        }

        /// <summary>
        /// Финальная проверка списка токенов.
        /// </summary>
        /// <param name="l">Список токенов для проверки.</param>
        [Obsolete("Contains only inserting in decimal-letter and bracket-letter a multiplication token.")]
        static void TokenListFinalChecking(List<string> l)
        {
            for (int i = 0, iPlusOne = 1; i < l.Count; i++, iPlusOne++)
            {
                if (iPlusOne < l.Count)
                {
                    if ((l[i].IsNumber() && (l[iPlusOne] == "(" || ObjectsStorage.AllFunctionNames.Contains(l[iPlusOne])))
                     || (l[i] == ")" && l[iPlusOne].IsNumber()))
                    {
                        l.Insert(i + 1, "*");
                        i++;
                        iPlusOne++;
                        continue;
                    }
                }
            }
        }
    }
}

