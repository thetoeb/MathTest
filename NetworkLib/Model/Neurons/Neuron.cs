using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NetworkLib.Model.Neurons
{
    public abstract class Neuron : INeuron
    {   
        public Neuron()
        {
            Value = 0;
            Bias = 0;
        }

        public double Value { get; set; }
        public double Bias { get; set; }

        public virtual void FeedForward()
        {            
        }       

        public virtual void Reset()
        {
            Value = 0;
        }
    }
}
