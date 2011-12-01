using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FelpoII.Core.Engines
{
    public class RunClock:IDisposable
    {
        Stopwatch sw;
        public RunClock()
        {
            sw = new Stopwatch();
            sw.Start();
        }


        public long GetElapsedMilliseconds()
        {
            return sw.ElapsedMilliseconds;
        }
        #region IDisposable Members

        public void Dispose()
        {
            sw.Stop();
        }

        #endregion
    }
}
