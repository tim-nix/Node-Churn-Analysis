using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class TrialRunner
    {
        private double GetValidStartTime(NetworkChurn churn, double time)
        {
            bool[] status = churn.getStatusAtTime(time);

            if (!status[0])
            {
                time = churn.getNextStartTimeForNode(0, time);
            }

            return time;
        }
        

        public TrialResult RunTrial(
            Network network,
            int numNodes,
            int numSessions,
            double baseTime,
            Distribution upDistro,
            Distribution downDistro,
            string delayOutputFile)
        {
            NetworkChurn netChurn = new NetworkChurn(numNodes);
            netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);

            double time = GetValidStartTime(netChurn, baseTime + 25.0);
            bool[] status = netChurn.getStatusAtTime(time);

            double delay = 0.0;
            double percentLive = 0.0;
            int numLive = 0;

            for (int i = 0; i < status.Length; i++)
            {
                if (status[i])
                    numLive++;
            }

            percentLive = (numLive / Convert.ToDouble(status.Length)) * 100.0;

            network.updateStatus(status);

            Message msg = new Message(network, netChurn, time);

            try
            {
                delay = msg.getPathMessageDelay();

                System.IO.File.AppendAllText(
                    delayOutputFile,
                    delay.ToString() + Environment.NewLine);
            }
            catch
            {
                return new TrialResult
                {
                    Delay = 0.0,
                    NumLive = 0,
                    PercentLive = 0.0,
                    StartTime = time,
                    Success = false,
                    Connected = false
                };
            }

            return new TrialResult
            {
                Delay = delay,
                NumLive = numLive,
                PercentLive = percentLive,
                StartTime = time,
                Success = true,
                Connected = network.isCurrentNetworkConnected()
            };
        }
    }
}
