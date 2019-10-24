using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib
{
    public interface INeuronProcess : INeuronInput
    {
        void Process();
    }
}
