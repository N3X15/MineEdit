using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;

namespace OpenMinecraft
{
    /// <summary>
    /// Courtesy of MSDN.  Adapted into a class
    /// </summary>
    public class Profiler
    {
        // Define variables for operation statistics.
        int numSamples = 0;
        long numMS = 0;
        long slowestRun = 0;
        long fastestRun = long.MaxValue;
        string Operation;
        Stopwatch timer;

        public Profiler(string opname)
        {
            Operation = opname;
        }

        public void Start()
        {
            timer = Stopwatch.StartNew();
        }

        public void Stop()
        {
            timer.Stop();
            long ms = timer.ElapsedMilliseconds;
            if(numSamples++!=0) // Both dumps the first record AND makes it a count instead of an index!
            {
                if (slowestRun < ms)
                    slowestRun = ms;
                if (fastestRun > ms)
                    fastestRun = ms;
                numMS += ms;
            }
            timer.Reset();
        }

        public override string ToString()
        {            
            long min = fastestRun;//*nanosecPerTick;
            long max = slowestRun;//*nanosecPerTick;
            long avg = numMS/numSamples;
            return string.Format("[PROFILER] Operation {0} complete with {1} sample(s) - Min: {2} ms, Max: {3} ms, Avg: {4} ms", Operation, numSamples, min, max, avg);
        }
    }
}
