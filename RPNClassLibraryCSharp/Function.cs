using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPNClassLibraryCSharp
{
    [Serializable]
    delegate string FunctionDelegate(params double[] args);

    [Serializable]
    sealed class Function : IEquatable<Function>, IComparable<Function>
    {
        public string this[params double[] args]
        {
            get
            {
                if (args != null && args.Length == NumberOfArguments)
                {
                    return evaluator(args);
                }
                else
                {
                    throw new ArgumentException("Invalid arguments array.");
                }
            }
        }

        public readonly string FunctionName;

        public readonly string FunctionText;

        public readonly ushort NumberOfArguments;

        public readonly string[] ArgumentNames;

        public readonly string Description;

        readonly FunctionDelegate evaluator;

        readonly Queue<string> RPNQueue;

        readonly Stack<string> ServiceStack;

        readonly object locker = new object();

        internal Function(string functionName, ushort numberOfArguments, string description, FunctionDelegate _delegate)
        {
            functionName = functionName.ToLowerInvariant();
            evaluator = _delegate;
            NumberOfArguments = numberOfArguments;
            FunctionText = null;
            ArgumentNames = null;
            RPNQueue = null;
            ServiceStack = null;
            FunctionName = functionName;
            Description = String.Copy(description);
        }

        public Function(string functionName, string functionText, string description, params string[] arguments)
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
                    for (int j = 0; j < arguments.Length; j++)
                    {
                        if (i != j && (arguments[i]== arguments[j]))
                        {
                            throw new ArgumentException("Argument names can't contains duplicates.");
                        }
                    }
                }
                foreach (string s in arguments)
                {
                    if (string.IsNullOrEmpty(s) || char.IsDigit(s[0]))
                    {
                        throw new ArgumentException("Wrong argument.");
                    }
                    else if (s.Equals(functionName) || ObjectsStorage.AllFunctionNames.Contains(s) || ObjectsStorage.AllConstantNames.Contains(s))
                    {
                        throw new ArgumentException("Argument name can't be equals to any function name or constant name.");
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
                ServiceStack = new Stack<string>();
                RPNQueue = new Queue<string>();
                Write(Controller.Tokenize(functionText), ServiceStack, RPNQueue);
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
            FunctionName = functionName;
            if (string.IsNullOrEmpty(description))
            {
                this.Description = string.Empty;
            }
            else
            {
                Description = String.Copy(description);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Function ? Equals((Function)obj) : false;
        }

        public bool Equals(Function other)
        {
            return String.Compare(FunctionName, other.FunctionName, true, CultureInfo.InvariantCulture)== 0;
        }

        public override int GetHashCode()
        {
            return FunctionName.GetHashCode();
        }

        public int CompareTo(Function other)
        {
            return String.Compare(FunctionName, other.FunctionName, true, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            if (FunctionText == null)
            {
                switch (NumberOfArguments)
                {
                    case 0: return string.Format("{0}()", FunctionName);
                    case 1: return string.Format("{0}(\"1 argument\")", FunctionName);
                    default: return string.Format("{0}(\"{1} arguments\")", FunctionName, NumberOfArguments);
                }
            }
            else
            {
                if (NumberOfArguments == 0)
                {
                    return string.Format("{0}() => {1}", FunctionName, FunctionText);
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
                    return string.Format("{0}({1}) => {2}", FunctionName, sb.ToString(), FunctionText);
                }
            }
        }

        void Write(List<string> l, Stack<string> s, Queue<string> q)
        {
            foreach (string str in l)
            {
                if (str.IsNumber() || ArgumentNames.Contains(str)) { q.Enqueue(str); }
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

        string Evaluate(params double[] args)
        {
            Monitor.Enter(locker);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                Queue<string> tmp = new Queue<string>();
                foreach (string s in RPNQueue)
                {
                    string tmps = s;
                    if (ArgumentNames.Contains(tmps))
                    {
                        tmps = args[ArgumentNames.FirstIndexOf(tmps)].ToString();
                    }
                    tmp.Enqueue(tmps);
                }
                return Controller.Evaluate(ServiceStack, tmp).ToString();
            }
            finally
            {
                Monitor.Exit(locker);
            }
        }

    }
}
