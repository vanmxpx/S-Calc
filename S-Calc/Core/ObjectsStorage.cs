using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace S_Calc.Core
{
    static class ObjectsStorage
    {
        public const string DoubleFactorialName = "_df";

        public const string DoubleFactorialNameRightFactorial = DoubleFactorialName + "!";

        public const string DoubleFactorialNameLeftFactorial = "!" + DoubleFactorialName;

        public const string TwoDoubleFactorials = DoubleFactorialName + DoubleFactorialName;

        public static Dictionary<string, byte> OperatorPriority;

        public static Dictionary<string, Func<double, string>> ArithmeticActionsUnary;

        public static Dictionary<string, Func<double, double, string>> ArithmeticActionsBinary;

        public static Dictionary<string, Constant> Constants;

        public static Dictionary<string, Function> Functions;

        public static HashSet<string> AllFunctionNames;

        public static HashSet<string> AllConstantNames;

        static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;

        static string ConstantsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "constants.dat");

        static string FunctionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "functions.dat");

        static string ConstantNamesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "constant_names.dat");

        static string FunctionNamesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "function_names.dat");

        public static void DoNothing()
        {
        }

        static ObjectsStorage()
        {
#if DEBUG
            try
            {
                File.Delete(ConstantsPath);
                File.Delete(ConstantNamesPath);
                File.Delete(FunctionsPath);
                File.Delete(FunctionNamesPath);
            }
            catch
            {
            }
#endif
            OperatorPriority = new Dictionary<string, byte>(8);
            OperatorPriority.Add("+", 1);
            OperatorPriority.Add("-", 1);
            OperatorPriority.Add("*", 2);
            OperatorPriority.Add("/", 2);
            OperatorPriority.Add("%", 2);
            OperatorPriority.Add("!", 3);
            OperatorPriority.Add(DoubleFactorialName, 3);
            OperatorPriority.Add("^", 3);
            ArithmeticActionsUnary = new Dictionary<string, Func<double, string>>();
            ArithmeticActionsUnary.Add("!", (operand) => MathematicalFunctions.Factorial((int)operand).ToString());
            ArithmeticActionsUnary.Add(DoubleFactorialName, (operand) => MathematicalFunctions.DoubleFactorial((int)operand).ToString());
            ArithmeticActionsBinary = new Dictionary<string, Func<double, double, string>>();
            ArithmeticActionsBinary.Add("+", (operand1, operand2) => (operand1 + operand2).ToString());
            ArithmeticActionsBinary.Add("-", (operand1, operand2) => (operand1 - operand2).ToString());
            ArithmeticActionsBinary.Add("*", (operand1, operand2) => (operand1 * operand2).ToString());
            ArithmeticActionsBinary.Add("/", (operand1, operand2) => (operand1 / operand2).ToString());
            ArithmeticActionsBinary.Add("%", (operand1, operand2) => (operand1 % operand2).ToString());
            ArithmeticActionsBinary.Add("^", (operand1, operand2) => (Math.Pow(operand1, operand2)).ToString());
            LoadFunctionsAndConstants();
        }

        static void LoadFunctionsAndConstants()
        {
            formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            bool WasFunctionsFileEmpty = false;
            bool WasConstantsFileEmpty = false;
            using (FileStream fs = new FileStream(ConstantsPath, FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                {
                    WasConstantsFileEmpty = true;
                }
                else
                {
                    Constants = (Dictionary<string, Constant>)formatter.Deserialize(fs);
                }
            }
            using (FileStream fs = new FileStream(ConstantNamesPath, FileMode.OpenOrCreate))
            {
                if(!WasConstantsFileEmpty)
                {
                    AllConstantNames = (HashSet<string>)formatter.Deserialize(fs);
                }
            }
            using (FileStream fs = new FileStream(FunctionsPath, FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                {
                    WasFunctionsFileEmpty = true;
                }
                else
                {
                    Functions = (Dictionary<string, Function>)formatter.Deserialize(fs);
                }
            }
            using (FileStream fs = new FileStream(FunctionNamesPath, FileMode.OpenOrCreate))
            {
                if (!WasFunctionsFileEmpty)
                {
                    AllFunctionNames = (HashSet<string>)formatter.Deserialize(fs);
                }
            }
            if (WasFunctionsFileEmpty)
            {
                AssignStandartFunctions();
            }
            if (WasConstantsFileEmpty)
            {
                AssignStandartConstants();
            }
        }

        static void AssignStandartFunctions()
        {
            AllFunctionNames = new HashSet<string>();
            Functions = new Dictionary<string, Function>();
            Functions.Add("random", new Function("random", 0, "Generates random number from 0.0 to 1.0.", FunctionType.Special, (operand) => (MathematicalFunctions.Random()).ToString()));
            Functions.Add("abs", new Function("abs", 1, "", FunctionType.Common, (operand) => (Math.Abs(operand[0])).ToString()));
            Functions.Add("sgn", new Function("sgn", 1, "", FunctionType.Common, (operand) => (Math.Sign(operand[0])).ToString()));
            Functions.Add("sqrt", new Function("sqrt", 1, "", FunctionType.Common, (operand) => (Math.Sqrt(operand[0])).ToString()));
            Functions.Add("sin", new Function("sin", 1, "", FunctionType.Trigonometric, (operand) => (Math.Sin(operand[0])).ToString()));
            Functions.Add("cos", new Function("cos", 1, "", FunctionType.Trigonometric, (operand) => (Math.Cos(operand[0])).ToString()));
            Functions.Add("tg", new Function("tg", 1, "", FunctionType.Trigonometric, (operand) => (Math.Tan(operand[0])).ToString()));
            Functions.Add("ctg", new Function("ctg", 1, "", FunctionType.Trigonometric, (operand) => (1.0 / Math.Tan(operand[0])).ToString()));
            Functions.Add("arcsin", new Function("arcsin", 1, "", FunctionType.Trigonometric, (operand) => (Math.Asin(operand[0])).ToString()));
            Functions.Add("arccos", new Function("arccos", 1, "", FunctionType.Trigonometric, (operand) => (Math.Acos(operand[0])).ToString()));
            Functions.Add("arctg", new Function("arctg", 1, "", FunctionType.Trigonometric, (operand) => (Math.Atan(operand[0])).ToString()));
            Functions.Add("arcctg", new Function("arcctg", 1, "", FunctionType.Trigonometric, (operand) => (Math.PI / 2.0 - Math.Atan(operand[0])).ToString()));
            Functions.Add("sec", new Function("sec", 1, "", FunctionType.Trigonometric, (operand) => (1.0 / Math.Cos(operand[0])).ToString()));
            Functions.Add("cosec", new Function("cosec", 1, "", FunctionType.Trigonometric, (operand) => (1.0 / Math.Sin(operand[0])).ToString()));
            Functions.Add("arcsec", new Function("arcsec", 1, "", FunctionType.Trigonometric, (operand) => (Math.Acos(1.0 / operand[0])).ToString()));
            Functions.Add("arccosec", new Function("arccosec", 1, "", FunctionType.Trigonometric, (operand) => (Math.Asin(1.0 / operand[0])).ToString()));
            Functions.Add("ln", new Function("ln", 1, "", FunctionType.Common, (operand) => (Math.Log(operand[0])).ToString()));
            Functions.Add("lg", new Function("lg", 1, "", FunctionType.Common, (operand) => (Math.Log10(operand[0])).ToString()));
            Functions.Add("exp", new Function("exp", 1, "", FunctionType.Common, (operand) => (Math.Exp(operand[0])).ToString()));
            Functions.Add("sh", new Function("sh", 1, "", FunctionType.Hyperbolic, (operand) => (Math.Sinh(operand[0])).ToString()));
            Functions.Add("ch", new Function("ch", 1, "", FunctionType.Hyperbolic, (operand) => (Math.Cosh(operand[0])).ToString()));
            Functions.Add("th", new Function("th", 1, "", FunctionType.Hyperbolic, (operand) => (1.0 / Math.Sin(operand[0])).ToString()));
            Functions.Add("cth", new Function("cth", 1, "", FunctionType.Hyperbolic, (operand) => (Math.Tanh(operand[0])).ToString()));
            Functions.Add("sech", new Function("sech", 1, "", FunctionType.Hyperbolic, (operand) => (1.0 / Math.Cosh(operand[0])).ToString()));
            Functions.Add("csch", new Function("csch", 1, "", FunctionType.Hyperbolic, (operand) => (1.0 / Math.Sinh(operand[0])).ToString()));
            Functions.Add("arsh", new Function("arsh", 1, "", FunctionType.Hyperbolic, (operand) => (MathematicalFunctions.Arsh(operand[0])).ToString()));
            Functions.Add("arch", new Function("arch", 1, "", FunctionType.Hyperbolic, (operand) => (MathematicalFunctions.Arch(operand[0])).ToString()));
            Functions.Add("arth", new Function("arth", 1, "", FunctionType.Hyperbolic, (operand) => (MathematicalFunctions.Arth(operand[0])).ToString()));
            Functions.Add("arcth", new Function("arcth", 1, "", FunctionType.Hyperbolic, (operand) => (MathematicalFunctions.Arcth(operand[0])).ToString()));
            Functions.Add("arcsech", new Function("arcsech", 1, "", FunctionType.Hyperbolic, (operand) => (MathematicalFunctions.Arsech(operand[0])).ToString()));
            Functions.Add("arccsch", new Function("arccsch", 1, "", FunctionType.Hyperbolic, (operand) => (MathematicalFunctions.Arcsch(operand[0])).ToString()));
            Functions.Add("log", new Function("log", 2, "", FunctionType.Common, (operand) => (Math.Log(operand[1], operand[0])).ToString()));
            Functions.Add("pow", new Function("pow", 2, "", FunctionType.Common, (operand) => (Math.Pow(operand[0], operand[1])).ToString()));
            Functions.Add("random_integer", new Function("random_integer", 2, "Generates random integer number", FunctionType.Special, (operand) => (MathematicalFunctions.RandomPositiveInteger((ulong)operand[0], (ulong)operand[1])).ToString()));
            AllFunctionNames.AddAllKeysToHashSet(Functions);
            SerializeFunctions();
        }

        static void AssignStandartConstants()
        {
            AllConstantNames = new HashSet<string>();
            Constants = new Dictionary<string, Constant>();
            Constants.Add("pi", new Constant("pi", "", Math.PI,false, ConstantType.Internal));
            Constants.Add("e", new Constant("e", "", Math.E,false, ConstantType.Internal));
            Constants.Add("gamma", new Constant("gamma", "", 0.577215664901533, false, ConstantType.Internal));
            Constants.Add("phi", new Constant("phi", "", 1.61803398874989,false, ConstantType.Internal));
            AllConstantNames.AddAllKeysToHashSet(Constants);
            SerializeConstants();
        }

        static void SerializeFunctions()
        {
            using (FileStream fs = new FileStream(FunctionsPath, FileMode.Open))
            {
                formatter.Serialize(fs, Functions);
            }
            using (FileStream fs = new FileStream(FunctionNamesPath, FileMode.Open))
            {
                formatter.Serialize(fs, AllFunctionNames);
            }
        }

        static void SerializeConstants()
        {
            using (FileStream fs = new FileStream(ConstantsPath, FileMode.Open))
            {
                formatter.Serialize(fs, Constants);
            }
            using (FileStream fs = new FileStream(ConstantNamesPath, FileMode.Open))
            {
                formatter.Serialize(fs, AllConstantNames);
            }
        }

        public static void SaveAllChanges()
        {
            SerializeConstants();
            SerializeFunctions();
        }

        public static void ResetFunctionsRating()
        {
            foreach (Function f in Functions.Values)
            {
                f.ResetRating();
            }
            SerializeFunctions();
        }

        public static void ResetConstantsRating()
        {
            foreach (Constant c in Constants.Values)
            {
                c.ResetRating();
            }
            SerializeConstants();
        }

        #region Addition, changing and removing

        public static void AddConstant(string Name, string Description, string Value)
        {
            Constants.Add(Name, new Constant(Name, Description, Value,true, ConstantType.Custom));
            AllConstantNames.Add(Name);
            SerializeConstants();
        }

        public static void RemoveConstant(string Name)
        {
            if (!AllConstantNames.Contains(Name))
            {
                throw new ArgumentException("This constant does not exist.");
            }
            else
            {
                if (Constants[Name].IsRemovable)
                {
                    Constants.Remove(Name);
                    AllConstantNames.Remove(Name);
                    SerializeConstants();
                }
                else
                {
                    throw new InvalidOperationException("Invalid operation: you are trying to remove or change standart internal constant.");
                }
            }
        }

        public static void ChangeConstant(string OldName, string Name, string Description, string Value)
        {
            RemoveConstant(OldName);
            AddConstant(Name, Description, Value);
        }

        public static void AddFunction(string Name, string Description, string FunctionText, params string[] Arguments)
        {
            Functions.Add(Name, new Function(Name, FunctionText, Description, FunctionType.Custom, Arguments));
            AllFunctionNames.Add(Name);
            SerializeFunctions();
        }

        public static void RemoveFunction(string Name)
        {
            if (!AllFunctionNames.Contains(Name))
            {
                throw new ArgumentException("This function does not exist.");
            }
            else
            {
                if (Functions[Name].IsRemovable)
                {
                    Functions.Remove(Name);
                    AllFunctionNames.Remove(Name);
                    SerializeFunctions();
                }
                else
                {
                    throw new InvalidOperationException("Invalid operation: you are trying to remove or change standart internal function.");
                }
            }
        }

        public static void ChangeFunction(string OldName, string Name, string Description, string FunctionText, params string[] Arguments)
        {
            RemoveFunction(OldName);
            AddFunction(Name, Description, FunctionText, Arguments);
        }

        #endregion

    }
}
