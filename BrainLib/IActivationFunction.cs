using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainLib
{
    public interface IActivationFunction
    {
        double Calculate(double value);
    }
}
