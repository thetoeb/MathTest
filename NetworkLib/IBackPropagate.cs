using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib
{
    public interface IBackPropagate
    {
        void Process(double targetValue, INeuron neuron, IEnumerable<ISynapse> synapses);
    }
}
