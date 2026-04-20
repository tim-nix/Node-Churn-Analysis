using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class SimulationSettings
    {
        public int NodeCount { get; set; }
        public int SimulationCount { get; set; }

        public double BaseTime { get; set; }
        public double StartOffset { get; set; }

        public Exponential UptimeDistribution { get; set; }
        public Exponential DowntimeDistribution { get; set; }
    }
}
