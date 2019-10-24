using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionResolver.Helper
{
    public static class ExpressionValues
    {
        public static char[] MathSymbols = new[] { '+', '-', '*', '/', '^' };

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
}
