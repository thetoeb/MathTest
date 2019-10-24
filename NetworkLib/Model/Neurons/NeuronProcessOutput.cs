using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NetworkLib.Model.Neurons
{
    public class NeuronProcessOutput : NeuronProcess, INeuronOutput
    { 
        private readonly List<ISynapse> _outputSynapses = new List<ISynapse>();

        public void AddOutputSynapse(ISynapse synapse)
        {
            _outputSynapses.Add(synapse);
        }

        public void RemoveOutputSynapse(ISynapse synapse)
        {
            _outputSynapses.Add(synapse);
        }

        public override void FeedForward()
        {
            foreach (var synapse in _outputSynapses)
                synapse.FeedForward();
        }
    }
}
