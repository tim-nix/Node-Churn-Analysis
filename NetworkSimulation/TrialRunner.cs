using System;
using System.Linq;

namespace NetworkSimulation
{
    /// <summary>
    /// Runs one Monte Carlo message-delivery trial on a supplied topology
    /// under generated node churn.
    /// 
    /// The runner creates a fresh NetworkChurn instance, advances to a valid
    /// source start time, updates the network status, computes message delay,
    /// appends the raw delay to an output file, and returns a TrialResult
    /// summarizing the trial.
    /// </summary>
    public class TrialRunner
    {
        /// <summary>
        /// Adjusts the requested start time so that source node 0 is live.
        /// </summary>
        /// <param name="churn">
        /// Generated churn process for the current trial.
        /// </param>
        /// <param name="time">
        /// Candidate message-generation time.
        /// </param>
        /// <returns>
        /// The original time if node 0 is live; otherwise, the next time at which
        /// node 0 becomes live.
        /// </returns>
        private double GetValidStartTime(NetworkChurn churn, double time)
        {
            bool[] status = churn.getStatusAtTime(time);

            if (!status[0])
            {
                time = churn.getNextStartTimeForNode(0, time);
            }

            return time;
        }

        /// <summary>
        /// Runs a single message-delay simulation trial and records the result.
        /// </summary>
        /// <param name="network">
        /// Network topology used for the trial.
        /// </param>
        /// <param name="numNodes">
        /// Number of nodes in the topology.
        /// </param>
        /// <param name="numSessions">
        /// Initial number of ON sessions generated per node. Timelines extend
        /// lazily if message delivery requires later cycles.
        /// </param>
        /// <param name="baseTime">
        /// Base time used when generating node churn.
        /// </param>
        /// <param name="upDistro">
        /// Distribution used to generate live/ON durations.
        /// </param>
        /// <param name="downDistro">
        /// Distribution used to generate failed/OFF durations.
        /// </param>
        /// <param name="delayOutputFile">
        /// File to which the raw message delay is appended.
        /// </param>
        /// <param name="delayMode">
        /// Message-delay mode specifying the source-target interpretation.
        /// </param>
        /// <returns>
        /// A TrialResult containing delay, live-node percentage, start time,
        /// success status, zero-delay status, and all-nodes-live status.
        /// </returns>
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
                delay = msg.getMessageDelay(delayMode);

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
                    TotalNodes = numNodes,
                    PercentLive = 0.0,
                    StartTime = time,
                    Success = false,
                    ZeroDelay = false,
                    AllNodesLive = false
                };
            }

            return new TrialResult
            {
                Delay = delay,
                NumLive = numLive,
                TotalNodes = numNodes,
                PercentLive = percentLive,
                StartTime = time,
                Success = true,
                ZeroDelay = Math.Abs(delay) < 1e-9,
                AllNodesLive = numLive == numNodes
            };
        }
    }
}
