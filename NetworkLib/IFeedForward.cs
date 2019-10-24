using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLib
{
    public interface IFeedForward
    {
        void FeedForward();
        void Reset();
    }
}
