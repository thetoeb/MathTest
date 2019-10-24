using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NetworkLib.Model.Neurons
{
    public class NeuronProcess : Neuron, INeuronProcess
    {
        private readonly Dictionary<ISynapse, bool> _synapses = new Dictionary<ISynapse, bool>();

        protected readonly List<double> _values = new List<double>();

        public virtual void AddInputSynapse(ISynapse synapse)
        {
            _synapses.Add(synapse, false);
        }

        public virtual void RemoveInputSynapse(ISynapse synapse)
        {
            _synapses.Remove(synapse);
        }

        public virtual void ProcessInput(ISynapse synapse)
        {
            _values.Add(synapse.Value);
            _synapses[synapse] = true;

            if(_synapses.All(kv => kv.Value))
                OnAllInputs();
        }

        public virtual void Process()
        {
            Value = Process(_values);
        }

        public override void Reset()
        {
            base.Reset();
            _values.Clear();
            var keys = new List<ISynapse>(_synapses.Keys);
            foreach (var key in keys)
                _synapses[key] = false;
        }

        protected virtual void OnAllInputs()
        {
            Process();
            FeedForward();
        }

        protected virtual double Process(IEnumerable<double> values)
        {
            return (values.Sum() - Bias);
        }
    }
}
