using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetworkTest2.Helper
{
    public static class MathE
    {
        public static double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1d + k);
        }

        public static double SigmoidDerived(double value)
        {
            return value * (1.0 - value);
        } 
    }
}
