using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib.Model.Neurons
{
    public class OutputNeuron : FunctionNeuronProcess
    {
        public OutputNeuron(Func<double, double> func) : base(func)
        {
        }
    }
}
