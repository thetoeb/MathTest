using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NetworkLib.Model.Synapses
{
    public class Synapse : ISynapse
    {
        public Synapse(INeuron source, INeuron target)
        {
            Weight = 1;
            Value = 0;
            Source = source;
            Target = target;
        }

        public double Weight { get; set; }
        public double Value { get; set; }
        public INeuron Source { get; }
        public INeuron Target { get; }

        public void FeedForward()
        {
            Value = Source.Value * Weight;

            if (Target is INeuronInput input)
            {
                input.ProcessInput(this);
            }
        }

        public void Reset()
        {
            Value = 0;
        }
    }
}
