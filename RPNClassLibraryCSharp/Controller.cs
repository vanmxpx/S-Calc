using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Threading.Tasks;

namespace RPNClassLibraryCSharp
{
    public static class Controller
    {
        /// <summary>
        /// Количество точек после запятой.
        /// </summary>
        static byte decimals;

        /// <summary>
        /// Словарь, где каждому математическому оператору поставлен в соответствие приоритет в виде числа System.Byte.
        /// </summary>
        static Dictionary<string, byte> operatorPriority;

        /// <summary>
        /// Хэш-таблица допустимых математических операторов.
        /// </summary>
        static HashSet<string> operatorsSet;

        /// <summary>
        /// Словарь унарных математических функций (название-делегат).
        /// </summary>
        static Dictionary<string, Func<double, string>> unaryFunctions;

        /// <summary>
        /// Словарь унарных математических операторов (название-делегат).
        /// </summary>
        static Dictionary<string, Func<double, string>> arithmeticActionsUnary;

        /// <summary>
        /// Словарь бинарных математических функций (название-делегат).
        /// </summary>
        static Dictionary<string, Func<double, double, string>> binaryFunctions;

        /// <summary>
        /// Словарь бинарных математических операторов (название-делегат).
        /// </summary>
        static Dictionary<string, Func<double, double, string>> arithmeticActionsBinary;

        /// <summary>
        /// Строка форматирования.
        /// </summary>
        const string formatString = ":G}";

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static Controller()
        {
            decimals = 4;
            locker = new object();
            operatorPriority = new Dictionary<string, byte>();
            operatorPriority.Add("+", 1);
            operatorPriority.Add("-", 1);
            operatorPriority.Add("*", 2);
            operatorPriority.Add("/", 2);
            operatorPriority.Add("%", 2);
            operatorPriority.Add("!", 3);
            operatorPriority.Add("!!", 3);
            operatorPriority.Add("^", 3);
            arithmeticActionsUnary = new Dictionary<string, Func<double, string>>();
            arithmeticActionsUnary.Add("!", (operand) => Functions.Factorial((int)operand).ToString());
            arithmeticActionsUnary.Add("!!", (operand) => Functions.DoubleFactorial((int)operand).ToString());
            arithmeticActionsBinary = new Dictionary<string, Func<double, double, string>>();
            arithmeticActionsBinary.Add("+", (operand1, operand2) => (operand1 + operand2).ToString());
            arithmeticActionsBinary.Add("-", (operand1, operand2) => (operand1 - operand2).ToString());
            arithmeticActionsBinary.Add("*", (operand1, operand2) => (operand1 * operand2).ToString());
            arithmeticActionsBinary.Add("/", (operand1, operand2) => (operand1 / operand2).ToString());
            arithmeticActionsBinary.Add("%", (operand1, operand2) => (operand1 % operand2).ToString());
            arithmeticActionsBinary.Add("^", (operand1, operand2) => (Math.Pow(operand1, operand2)).ToString());
            operatorsSet = new HashSet<string>();
            operatorsSet.Add("+");
            operatorsSet.Add("-");
            operatorsSet.Add("/");
            operatorsSet.Add("*");
            operatorsSet.Add("^");
            operatorsSet.Add("!");
            operatorsSet.Add("!!");
            operatorsSet.Add("%");
            unaryFunctions = new Dictionary<string, Func<double, string>>();
            unaryFunctions.Add("abs", (operand) => (Math.Abs(operand)).ToString());
            unaryFunctions.Add("sgn", (operand) => (Math.Sign(operand)).ToString());
            unaryFunctions.Add("sqrt", (operand) => (Math.Sqrt(operand)).ToString());
            unaryFunctions.Add("sin", (operand) => (Math.Sin(operand)).ToString());
            unaryFunctions.Add("cos", (operand) => (Math.Cos(operand)).ToString());
            unaryFunctions.Add("tg", (operand) => (Math.Tan(operand)).ToString());
            unaryFunctions.Add("ctg", (operand) => (1.0 / Math.Tan(operand)).ToString());
            unaryFunctions.Add("arcsin", (operand) => (Math.Asin(operand)).ToString());
            unaryFunctions.Add("arccos", (operand) => (Math.Acos(operand)).ToString());
            unaryFunctions.Add("arctg", (operand) => (Math.Atan(operand)).ToString());
            unaryFunctions.Add("arcctg", (operand) => (Math.PI / 2.0 - Math.Atan(operand)).ToString());
            unaryFunctions.Add("sec", (operand) => (1.0 / Math.Cos(operand)).ToString());
            unaryFunctions.Add("cosec", (operand) => (1.0 / Math.Sin(operand)).ToString());
            unaryFunctions.Add("arcsec", (operand) => (Math.Acos(1.0 / operand)).ToString());
            unaryFunctions.Add("arccosec", (operand) => (Math.Asin(1.0 / operand)).ToString());
            unaryFunctions.Add("ln", (operand) => (Math.Log(operand)).ToString());
            unaryFunctions.Add("lg", (operand) => (Math.Log10(operand)).ToString());
            unaryFunctions.Add("exp", (operand) => (Math.Exp(operand)).ToString());
            unaryFunctions.Add("sh", (operand) => (Math.Sinh(operand)).ToString());
            unaryFunctions.Add("ch", (operand) => (Math.Cosh(operand)).ToString());
            unaryFunctions.Add("th", (operand) => (1.0 / Math.Sin(operand)).ToString());
            unaryFunctions.Add("cth", (operand) => (Math.Tanh(operand)).ToString());
            unaryFunctions.Add("sech", (operand) => (1.0 / Math.Cosh(operand)).ToString());
            unaryFunctions.Add("csch", (operand) => (1.0 / Math.Sinh(operand)).ToString());
            unaryFunctions.Add("arsh", (operand) => (Functions.Arsh(operand)).ToString());
            unaryFunctions.Add("arch", (operand) => (Functions.Arch(operand)).ToString());
            unaryFunctions.Add("arth", (operand) => (Functions.Arth(operand)).ToString());
            unaryFunctions.Add("arcth", (operand) => (Functions.Arcth(operand)).ToString());
            unaryFunctions.Add("arcsech", (operand) => (Functions.Arsech(operand)).ToString());
            unaryFunctions.Add("arccsch", (operand) => (Functions.Arcsch(operand)).ToString());
            binaryFunctions = new Dictionary<string, Func<double, double, string>>();
            binaryFunctions.Add("log", (operand1, operand2) => (Math.Log(operand2, operand1)).ToString());
            binaryFunctions.Add("pow", (operand1, operand2) => (Math.Pow(operand1, operand2)).ToString());
        }

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

