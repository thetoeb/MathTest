using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib
{
    public interface INeuron : IFeedForward
    {
        double Value { get; set; }
        double Bias { get; set; }
    }
}
