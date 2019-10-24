using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkTest2.Model
{
    public class Synapse
    {
        public double Input;
        public double Weight;
        public double Output;

        public void Process()
        {
            Output = Input * Weight;
        }
    }
}
