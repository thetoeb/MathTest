using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib.Model
{
    public class BackPropagation : IBackPropagate
    {
        public void Process(double targetValue, INeuron neuron, IEnumerable<ISynapse> synapses)
        {
            var diff = targetValue - neuron.Value;
            
        }
    }
}
