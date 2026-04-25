using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class ResultSummary
    {
        public double ConnectivityPercent { get; set; }
        public double AverageMessageDelay { get; set; }
        public double AverageLivePercent { get; set; }
        public int TrialCount { get; set; }
    }
}
