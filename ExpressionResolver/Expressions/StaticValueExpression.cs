using System;
using System.Collections.Generic;
using System.Text;
using ExpressionResolver.Interface;

namespace ExpressionResolver.Expressions
{
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
}
