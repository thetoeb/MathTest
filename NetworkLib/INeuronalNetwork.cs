using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NetworkLib
{
    public interface INeuronalNetwork : IFeedForward
    {
        void AddNeuron(INeuron neuron);
        void RemoveNeuron(INeuron neuron);
        void AddSynapse(ISynapse synapse);
        void RemoveSynapse(ISynapse synapse);


    }
}
