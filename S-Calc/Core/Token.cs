using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    /// <summary>
    /// Тип токена
    /// </summary>
    [Serializable]
    public enum TokenType
    {
        /// <summary>
        /// 
        /// </summary>
        Undefined,
        /// <summary>
        /// Токен является числом
        /// </summary>
        Number,
        /// <summary>
        /// Токен является функцией
        /// </summary>
        Function,
        /// <summary>
        /// Токен является разделителем аргументов функции
        /// </summary>
        Separator,
        /// <summary>
        /// Токен является унарным оператором
        /// </summary>
        UnaryOperator,
        /// <summary>
        /// Токен является бинарным оператором
        /// </summary>
        BinaryOperator,
        /// <summary>
        /// Токен является левой круглой скобкой
        /// </summary>
        LeftBracket,
        /// <summary>
        /// Токен является правой круглой скобкой
        /// </summary>
        RightBracket,
        /// <summary>
        /// Токен является переменным значением
        /// </summary>
        Variable
    }

    /// <summary>
    /// Элемент лексического анализа. Представляет собой любую единицу в математической записи выражения.
    /// </summary>
    [Serializable]
    struct Token : IEquatable<Token>, IEnumerable<char>, IFormattable
    {
        /// <summary>
        /// Строка, что относится к данному токену.
        /// </summary>
        public readonly string Title;

        /// <summary>
        /// Тип токена.
        /// </summary>
        public readonly TokenType Type;

        /// <summary>
        /// Место токена в математической записи (первый токен в записи должен быть под номером 1, в конструктор в этом случае нужно передать 0).
        /// </summary>
        public readonly int Position;

        public readonly string Text;

        public readonly double? Value;

        public readonly int? OperatorPriority;

        public readonly Function Function;

        /// <summary>
        /// Конструктор типа.
        /// </summary>
        /// <param name="s">Строка, которая расценивается как токен.</param>
        /// <param name="position">Место токена в математической записи (первый токен в записи должен быть под номером 1, в конструктор в этом случае нужно передать 0).</param>
        public Token(string s, int position)
        {
            Title = string.Copy(s);
            Position = position;
            Value = null;
            OperatorPriority = null;
            Function = null;
            Text = null;
            if (s.IsNumber())
            {
                Type = TokenType.Number;
                if (ObjectsStorage.Constants.ContainsKey(s))
                {
                    Value = ObjectsStorage.Constants[s].Value;
                }
                else
                {
                     Value = double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
            }
            else if (ObjectsStorage.OperatorPriority.ContainsKey(s))
            {
                OperatorPriority = ObjectsStorage.OperatorPriority[s];
                if (s == "!" || s == ObjectsStorage.DoubleFactorialName)
                {
                    Type = TokenType.UnaryOperator;
                }
                else
                {
                    Type = TokenType.BinaryOperator;
                }
            }
            else if (ObjectsStorage.AllFunctionNames.Contains(s))
            {
                Type = TokenType.Function;
                Function = ObjectsStorage.Functions[s];
            }
            else if (s == "(")
            {
                Type = TokenType.LeftBracket;
                Text = "(";
            }
            else if (s == ")")
            {
                Type = TokenType.RightBracket;
                Text = ")";
            }
            else if (s == ",")
            {
                Type = TokenType.Separator;
                Text = ",";
            }
            else
            {
                throw new ArgumentException($"Undefined token in position {Position}.");
            }
        }

        /// <summary>
        /// Конструктор типа (используется при вызове из меню создания пользовательской функции).
        /// </summary>
        /// <param name="s">Строка, которая расценивается как токен.</param>
        /// <param name="position">Место токена в математической записи (первый токен в записи должен быть под номером 1, в конструктор в этом случае нужно передать 0).</param>
        /// <param name="FunctionArgumentNames">Массив имён аргументов.</param>
        public Token(string s, int position, string[] FunctionArgumentNames)
        {
            Title = string.Copy(s);
            Position = position;
            Value = null;
            OperatorPriority = null;
            Function = null;
            Text = null;
            if (s.IsNumber())
            {
                Type = TokenType.Number;
                if (ObjectsStorage.Constants.ContainsKey(s))
                {
                    Value = ObjectsStorage.Constants[s].Value;
                }
                else
                {
                    Value = double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
            }
            else if (ObjectsStorage.OperatorPriority.ContainsKey(s))
            {
                OperatorPriority = ObjectsStorage.OperatorPriority[s];
                if (s == "!" || s == ObjectsStorage.DoubleFactorialName)
                {
                    Type = TokenType.UnaryOperator;
                }
                else
                {
                    Type = TokenType.BinaryOperator;
                }
            }
            else if (ObjectsStorage.AllFunctionNames.Contains(s))
            {
                Type = TokenType.Function;
                Function = ObjectsStorage.Functions[s];
            }
            else if (s == "(")
            {
                Type = TokenType.LeftBracket;
            }
            else if (s == ")")
            {
                Type = TokenType.RightBracket;
            }
            else if (s == ",")
            {
                Type = TokenType.Separator;
            }
            else if (FunctionArgumentNames.Contains(s))
            {
                Type = TokenType.Variable;
                Text = string.Copy(s);
            }
            else
            {
                throw new ArgumentException($"Undefined token in position {Position}.");
            }
        }

        /// <summary>
        /// Проверка на равенство объекту.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Token ? Equals((Token)obj) : false;
        }

        /// <summary>
        /// Проверка на равенство другому токену.
        /// </summary>
        /// <param name="other">Другой токен.</param>
        /// <returns></returns>
        public bool Equals(Token other)
        {
            if (Type == other.Type)
            {
                return string.Equals(Title, other.Title);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Получение перечислителя, что вызывает GetEnumerator of String на поле Title.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<char> GetEnumerator()
        {
            return Title.GetEnumerator();
        }

        /// <summary>
        /// Получение хэш-кода токена.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Title.GetHashCode() ^ Type.GetHashCode();
        }

        /// <summary>
        /// Строковое представление токена.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Type == TokenType.Number ?  Value.Value.ToString(CultureInfo.InvariantCulture) : Title;
        }

        /// <summary>
        /// Строковое представление токена.
        /// </summary>
        /// <param name="format">Строка форматирования.</param>
        /// <param name="formatProvider">Поставщик формата.</param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Type== TokenType.Number? string.Format(formatProvider,format, Value.Value.ToString(CultureInfo.InvariantCulture)) : string.Format(formatProvider, format, Title);
        }

        /// <summary>
        /// Получение перечислителя, что вызывает GetEnumerator of String на поле Title.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
