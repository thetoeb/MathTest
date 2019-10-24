using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionResolver.Exceptions;
using ExpressionResolver.Expressions;
using ExpressionResolver.Helper;
using ExpressionResolver.Interface;

namespace ExpressionResolver
{
    public class Resolver : IExpression
    {
        private readonly IExpression _expression;

        public Resolver(string calc)
        {
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
                        if (!Variables.ContainsKey(subs[0].ExpressionString))
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
                if (ex != null) es.Add(ex);
            }

            if (es.Count > 0)
            {
                if(es.Count < 3)
                    throw new InvalidExpressionException(s.ExpressionString);

                if (es.Any(e => e is StaticCalcExpression))
                {
                    do
                    {
                        var index = IndexOfCalc(es);
                        var calc = (StaticCalcExpression)es[index];
                        var v1 = es[index - 1];
                        var v2 = es[index + 1];

                        var e = new CalcExpression(v1, v2, calc.Type);
                        es.RemoveRange(index - 1, 3);
                        es.Insert(index - 1, e);

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

        private class SubExpression
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
                            if (!string.IsNullOrWhiteSpace(currentExpression))
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
                return $"[\"{ExpressionString}\"]{(Parent != null ? " => " + Parent : "")}";
            }
        }
    }
}
