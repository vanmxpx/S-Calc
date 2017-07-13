using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace RPNClassLibraryCSharp
{
    static class ObjectsStorage
    {
        internal static Dictionary<string, byte> OperatorPriority;

        internal static Dictionary<string, Func<double, string>> ArithmeticActionsUnary;

        internal static Dictionary<string, Func<double, double, string>> ArithmeticActionsBinary;

        internal static Dictionary<string, Constant> InternalConstants;

        internal static Dictionary<string, Constant> CustomConstants;

        internal static Dictionary<string, Function> InternalFunctions;

        internal static Dictionary<string, Function> CustomFunctions;

        internal static HashSet<string> AllFunctionNames;

        internal static HashSet<string> AllConstantNames;

        static string ConstantsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "constants.dat");

        static string FunctionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "functions.dat");

        static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;

        static ObjectsStorage()
        {
            OperatorPriority = new Dictionary<string, byte>(8);
            OperatorPriority.Add("+", 1);
            OperatorPriority.Add("-", 1);
            OperatorPriority.Add("*", 2);
            OperatorPriority.Add("/", 2);
            OperatorPriority.Add("%", 2);
            OperatorPriority.Add("!", 3);
            OperatorPriority.Add("!!", 3);
            OperatorPriority.Add("^", 3);
            ArithmeticActionsUnary = new Dictionary<string, Func<double, string>>();
            ArithmeticActionsUnary.Add("!", (operand) => MathematicalFunctions.Factorial((int)operand).ToString());
            ArithmeticActionsUnary.Add("!!", (operand) => MathematicalFunctions.DoubleFactorial((int)operand).ToString());
            ArithmeticActionsBinary = new Dictionary<string, Func<double, double, string>>();
            ArithmeticActionsBinary.Add("+", (operand1, operand2) => (operand1 + operand2).ToString());
            ArithmeticActionsBinary.Add("-", (operand1, operand2) => (operand1 - operand2).ToString());
            ArithmeticActionsBinary.Add("*", (operand1, operand2) => (operand1 * operand2).ToString());
            ArithmeticActionsBinary.Add("/", (operand1, operand2) => (operand1 / operand2).ToString());
            ArithmeticActionsBinary.Add("%", (operand1, operand2) => (operand1 % operand2).ToString());
            ArithmeticActionsBinary.Add("^", (operand1, operand2) => (Math.Pow(operand1, operand2)).ToString());
            AllFunctionNames = new HashSet<string>();
            AllConstantNames = new HashSet<string>();
            InternalFunctions = new Dictionary<string, Function>();
            InternalFunctions.Add("random", new Function("random", 0, "Generates random number from 0.0 to 1.0.", (operand) => (MathematicalFunctions.Random()).ToString()));
            InternalFunctions.Add("abs", new Function("abs", 1, "", (operand) => (Math.Abs(operand[0])).ToString()));
            InternalFunctions.Add("sgn", new Function("sgn", 1, "", (operand) => (Math.Sign(operand[0])).ToString()));
            InternalFunctions.Add("sqrt", new Function("sqrt", 1, "", (operand) => (Math.Sqrt(operand[0])).ToString()));
            InternalFunctions.Add("sin", new Function("sin", 1, "", (operand) => (Math.Sin(operand[0])).ToString()));
            InternalFunctions.Add("cos", new Function("cos", 1, "", (operand) => (Math.Cos(operand[0])).ToString()));
            InternalFunctions.Add("tg", new Function("tg", 1, "", (operand) => (Math.Tan(operand[0])).ToString()));
            InternalFunctions.Add("ctg", new Function("ctg", 1, "", (operand) => (1.0/Math.Tan(operand[0])).ToString()));
            InternalFunctions.Add("arcsin", new Function("arcsin", 1, "", (operand) => (Math.Asin(operand[0])).ToString()));
            InternalFunctions.Add("arccos", new Function("arccos", 1, "", (operand) => (Math.Acos(operand[0])).ToString()));
            InternalFunctions.Add("arctg", new Function("arctg", 1, "", (operand) => (Math.Atan(operand[0])).ToString()));
            InternalFunctions.Add("arcctg", new Function("arcctg", 1, "", (operand) => (Math.PI / 2.0 - Math.Atan(operand[0])).ToString()));
            InternalFunctions.Add("sec", new Function("sec", 1, "", (operand) => (1.0 / Math.Cos(operand[0])).ToString()));
            InternalFunctions.Add("cosec", new Function("cosec", 1, "", (operand) => (1.0 / Math.Sin(operand[0])).ToString()));
            InternalFunctions.Add("arcsec", new Function("arcsec", 1, "", (operand) => (Math.Acos(1.0 / operand[0])).ToString()));
            InternalFunctions.Add("arccosec", new Function("arccosec", 1, "", (operand) => (Math.Asin(1.0 / operand[0])).ToString()));
            InternalFunctions.Add("ln", new Function("ln", 1, "", (operand) => (Math.Log(operand[0])).ToString()));
            InternalFunctions.Add("lg", new Function("lg", 1, "", (operand) => (Math.Log10(operand[0])).ToString()));
            InternalFunctions.Add("exp", new Function("exp", 1, "", (operand) => (Math.Exp(operand[0])).ToString()));
            InternalFunctions.Add("sh", new Function("sh", 1, "", (operand) => (Math.Sinh(operand[0])).ToString()));
            InternalFunctions.Add("ch", new Function("ch", 1, "", (operand) => (Math.Cosh(operand[0])).ToString()));
            InternalFunctions.Add("th", new Function("th", 1, "", (operand) => (1.0 / Math.Sin(operand[0])).ToString()));
            InternalFunctions.Add("cth", new Function("cth", 1, "", (operand) => (Math.Tanh(operand[0])).ToString()));
            InternalFunctions.Add("sech", new Function("sech", 1, "", (operand) => (1.0 / Math.Cosh(operand[0])).ToString()));
            InternalFunctions.Add("csch", new Function("csch", 1, "", (operand) => (1.0 / Math.Sinh(operand[0])).ToString()));
            InternalFunctions.Add("arsh", new Function("arsh", 1, "", (operand) => (MathematicalFunctions.Arsh(operand[0])).ToString()));
            InternalFunctions.Add("arch", new Function("arch", 1, "", (operand) => (MathematicalFunctions.Arch(operand[0])).ToString()));
            InternalFunctions.Add("arth", new Function("arth", 1, "", (operand) => (MathematicalFunctions.Arth(operand[0])).ToString()));
            InternalFunctions.Add("arcth", new Function("arcth", 1, "", (operand) => (MathematicalFunctions.Arcth(operand[0])).ToString()));
            InternalFunctions.Add("arcsech", new Function("arcsech", 1, "", (operand) => (MathematicalFunctions.Arsech(operand[0])).ToString()));
            InternalFunctions.Add("arccsch", new Function("arccsch", 1, "", (operand) => (MathematicalFunctions.Arcsch(operand[0])).ToString()));
            InternalFunctions.Add("log", new Function("log", 2, "", (operand) => (Math.Log(operand[1], operand[0])).ToString()));
            InternalFunctions.Add("pow", new Function("pow", 2, "", (operand) => (Math.Pow(operand[0], operand[1])).ToString()));
            InternalFunctions.Add("random_integer", new Function("random_integer", 2, "Generates random integer number", (operand) => (MathematicalFunctions.RandomPositiveInteger((ulong)operand[0], (ulong)operand[1])).ToString()));
            InternalConstants = new Dictionary<string, Constant>();
            InternalConstants.Add("pi", new Constant("pi", "", Math.PI));
            InternalConstants.Add("e", new Constant("e", "", Math.E));
            InternalConstants.Add("gamma", new Constant("gamma", "", 0.577215664901533));
            InternalConstants.Add("phi", new Constant("phi", "", 1.61803398874989));
            formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(ConstantsPath, FileMode.OpenOrCreate))
                {
                    if (fs.Length == 0)
                    {
                        CustomConstants = new Dictionary<string, Constant>();
                    }
                    else
                    {
                        CustomConstants = (Dictionary<string, Constant>)formatter.Deserialize(fs);
                    }
                }
            }
            catch (Exception e)
            {
                CustomConstants = new Dictionary<string, Constant>();
            }
            try
            {
                using (FileStream fs = new FileStream(FunctionsPath, FileMode.OpenOrCreate))
                {
                    if (fs.Length == 0)
                    {
                        CustomFunctions = new Dictionary<string, Function>();
                    }
                    else
                    {
                        CustomFunctions = (Dictionary<string, Function>)formatter.Deserialize(fs);
                    }
                }
            }
            catch (Exception e)
            {
                CustomFunctions = new Dictionary<string, Function>();
            }
            AllFunctionNames.AddAllKeysToHashSet(InternalFunctions);
            AllFunctionNames.AddAllKeysToHashSet(CustomFunctions);
            AllConstantNames.AddAllKeysToHashSet(InternalConstants);
            AllConstantNames.AddAllKeysToHashSet(CustomConstants);
        }

        static void SerializeFunction()
        {
            using (FileStream fs = new FileStream(FunctionsPath, FileMode.Open))
            {
                formatter.Serialize(fs, CustomFunctions);
            }
        }

        static void SerializeConstant()
        {
            using (FileStream fs = new FileStream(ConstantsPath, FileMode.Open))
            {
                formatter.Serialize(fs, CustomConstants);
            }
        }

        internal static void AddConstant(string Name, string Description, string Value)
        {
            CustomConstants.Add(Name, new Constant(Name, Description, Value));
            AllConstantNames.Add(Name);
            SerializeConstant();
        }

        internal static void RemoveConstant(string Name)
        {
            if (!AllConstantNames.Contains(Name))
            {
                throw new ArgumentException("This constant does not exist.");
            }
            else
            {
                CustomConstants.Remove(Name);
                AllConstantNames.Remove(Name);
                SerializeConstant();
            }
        }

        internal static void ChangeConstant(string OldName, string Name, string Description, string Value)
        {
            RemoveConstant(OldName);
            AddConstant(Name, Description, Value);
        }

        internal static void AddFunction(string Name, string Description, string FunctionText, params string[] Arguments)
        {
            CustomFunctions.Add(Name, new Function(Name, FunctionText, Description, Arguments));
            AllFunctionNames.Add(Name);
            SerializeFunction();
        }

        internal static void RemoveFunction(string Name)
        {
            if (!AllFunctionNames.Contains(Name))
            {
                throw new ArgumentException("This function does not exist.");
            }
            else
            {
                CustomFunctions.Remove(Name);
                AllFunctionNames.Remove(Name);
                SerializeFunction();
            }
        }

        internal static void ChangeFunction(string OldName, string Name, string Description, string FunctionText, params string[] Arguments)
        {
            RemoveFunction(OldName);
            AddFunction(Name, Description, FunctionText, Arguments);
        }
    }
}
