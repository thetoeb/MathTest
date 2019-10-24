using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionResolver.Exceptions
{
    public class InvalidExpressionException : Exception
    {
        public string Expression;
        public InvalidExpressionException(string expression)
        {
            Expression = expression;
        }
    }
}
