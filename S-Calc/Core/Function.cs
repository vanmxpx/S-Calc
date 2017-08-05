using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    /// <summary>
    /// Тип функции
    /// </summary>
    [Serializable]
    public enum FunctionType
    {
        /// <summary>
        /// Тригонометрическая функция
        /// </summary>
        Trigonometric,
        /// <summary>
        /// Гиперболическая функция
        /// </summary>
        Hyperbolic,
        /// <summary>
        /// Общая функция
        /// </summary>
        Common,
        /// <summary>
        /// Специальная функция
        /// </summary>
        Special,
        /// <summary>
        /// Настраиваемая пользовательская функция
        /// </summary>
        Custom
    }

    [Serializable]
    public delegate string FunctionDelegate(params double[] args);

    [Serializable]
    public sealed class Function : IEquatable<Function>, IComparable<Function>, ICalculatorUnit
    {
        public string this[params double[] args]
        {
            get
            {
                if (args != null && args.Length == NumberOfArguments)
                {
                    rating++;
                    if (rating.Equals(UInt32.MaxValue))
                    {
                        ObjectsStorage.ResetFunctionsRating();
                    }
                    return evaluator(args);
                }
                else
                {
                    throw new ArgumentException("Invalid arguments array.");
                }
            }
        }

        public readonly FunctionType Type;

        UInt32 rating;

        readonly bool removable;

        public bool IsRemovable => removable;

        public UInt32 Rating => rating;

        readonly string functionName;

        public string Name => functionName;

        public readonly string FunctionText;

        public readonly ushort NumberOfArguments;

        public readonly string[] ArgumentNames;

        readonly string description;

        public string Description => description;

        internal readonly FunctionDelegate evaluator;

        readonly Queue<Token> RPNQueue;

        readonly Stack<Token> ServiceStack;

        readonly object locker = new object();

        public void ResetRating()
        {
            rating = 0;
        }

        public Function(string functionName, ushort numberOfArguments, string description, FunctionType type, FunctionDelegate _delegate)
        {
            functionName = functionName.ToLowerInvariant();
            evaluator = _delegate;
            NumberOfArguments = numberOfArguments;
            Type = type;
            rating = 0;
            removable = false;
            FunctionText = null;
            ArgumentNames = null;
            RPNQueue = null;
            ServiceStack = null;
            this.functionName = string.Copy(functionName);
            this.description = string.Copy(description);
        }

        public Function(string functionName, string functionText, string description, FunctionType type, params string[] arguments)
        {
            if (string.IsNullOrEmpty(functionName) || char.IsDigit(functionName[0]))
            {
                throw new ArgumentException("Wrong function name.");
            }
            else
            {
                functionName = functionName.ToLowerInvariant();
                if (ObjectsStorage.AllFunctionNames.Contains(functionName) || ObjectsStorage.AllConstantNames.Contains(functionName))
                {
                    throw new ArgumentException("Function or constant with the same name is already exists.");
                }
            }
            if (string.IsNullOrEmpty(functionText))
            {
                throw new ArgumentException("Function text does not exist.");
            }
            else
            {
                functionText = functionText.ToLowerInvariant();
            }
            if (arguments != null)
            {
                arguments = arguments.Select(s => s.ToLowerInvariant()).ToArray();
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (string.IsNullOrEmpty(arguments[i]) || char.IsDigit(arguments[i][0]))
                    {
                        throw new ArgumentException("Wrong argument.");
                    }
                    else if (arguments[i].Equals(functionName) || ObjectsStorage.AllFunctionNames.Contains(arguments[i]) || ObjectsStorage.AllConstantNames.Contains(arguments[i]))
                    {
                        throw new ArgumentException("Argument name can't be equals to any function name or constant name.");
                    }
                    for (int j = 0; j < arguments.Length; j++)
                    {
                        if (i != j && (arguments[i]== arguments[j]))
                        {
                            throw new ArgumentException("Argument names can't contains duplicates.");
                        }
                    }
                }
                NumberOfArguments = (ushort)arguments.Length;
                ArgumentNames = new string[NumberOfArguments];
                Array.Copy(arguments, ArgumentNames, NumberOfArguments);
            }
            else
            {
                NumberOfArguments = 0;
                ArgumentNames = new string[0] { };
            }
            try
            {
                Monitor.Enter(locker);
                ServiceStack = new Stack<Token>();
                RPNQueue = new Queue<Token>();
                Write(Tokenize(functionText), ServiceStack, RPNQueue);
                evaluator += Evaluate;
            }
            catch (Exception e)
            {
                throw new Exception("Custom function creating was not completed. An error occurred. Text: " + e.Message);
            }
            finally
            {
                Monitor.Exit(locker);
            }
            FunctionText = String.Copy(functionText);
            this.functionName = String.Copy(functionName);
            Type = type;
            rating = 0;
            removable = true;
            if (string.IsNullOrEmpty(description))
            {
                this.description = string.Empty;
            }
            else
            {
                this.description = string.Copy(description);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Function ? Equals((Function)obj) : false;
        }

        public bool Equals(Function other)
        {
            return String.Compare(functionName, other.functionName, true, CultureInfo.InvariantCulture)== 0;
        }

        public override int GetHashCode()
        {
            return functionName.GetHashCode();
        }

        public int CompareTo(Function other)
        {
            return rating.CompareTo(other.rating);
            //return String.Compare(functionName, other.functionName, true, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            if (FunctionText == null)
            {
                switch (NumberOfArguments)
                {
                    case 0: return string.Format("{0}()", functionName);
                    case 1: return string.Format("{0}(x)", functionName);
                    case 2: return string.Format("{0}(x,y)", functionName);
                    case 3: return string.Format("{0}(x,y,z)", functionName);
                    default: return string.Format("{0}(\"{1} arguments\")", functionName, NumberOfArguments);
                }
            }
            else
            {
                if (NumberOfArguments == 0)
                {
                    return string.Format("{0}() => {1}", Name, FunctionText);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in ArgumentNames)
                    {
                        sb.Append(s);
                        sb.Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    return string.Format("{0}({1}) => {2}", functionName, sb.ToString(), FunctionText);
                }
            }
        }

        void Write(List<Token> l, Stack<Token> s, Queue<Token> q)
        {
            foreach (Token str in l)
            {
                switch (str.Type)
                {
                    case TokenType.Number:
                        q.Enqueue(str);
                        break;
                    case TokenType.Variable:
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

        string Evaluate(params double[] args)
        {
            Monitor.Enter(locker);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                Queue<Token> tmp = new Queue<Token>();
                foreach (Token s in RPNQueue)
                {
                    Token tmps = s;
                    if (tmps.Type == TokenType.Variable && ArgumentNames.Contains(tmps.Text))
                    {
                        tmps = new Token(args[ArgumentNames.FirstIndexOf(tmps.Text)].ToString(), tmps.Position);
                    }
                    tmp.Enqueue(tmps);
                }
                return Evaluate(ServiceStack, tmp).ToString();
            }
            finally
            {
                Monitor.Exit(locker);
            }
        }

        double Evaluate(Stack<Token> s, Queue<Token> q)
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
                        res.Add(new Token(tmp, res.Count + 1, ArgumentNames));
                        continue;
                    }
                    else { throw new ArgumentException("Syntactic analysis has been failed, check your expression."); }
                }

            }
            return res;
        }

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

    }
}
