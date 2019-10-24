using System;
using System.Collections.Generic;
using System.Text;
using ExpressionResolver.Interface;

namespace ExpressionResolver.Expressions
{
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
}
