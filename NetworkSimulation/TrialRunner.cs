using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class TrialRunner
    {
        public TrialResult RunPathTrial(Network network, NetworkChurn churn, double baseTime)
        {
            double time = baseTime + 25.0;
            double delay = 0.0;
            double percentLive = 0.0;
            int numLive = 0;

            bool[] status = churn.getStatusAtTime(time);

            if (!status[0])
            {
                time = churn.getNextStartTimeForNode(0, time);
                status = churn.getStatusAtTime(time);
            }

            for (int i = 0; i < status.Length; i++)
            {
                if (status[i])
                {
                    numLive++;
                }
            }

            percentLive = (double)numLive / status.Length;

            network.updateStatus(status);

            Message msg = new Message(network, churn, time);
            delay = msg.getPathMessageDelay();

            return new TrialResult
            {
                Delay = delay,
                NumLive = numLive,
                PercentLive = percentLive,
                StartTime = time,
                Success = true
            };
        }
    }
}
