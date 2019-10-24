using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NetworkLib.Model.Neurons
{
    public class FunctionNeuronProcess : NeuronProcessOutput
    {
        private readonly Func<double, double> _func;

        public FunctionNeuronProcess(Func<double, double> func)
        {
            _func = func;
        }

        protected override double Process(IEnumerable<double> values)
        {
            var value = values.Sum() - Bias;
            return _func(value);
        }
    }
}
