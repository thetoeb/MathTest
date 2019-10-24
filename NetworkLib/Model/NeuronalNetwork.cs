using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NetworkLib.Model.Synapses;

namespace NetworkLib.Model
{
    public class NeuronalNetwork : INeuronalNetwork
    {
        private readonly List<INeuron> _neurons = new List<INeuron>();
        private readonly List<ISynapse> _synapses = new List<ISynapse>();
        private readonly List<INeuron> _startingNeurons = new List<INeuron>();
        private readonly List<INeuron> _endingNeurons = new List<INeuron>();
      
        public void AddNeuron(INeuron neuron)
        {
            _neurons.Add(neuron);

            if (_synapses.All(s => s.Target != neuron))
                _startingNeurons.Add(neuron);

            if (_synapses.All(s => s.Source != neuron))
                _endingNeurons.Add(neuron);
        }

        public void RemoveNeuron(INeuron neuron)
        {
            _neurons.Remove(neuron);

            if (_startingNeurons.Contains(neuron))
                _startingNeurons.Remove(neuron);

            if (_endingNeurons.Contains(neuron))
                _endingNeurons.Remove(neuron);
        }

        public void AddSynapse(ISynapse synapse)
        {
            _synapses.Add(synapse);

            if (synapse.Source is INeuronOutput outputNeuron)
            {
                outputNeuron.AddOutputSynapse(synapse);

                if (_endingNeurons.Contains(synapse.Source))
                    _endingNeurons.Remove(synapse.Source);
            }

            if (synapse.Target is INeuronInput inputNeuron)
            {
                inputNeuron.AddInputSynapse(synapse);

                if (_startingNeurons.Contains(synapse.Target))
                    _startingNeurons.Remove(synapse.Target);
            }
        }

        public void RemoveSynapse(ISynapse synapse)
        {
            _synapses.Remove(synapse);

            if (synapse.Source is INeuronOutput outputNeuron)
            {
                outputNeuron.RemoveOutputSynapse(synapse);

                if (_synapses.All(s => s.Source != synapse.Source))
                    _endingNeurons.Add(synapse.Source);
            }

            if (synapse.Target is INeuronInput inputNeuron)
            {
                inputNeuron.RemoveInputSynapse(synapse);

                if (_synapses.All(s => s.Target != synapse.Target))
                    _startingNeurons.Add(synapse.Target);
            }
        }

        public void FeedForward()
        {
            foreach (var neuron in _startingNeurons)
            {
                neuron.FeedForward();
            }
        }

        public void Reset()
        {
            foreach (var allNeuron in _neurons)
                allNeuron.Reset();

            foreach (var synapse in _synapses)
               synapse.Reset();
        }

        public ISynapse CreateSynapse(INeuron source, INeuron target) 
        {
            var synapse = new Synapse(source, target);
            AddSynapse(synapse);
            return synapse;
        }
    }
}
