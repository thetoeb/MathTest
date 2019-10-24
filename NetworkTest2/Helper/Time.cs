using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NetworkTest2.Helper
{
    public class Time : IDisposable
    {
        public static event Action<string> LogAction = s => { };
        public static event Action<string> StartLogAction = s => { };
        public static Func<string, TimeSpan, string> LogText = (name, ts) => $"Time Ellapsed for '{name}': {ts.TotalMilliseconds}ms";
        public static Func<string, string> StartText = (name) => $"Starting '{name}'.";                                                
            
       
        private readonly Stopwatch _stopwatch;
        private readonly string _name;

        public Time(string name)
        {
            _name = name;
            _stopwatch = new Stopwatch();

            var message = StartText(_name);
            StartLogAction?.Invoke(message);

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var message = LogText(_name, _stopwatch.Elapsed);
            LogAction(message);
        }


        public static Time Capture(string name)
        {
            return new Time(name);
        }
    }
}
