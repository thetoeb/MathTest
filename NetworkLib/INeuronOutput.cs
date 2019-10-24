using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib
{
    public interface INeuronOutput : INeuron
    {
        void AddOutputSynapse(ISynapse synapse);
        void RemoveOutputSynapse(ISynapse synapse);
    }
}