        readonly static object locker;

        /// <summary>
        /// Запись выражения в постфиксную нотацию (обратную польскую нотацию).
        /// </summary>
        /// <param name="l">Список токенов.</param>
        /// <param name="s">Стек.</param>
        /// <param name="q">Очередь.</param>
        static void Write(List<string> l, Stack<string> s, Queue<string> q)
        {
            foreach (string str in l)
            {
                if (str.IsNumber()) { q.Enqueue(str); }
                else if (unaryFunctions.ContainsKey(str) || binaryFunctions.ContainsKey(str)) { s.Push(str); }
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
                else if (str != "!" && operatorsSet.Contains(str))
                {
                    while (s.Count > 0 && operatorsSet.Contains(s.Peek()) && operatorPriority[str] <= operatorPriority[s.Peek()])
                    {
                        q.Enqueue(s.Pop());
                    }
                    s.Push(str);
                }
                else if (str == "!" && operatorsSet.Contains(str))
                {
                    while (s.Count > 0 && operatorsSet.Contains(s.Peek()) && operatorPriority[str] < operatorPriority[s.Peek()])
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
                    if (s.Count > 0 && (unaryFunctions.ContainsKey(s.Peek()) || binaryFunctions.ContainsKey(s.Peek()))) { q.Enqueue(s.Pop()); }
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
        static double Evaluate(Stack<string> s, Queue<string> q)
        {
            if (q.Count == 0) { throw new ArgumentException("Invalid expression."); }
            string tmp;
            while (q.Count() > 0)
            {
                tmp = q.Peek();
                if (tmp.IsNumber())
                {
                    switch (tmp)
                    {
                        case "pi": s.Push(Math.PI.ToString()); q.Dequeue(); break;
                        case "e": s.Push(Math.E.ToString()); q.Dequeue(); break;
                        case "gamma": s.Push("0.577215664901533"); q.Dequeue(); break;
                        case "phi": s.Push("1.61803398874989"); q.Dequeue(); break;
                        default: s.Push(q.Dequeue()); break;
                    }
                }
                else if (operatorsSet.Contains(tmp))
                {
                    if (tmp != "!" && tmp != "!!")
                    {
                        double operand2 = double.Parse(s.Pop(), NumberStyles.Float);
                        double operand1 = double.Parse(s.Pop(), NumberStyles.Float);
                        s.Push(arithmeticActionsBinary[tmp](operand1, operand2));
                        q.Dequeue();
                    }
                    else
                    {
                        double operand = double.Parse(s.Pop(), NumberStyles.Float);
                        s.Push(arithmeticActionsUnary[tmp](operand));
                        q.Dequeue();
                    }
                }
                else if (unaryFunctions.ContainsKey(tmp))
                {
                    double operand = double.Parse(s.Pop(), NumberStyles.Float);
                    s.Push(unaryFunctions[tmp](operand));
                    q.Dequeue();
                }
                else if (binaryFunctions.ContainsKey(tmp))
                {
                    double operand2 = double.Parse(s.Pop(), NumberStyles.Float);
                    double operand1 = double.Parse(s.Pop(), NumberStyles.Float);
                    s.Push(binaryFunctions[tmp](operand1, operand2));
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

        /// <summary>
        /// Разбиение строки выражения на список токенов.
        /// </summary>
        /// <param name="primeExpression">Строка, содержащая в себе выражение.</param>
        /// <returns>Список токенов.</returns>
        unsafe static List<string> Tokenize(string primeExpression)
        {
            primeExpression = primeExpression.Replace(" ", string.Empty)
                .Replace("√", "sqrt")
                .Replace("×", "*")
                .Replace("π", "pi")
                .Replace("γ", "gamma")
                .Replace("φ", "phi")
                .ToLowerInvariant();
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
                    if ((*s >= 40 && *s <= 42) || *s == '%' || *s == '/' || *s == '^' || *s == ',')
                    {
                        res.Add(new string(*s, 1));
                        continue;
                    }
                    else if ((*s == '+' || *s == '-') && (i == 0 || cm1 == '('))
                    {
                        res.Add("0");
                        res.Add(new string(*s, 1));
                        continue;
                    }
                    else if (*s == '+' || *s == '-')
                    {
                        res.Add(new string(*s, 1));
                        continue;
                    }
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
                    else if (i != len - 1 && *s == '!' && cp1 != '!') { res.Add(new string(*s, 1)); continue; }
                    else if (i != len - 1 && *s == '!' && cp1 == '!') { res.Add("!!"); i++; s++; continue; }
                    else if (i == len - 1 && *s == '!') { res.Add(new string(*s, 1)); continue; }
                    else if ((*s >= 65 && *s <= 90) || (*s >= 97 && *s <= 122))
                    {
                        res.Add(string.Empty);
                        do
                        {
                            res[res.Count - 1] += *s;
                            i++;
                            s++;
                            if (i >= len) { break; }
                        } while ((*s >= 48 && *s <= 57) || (*s >= 65 && *s <= 90) || (*s >= 97 && *s <= 122));
                        i--;
                        s--;
                        continue;
                    }
                    else { return null; }
                    //throw new ArgumentException("Suspected error in expression.");
                }
            }
            TokenListChecking(res);
            return res;
        }

        /// <summary>
        /// Финальная проверка списка токенов.
        /// </summary>
        /// <param name="l">Список токенов для проверки.</param>
        static void TokenListChecking(List<string> l)
        {
            for (int i = 0, iPlusOne = 1; i < l.Count; i++, iPlusOne++)
            {
                if (iPlusOne < l.Count)
                {
                    if ((l[i].IsNumber() && (l[iPlusOne] == "(" || unaryFunctions.ContainsKey(l[iPlusOne]) || binaryFunctions.ContainsKey(l[iPlusOne])))
                     || (l[i] == ")" && l[iPlusOne].IsNumber()))
                    {
                        l.Insert(i + 1, "*");
                        i++;
                        continue;
                    }
                }
            }
        }

    }
}
