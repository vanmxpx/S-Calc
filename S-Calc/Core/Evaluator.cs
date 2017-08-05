using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace S_Calc.Core
{
    sealed class Evaluator
    {
        #region FIELDS

        byte decimals;

        Stack<Token> s;

        Queue<Token> q;

        internal HistoryBuffer<CalculationHistory> History;

        readonly object locker;

        readonly BinaryFormatter formatter;

        string HistoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "history.dat");

        #endregion

        public Evaluator(byte Decimals)
        {
            decimals = Decimals <= 15 ? Decimals : (byte)15;
            locker = new object();
            s = new Stack<Token>();
            q = new Queue<Token>();
            formatter = new BinaryFormatter();
            bool WasHistoryFileEmpty = false;
            using (FileStream fs = new FileStream(HistoryPath, FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                {
                    WasHistoryFileEmpty = true;
                }
                else
                {
                    History = (HistoryBuffer<CalculationHistory>)formatter.Deserialize(fs);
                }
            }
            if (WasHistoryFileEmpty)
            {
                History = new HistoryBuffer<CalculationHistory>();
                using (FileStream fs = new FileStream(HistoryPath, FileMode.Open))
                {
                    formatter.Serialize(fs, History);
                }
            }
        }

        public void SetDecimals(byte Decimals)
        {
            decimals = Decimals <= 15 ? Decimals : (byte)15;
        }

        public byte GetDecimals()
        {
            return decimals;
        }

        #region RPN_LOGIC

        void Write(List<Token> l)
        {
            foreach (Token str in l)
            {
                switch (str.Type)
                {
                    case TokenType.Number:
                        q.Enqueue(str);
                        break;
                    case TokenType.Function:
                        s.Push(str);
                        break;
                    case TokenType.Separator:
                        try
                        {
                            while (s.Peek().Type != TokenType.LeftBracket)
                            {
                                q.Enqueue(s.Pop());
                            }
                        }
                        catch
                        {
                            throw new Exception("The separator was misplaced or \"(\" were mismatched.");
                        }
                        break;
                    case TokenType.BinaryOperator:
                        while (s.Count > 0 && (s.Peek().Type == TokenType.BinaryOperator || s.Peek().Type == TokenType.UnaryOperator) && str.Value <= s.Peek().Value)
                        {
                            q.Enqueue(s.Pop());
                        }
                        s.Push(str);
                        break;
                    case TokenType.UnaryOperator:
                        while (s.Count > 0 && (s.Peek().Type == TokenType.BinaryOperator || s.Peek().Type == TokenType.UnaryOperator) && str.Value < s.Peek().Value)
                        {
                            q.Enqueue(s.Pop());
                        }
                        s.Push(str);
                        break;
                    case TokenType.LeftBracket:
                        s.Push(str);
                        break;
                    case TokenType.RightBracket:
                        try
                        {
                            while (s.Peek().Type != TokenType.LeftBracket)
                            {
                                q.Enqueue(s.Pop());
                            }
                        }
                        catch
                        {
                            throw new Exception("Expect \"(\".");
                        }
                        s.Pop();
                        if (s.Count > 0 && s.Peek().Type == TokenType.Function) { q.Enqueue(s.Pop()); }
                        break;
                    default:
                        throw new Exception($"Suspected error in expression near token number {str.Position}.");
                }
            }
            while (s.Count > 0)
            {
                if (s.Peek().Type == TokenType.LeftBracket)
                {
                    throw new Exception("Mismatched parentheses.");
                }
                q.Enqueue(s.Pop());
            }
        }

        double Evaluate()
        {
            if (q.Count == 0) { throw new Exception("Invalid expression."); }
            Token tmp;
            while (q.Count() > 0)
            {
                tmp = q.Peek();

                switch (tmp.Type)
                {
                    case TokenType.Number:
                        s.Push(new Token(q.Dequeue().Value.Value.ToString(CultureInfo.InvariantCulture), tmp.Position));
                        break;
                    case TokenType.BinaryOperator:
                        try
                        {
                            double operand2 = s.Pop().Value.Value;
                            double operand1 = s.Pop().Value.Value;
                            s.Push(new Token(ObjectsStorage.ArithmeticActionsBinary[tmp.Title](operand1, operand2), tmp.Position));
                            q.Dequeue();
                        }
                        catch
                        {
                            throw new Exception($"Unexpected token near operator (operator was token number {tmp.Position}). Both operands must be numbers.");
                        }
                        break;
                    case TokenType.UnaryOperator:
                        try
                        {
                            double operand = s.Pop().Value.Value;
                            s.Push(new Token(ObjectsStorage.ArithmeticActionsUnary[tmp.Title](operand), tmp.Position));
                            q.Dequeue();
                        }
                        catch
                        {
                            throw new Exception($"Unexpected token near factorial (factorial was token number {tmp.Position}). Operand must be number.");
                        }
                        break;
                    case TokenType.Function:
                        {
                            Function sourceFuncRef = null;
                            sourceFuncRef = tmp.Function;
                            double[] operandArray = new double[sourceFuncRef.NumberOfArguments];
                            for (int i = sourceFuncRef.NumberOfArguments - 1; i >= 0; i--)
                            {
                                try
                                {
                                    operandArray[i] = s.Pop().Value.Value;
                                }
                                catch
                                {
                                    throw new Exception($"Unexpected function argument (function was token number {tmp.Position}). Arguments must be numbers.");
                                }
                            }
                            s.Push(new Token(sourceFuncRef[operandArray], tmp.Position));
                            q.Dequeue();
                        }
                        break;
                    default:
                        throw new Exception($"Invalid token (token number {tmp.Position}).");
                }


            }
            if (s.Count != 1) { throw new Exception("An error occurred."); }
            if (s.Peek().Value.GetType() == typeof(double))
            {
                return s.Pop().Value.Value;
            }
            else
            {
                throw new Exception("An error occurred: answer wasn't number.");
            }
        }

        #endregion

        public void Evaluate(string expression, int CursorPosition, out string result, out bool success, bool SaveToHistory = true)
        {
            Monitor.Enter(locker);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            result = string.Empty;
            success = true;
            try
            {
                Write(Tokenize(expression));
                double res = Evaluate();
                res = Math.Round(res, decimals);
                result = string.Format("{0:G}", res).Replace("E+000", string.Empty);
            }
            catch (Exception e)
            {
                success = false;
                result = e.Message;
            }
            finally
            {
                s.Clear();
                q.Clear();
                if (SaveToHistory)
                {
                    History.ResetCursorPosition();
                    History.Push(new CalculationHistory(expression, result, CursorPosition));
                }
                Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
                Monitor.Exit(locker);
            }
        }

        public Task<Tuple<bool, string>> EvaluateAsync(string expression, int CursorPosition, bool SaveToHistory = true)
        {
            return Task.Run(() =>
            {
                Monitor.Enter(locker);
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Stack<Token> s = new Stack<Token>(expression.Length);
                Queue<Token> q = new Queue<Token>(expression.Length);
                try
                {
                    Write(Tokenize(expression));
                    double res = Evaluate();
                    res = Math.Round(res, decimals);
                    var tuple = Tuple.Create(true, string.Format("{0:G}", res).Replace("E+000", string.Empty));
                    if (SaveToHistory)
                    {
                        History.Push(new CalculationHistory(expression, tuple.Item2, CursorPosition));
                    }
                    return tuple;
                }
                catch (Exception e)
                {
                    var tuple = Tuple.Create(false, e.Message);
                    if (SaveToHistory)
                    {
                        History.ResetCursorPosition();
                        History.Push(new CalculationHistory(expression, tuple.Item2, CursorPosition));
                    }
                    return Tuple.Create(false, e.Message);
                }
                finally
                {
                    s.Clear();
                    q.Clear();
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
                    Monitor.Exit(locker);
                }
            });
        }

        #region TOKENIZER

        /// <summary>
        /// Разбиение строки выражения на список токенов.
        /// </summary>
        /// <param name="primeExpression">Строка, содержащая в себе выражение.</param>
        /// <returns>Список токенов.</returns>
        unsafe List<Token> Tokenize(string primeExpression)
        {
            TokenListPrimaryCheckingAndInterpreting(ref primeExpression);
            int len = primeExpression.Length;
            List<Token> res = new List<Token>(len);
            int i = 0;
            string tmp = string.Empty;
            fixed (char* pointer = primeExpression)
            {
                char* s = pointer;
                char cp1, cm1;
                for (; i < len; i++, s++)
                {
                    cp1 = *(s + 1);
                    cm1 = *(s - 1);
                    if ((*s >= 40 && *s <= 47) || *s == '%' || *s == '!' || *s == '^') { res.Add(new Token(new string(*s, 1), res.Count + 1)); continue; }
                    else if (char.IsDigit(*s))
                    {
                        tmp = string.Empty;
                        do
                        {
                            tmp += *s;
                            i++;
                            s++;
                            if (i >= len) { break; }
                        } while ((*s >= 48 && *s <= 57) || *s == '.');
                        //Для экспоненциальной записи
                        cp1 = *(s + 1);
                        if (*s == 'E' && (cp1 == '+' || cp1 == '-'))
                        {
                            tmp += *s;
                            tmp += cp1;
                            i += 2;
                            s += 2;
                            do
                            {
                                tmp += *s;
                                i++;
                                s++;
                                if (i >= len) { break; }
                            } while (*s >= 48 && *s <= 57);
                        }
                        i--;
                        s--;
                        res.Add(new Token(tmp, res.Count + 1));
                        continue;
                    }
                    else if ((*s >= 65 && *s <= 90) || (*s >= 97 && *s <= 122) || (*s == '_'))
                    {
                        tmp = string.Empty;
                        do
                        {
                            tmp += *s;
                            i++;
                            s++;
                            if (i >= len) { break; }
                        } while ((*s >= 48 && *s <= 57) || (*s >= 65 && *s <= 90) || (*s >= 97 && *s <= 122) || (*s == '_'));
                        i--;
                        s--;
                        res.Add(new Token(tmp, res.Count + 1));
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
        unsafe void TokenListPrimaryCheckingAndInterpreting(ref string primeExpression)
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
            primeExpression = Regex.Replace(primeExpression, @"([\d])([\p{L}])", "${1}*${2}");
            primeExpression = Regex.Replace(primeExpression, @"([)])([\p{L}|(])", "${1}*${2}");
            primeExpression = Regex.Replace(primeExpression, @"(\d)\*e([+-])(\d)", "${1}E${2}${3}");
            if (primeExpression.Length == 0)
            {
                throw new ArgumentException($"Expression is empty.");
            }
            if (primeExpression[0] == '-')
            {
                primeExpression = primeExpression.Insert(0, "0");
            }
            else if (primeExpression[0] == '+')
            {
                primeExpression = primeExpression.Remove(0, 1);
            }
            primeExpression = primeExpression.Replace("!!", ObjectsStorage.DoubleFactorialName);
            if (
                primeExpression.Contains(ObjectsStorage.DoubleFactorialNameRightFactorial) || 
                primeExpression.Contains(ObjectsStorage.DoubleFactorialNameLeftFactorial) || 
                primeExpression.Contains(ObjectsStorage.TwoDoubleFactorials))
            {
                int errorIndex = 0;
                if (primeExpression.Contains(ObjectsStorage.DoubleFactorialNameRightFactorial))
                {
                    errorIndex = primeExpression.IndexOf(ObjectsStorage.DoubleFactorialNameRightFactorial);
                }
                else if (primeExpression.Contains(ObjectsStorage.DoubleFactorialNameLeftFactorial))
                {
                    errorIndex = primeExpression.IndexOf(ObjectsStorage.DoubleFactorialNameLeftFactorial);
                }
                else
                {
                    errorIndex = primeExpression.IndexOf(ObjectsStorage.TwoDoubleFactorials);
                }
                throw new ArgumentException($"Invalid symbol near {errorIndex + 1} symbol: factorial must be \'!\' or \'!!\'.");
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
                    if (((c >= 43 && c <= 47) || (c >= 34 && c <= 39) || c == 94) && ((cp1 >= 43 && cp1 <= 47) || (cp1 >= 33 && cp1 <= 39) || cp1 == 94))
                    {
                        if (!(c == 94 && (cp1 == 43 || cp1 == 45)))
                        {
                            throw new ArgumentException($"Invalid symbol near {i + 1} symbol: it is not allowed to repeat operators and separators.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Финальная проверка списка токенов.
        /// </summary>
        /// <param name="l">Список токенов для проверки.</param>
        [Obsolete("Contains only inserting in decimal-letter and bracket-letter a multiplication token.")]
        void TokenListFinalChecking(List<string> l)
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

        #endregion

        ~Evaluator()
        {
            s = null;
            q = null;
            using (FileStream fs = new FileStream(HistoryPath, FileMode.Open))
            {
                formatter.Serialize(fs, History);
            }
            ObjectsStorage.SaveAllChanges();
        }
    }
}
