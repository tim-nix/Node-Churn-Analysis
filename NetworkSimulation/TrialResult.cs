using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class TrialResult
    {
        public double Delay { get; set; }
        public int NumLive { get; set; }
        public double PercentLive { get; set; }
        public double StartTime { get; set; }
        public bool Success { get; set; }
    }
}
