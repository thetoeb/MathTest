using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib
{
    public interface ISynapse : IFeedForward
    {
        double Weight { get; set; }
        double Value { get; set; }

        INeuron Source { get; }
        INeuron Target { get; }
    }
}
