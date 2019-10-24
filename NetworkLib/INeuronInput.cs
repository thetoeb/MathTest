using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib
{
    public interface INeuronInput : INeuron
    {
        void AddInputSynapse(ISynapse synapse);
        void RemoveInputSynapse(ISynapse synapse);
        void ProcessInput(ISynapse synapse);
    }
}
