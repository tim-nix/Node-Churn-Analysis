using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class TopologyComparison
    {
        public int NumNodes { get; set; }

        public double PathAverageDelay { get; set; }
        public double CycleAverageDelay { get; set; }

        public double DelayReduction { get; set; }
        public double DelayReductionPercent { get; set; }
    }
}
