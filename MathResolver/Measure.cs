using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathResolver
{
    public static class Measure
    {
        public static T It<T>(Func<T> func, out TimeSpan time)
        {
            var stopwatch = Stopwatch.StartNew();
            var r = func();
            stopwatch.Stop();
            time = stopwatch.Elapsed;
            return r;
        }

        public static void It(Action action, out TimeSpan time)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            time = stopwatch.Elapsed;
        }
    }
}
