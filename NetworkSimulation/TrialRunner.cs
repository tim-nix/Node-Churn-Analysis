using System;
using System.Linq;

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
            string delayOutputFile,
            MessageDelayMode delayMode)
        {
            NetworkChurn netChurn = new NetworkChurn(numNodes);
            netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);

            double time = GetValidStartTime(netChurn, baseTime + 25.0);
            bool[] status = netChurn.getStatusAtTime(time);

            int numLive = status.Count(isLive => isLive);
            double percentLive = (numLive / Convert.ToDouble(status.Length)) * 100.0;

            network.updateStatus(status);

            Message msg = new Message(network, netChurn, time);
            double delay = 0.0;

            try
            {
                switch (delayMode)
                {
                    case MessageDelayMode.PathEndpoint:
                        delay = msg.getPathMessageDelay();
                        break;

                    case MessageDelayMode.CycleDiameter:
                        delay = msg.getCycleMessageDelay();
                        break;

                    default:
                        throw new ArgumentException("Unsupported message delay mode.");
                }

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
