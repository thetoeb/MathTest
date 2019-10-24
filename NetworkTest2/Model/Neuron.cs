using System;
using System.Collections.Generic;
using System.Text;
using NetworkTest2.Helper;

namespace NetworkTest2.Model
{
    public class Neuron
    {
        public double Input;
        public double Bias;
        public double Output;
        public double Error;

        public void Process()
        {
            Output = MathE.Sigmoid(Input + Bias);
        }

        public double DerivateProcess(double value)
        {
            return MathE.SigmoidDerived(value);
        }
    }
}
