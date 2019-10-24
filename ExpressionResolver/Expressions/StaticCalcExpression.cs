using System;
using System.Collections.Generic;
using System.Text;
using ExpressionResolver.Interface;

namespace ExpressionResolver.Expressions
{
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
}
