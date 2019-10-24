using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NetworkLib.Model.Neurons
{
    public class InputNeuron : Neuron, INeuronOutput
    {
        private readonly List<ISynapse> _synapses = new List<ISynapse>();

        public void AddOutputSynapse(ISynapse synapse)
        {
            _synapses.Add(synapse);
        }

        public void RemoveOutputSynapse(ISynapse synapse)
        {
            _synapses.Remove(synapse);
        }

        public override void FeedForward()
        {
            foreach (var synapse in _synapses)
                synapse.FeedForward();
        }
    }
}
