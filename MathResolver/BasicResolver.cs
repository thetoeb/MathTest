using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathResolver
{
    public class BasicResolver : IExpression
    {
        private readonly IExpression _expression;

        public BasicResolver(string calc)
        {
            // "3 * (1 + 2 * (4 + 3)) - (1 - 2) + 1"
            calc = RemoveWhiteSpace(calc);
            _expression = GetExpression(new SubExpression(calc));
        }

        public Dictionary<string, decimal> Variables = new Dictionary<string, decimal>();

        public decimal Resolve()
        {
            return _expression.Resolve();
        }

        private IExpression GetExpression(SubExpression s)
        {
            var subs = s.GetSubExpressions();
            if (subs == null)
                return null;

            if (subs.Length == 1)
            {
                if (!subs[0].ExpressionString.HasMathSymbols())
                {
                    if (subs[0].ExpressionString.All(char.IsLetter))
                    {
                        Variables.Add(subs[0].ExpressionString, 0);
                        return new VariableValueExpression(subs[0].ExpressionString, Variables);
                    }
                    return new StaticValueExpression(subs[0].ExpressionString);
                }
                else
                {
                    var ct = GetCalcType(subs[0].ExpressionString[0]);
                    if (ct != null)
                        return new StaticCalcExpression(ct.Value);
                    else
                        throw new InvalidExpressionException(subs[0].ExpressionString);
                }
            }

            var es = new List<IExpression>();
            for (var i = 0; i < subs.Length; i++)
            {
                var ex = GetExpression(subs[i]);
                if(ex != null) es.Add(ex);
            }

            if (es.Count > 0)
            {
                if (es.Any(e => e is StaticCalcExpression))
                {
                    do
                    {
                        var index = IndexOfCalc(es);
                        var calc = (StaticCalcExpression) es[index];
                        var v1 = es[index - 1];
                        var v2 = es[index + 1];

                        var e = new CalcExpression(v1, v2, calc.Type);
                        es.RemoveRange(index-1, 3);
                        es.Insert(index-1, e);

                    } while (es.Count > 1);
                }

                return es.First();
            }

            return null;
        }

        private CalcType? GetCalcType(char c)
        {
            if (c == '+') return CalcType.Add;
            if (c == '-') return CalcType.Subtract;
            if (c == '*') return CalcType.Multiply;
            if (c == '/') return CalcType.Divide;
            if (c == '^') return CalcType.Power;
            //if (c == '#') return CalcType.Add;
            return null;
        }

        private string RemoveWhiteSpace(string s)
        {
            var result = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                if (!char.IsWhiteSpace(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        private int IndexOfCalc(List<IExpression> l, params CalcType[] types)
        {
            var o = l.FirstOrDefault(e => e is StaticCalcExpression sc && types.Contains(sc.Type));
            return l.IndexOf(o);
        }

        private int IndexOfCalc(List<IExpression> l)
        {
            var index = IndexOfCalc(l, CalcType.Power, CalcType.SquareRoot);
            if (index == -1)
            {
                index = IndexOfCalc(l, CalcType.Multiply, CalcType.Divide);
                if (index == -1)
                {
                    index = IndexOfCalc(l, CalcType.Add, CalcType.Subtract);
                }
            }

            return index;
        }
    }

    public interface IExpression
    {
        decimal Resolve();
    }

    public class InvalidExpressionException : Exception
    {
        public string Expression;
        public InvalidExpressionException(string expression)
        {
            Expression = expression;
        }
    }

    public static class ExpressionValues
    {
        public static char[] MathSymbols = new[] {'+', '-', '*', '/', '^'};

        public static bool HasMathSymbols(this string s)
        {
            return s.Any(c => MathSymbols.Contains(c));
        }

        public static bool HasMathSymbol(this string s, int index)
        {
            return MathSymbols.Contains(s[index]);
        }

        public static bool IsMathSymbol(char c)
        {
            return MathSymbols.Contains(c);
        }
    }

    public enum CalcType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Power,
        SquareRoot,
    }

    public class StaticValueExpression : IExpression
    {
        public decimal Value;

        public StaticValueExpression(decimal d)
        {
            Value = d;
        }

        public StaticValueExpression(string d)
        {
            Value = decimal.Parse(d);
        }

        public decimal Resolve()
        {
            return Value;
        }

        public override string ToString()
        {
            return $"ValueExpression: \"{Value}\"";
        }
    }

    public class VariableValueExpression : IExpression
    {
        private Dictionary<string, decimal> _lookUp;
        public string VariableName;

        public VariableValueExpression(string name, Dictionary<string, decimal> lookup)
        {
            VariableName = name;
            _lookUp = lookup;
        }

        public decimal Resolve()
        {
            if (!_lookUp.ContainsKey(VariableName))
                return 0;
            return _lookUp[VariableName];
        }
    }

    public class StaticCalcExpression : IExpression
    {
        public CalcType Type;

        public StaticCalcExpression(CalcType c)
        {
            Type = c;
        }
        public decimal Resolve()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"CalcExpression: \"{Type}\"";
        }
    }
    public class CalcExpression : IExpression
    {
        public IExpression Expression1;
        public IExpression Expression2;
        public CalcType Type;

        public CalcExpression(IExpression e1, IExpression e2, CalcType type)
        {
            Expression1 = e1;
            Expression2 = e2;
            Type = type;
        }

        public CalcExpression(IExpression e1, IExpression e2)
        {
            Expression1 = e1;
            Expression2 = e2;
        }

        public CalcExpression()
        {
        }

        public decimal Resolve()
        {
            var v1 = Expression1.Resolve();
            var v2 = Expression2.Resolve();

            switch (Type)
            {
                case CalcType.Add:
                    return v1 + v2;
                case CalcType.Subtract:
                    return v1 - v2;
                case CalcType.Multiply:
                    return v1 * v2;
                case CalcType.Divide:
                    return v1 / v2;
                case CalcType.Power:
                    return Convert.ToDecimal(Math.Pow(Convert.ToDouble(v1), Convert.ToDouble(v2)));
                case CalcType.SquareRoot:
                    return Convert.ToDecimal(Math.Pow(Convert.ToDouble(v1), 1.0 / Convert.ToDouble(v2)));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    public class SubExpression
    {
        public SubExpression Parent;
        public string ExpressionString;

        public SubExpression(string s)
        {
            ExpressionString = s;
        }

        public SubExpression(string s, SubExpression parent)
        {
            Parent = parent;
            ExpressionString = s;
        }

        public SubExpression[] GetSubExpressions()
        {
            if (string.IsNullOrWhiteSpace(ExpressionString))
                return null;

            if (!ExpressionString.Contains('(') && !ExpressionString.Contains(')'))
                return GetSubs();

            var subs = 0;
            var plain = true;
            var currentExpression = string.Empty;
            var result = new List<SubExpression>();
            for (var i = 0; i < ExpressionString.Length; i++)
            {
                var c = ExpressionString[i];

                if (c == '(')
                {
                    subs++;

                    if (subs == 1)
                    {
                        if(!string.IsNullOrWhiteSpace(currentExpression))
                            result.Add(new SubExpression(currentExpression, this));
                        currentExpression = string.Empty;
                    }
                    else
                    {
                        currentExpression += c;
                    }
                }
                else if (c == ')')
                {
                    subs--;
                    

                    if (subs == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(currentExpression))
                            result.Add(new SubExpression(currentExpression, this));
                        currentExpression = string.Empty;
                    }
                    else
                    {
                        currentExpression += c;
                    }
                }
                else if (ExpressionValues.IsMathSymbol(c) && subs == 0)
                {
                    if (!string.IsNullOrWhiteSpace(currentExpression))
                        result.Add(new SubExpression(currentExpression, this));
                    currentExpression = string.Empty;
                    result.Add(new SubExpression(c.ToString(), this));
                }
                else
                {
                    currentExpression += c;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentExpression))
                result.Add(new SubExpression(currentExpression, this));

            if (result.Count == 1 && result[0].ExpressionString == ExpressionString)
                return null;

            //var newExpressions = new List<(int, SubExpression)>();
            //for (var i = 0; i < result.Count; i++)
            //{
            //    var se = result[i];
            //    if (se.ExpressionString.Length > 1)
            //    {
            //        if (se.ExpressionString.HasMathSymbol(0))
            //        {
            //            newExpressions.Add((i, new SubExpression(se.ExpressionString[0].ToString(), this)));
            //            se.ExpressionString = se.ExpressionString.Substring(1);
            //        }

            //        if (se.ExpressionString.HasMathSymbol(se.ExpressionString.Length - 1))
            //        {
            //            newExpressions.Add((i+1, new SubExpression(ExpressionString[se.ExpressionString.Length - 1].ToString(), this)));
            //            se.ExpressionString = se.ExpressionString.Substring(0, se.ExpressionString.Length - 1);
            //        }
            //    }
            //}

            //var offset = 0;
            //foreach (var t in newExpressions)
            //{
            //   result.Insert(t.Item1+offset++, t.Item2);
            //}

            return result.ToArray();
        }

        private SubExpression[] GetSubs()
        {
            var se = new List<SubExpression>();
            var currentExpression = string.Empty;
            for (var i = 0; i < ExpressionString.Length; i++)
            {
                if (ExpressionValues.IsMathSymbol(ExpressionString[i]))
                {
                    if (!string.IsNullOrWhiteSpace(currentExpression))
                    {
                        se.Add(new SubExpression(currentExpression, this));
                        currentExpression = string.Empty;
                    }
                    se.Add(new SubExpression(ExpressionString[i].ToString(), this));
                }
                else
                {
                    currentExpression += ExpressionString[i];
                }
            }

            if (!string.IsNullOrWhiteSpace(currentExpression))
                se.Add(new SubExpression(currentExpression, this));
            
            return se.ToArray();
        }

        public override string ToString()
        {
            return $"[\"{ExpressionString}\"]{(Parent != null ? " => "+Parent : "" )}";
        }
    }
}
