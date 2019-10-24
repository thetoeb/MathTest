using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionResolver.Interface
{
    public interface IExpression
    {
        decimal Resolve();
    }
}
