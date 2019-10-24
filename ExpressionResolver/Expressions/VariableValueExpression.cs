using System;
using System.Collections.Generic;
using System.Text;
using ExpressionResolver.Interface;

namespace ExpressionResolver.Expressions
{
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
}
