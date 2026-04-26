using System;

namespace NetworkSimulation
{
    public class CycleExperimentReporter
    {
        public void ClearOutputFiles()
        {
            System.IO.File.WriteAllText("graph_sizes_cycle.txt", "");
            System.IO.File.WriteAllText("zero_delay_percent_cycle.txt", "");
            System.IO.File.WriteAllText("redundancy_gain_cycle.txt", "");
            System.IO.File.WriteAllText("all_nodes_live_percent_cycle.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_cycle.txt", "");
            System.IO.File.WriteAllText("avg_up_time_cycle.txt", "");
        }

        public void WriteSummary(int numNodes, ResultSummary summary)
        {
            Console.WriteLine("Cycle graph family with {0} nodes has zero message delay {1:N2}% of the time.",
                numNodes, summary.ZeroDelayPercent);

            Console.WriteLine("Cycle graph family with {0} nodes has all nodes live {1:N2}% of the time.",
                numNodes, summary.AllNodesLivePercent);

            Console.WriteLine("Redundancy gain from topology is {0:N2} percentage points.",
                summary.RedundancyGainPercent);

            Console.WriteLine("The average message delay to the diameter target node is {0:N4}.",
                summary.AverageMessageDelay);

            Console.WriteLine("On average, {0:N2}% nodes are live at any given time",
                summary.AverageLivePercent);

            Console.WriteLine();

            System.IO.File.AppendAllText("graph_sizes_cycle.txt",
                numNodes.ToString() + Environment.NewLine);

            System.IO.File.AppendAllText("zero_delay_percent_cycle.txt",
                summary.ZeroDelayPercent.ToString() + Environment.NewLine);

            System.IO.File.AppendAllText("redundancy_gain_cycle.txt",
                summary.RedundancyGainPercent.ToString() + Environment.NewLine);

            System.IO.File.AppendAllText("all_nodes_live_percent_cycle.txt",
                summary.AllNodesLivePercent.ToString() + Environment.NewLine);

            System.IO.File.AppendAllText("avg_msg_delays_cycle.txt",
                summary.AverageMessageDelay.ToString() + Environment.NewLine);

            System.IO.File.AppendAllText("avg_up_time_cycle.txt",
                summary.AverageLivePercent.ToString() + Environment.NewLine);
        }
    }
}