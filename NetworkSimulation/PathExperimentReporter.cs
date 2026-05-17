using System;

namespace NetworkSimulation
{
    /// <summary>
    /// Writes path-experiment summary statistics to the console and to
    /// path-specific output files.
    /// </summary>
    public class PathExperimentReporter
    {
        /// <summary>
        /// Clears path summary output files before a new experiment run.
        /// </summary>
        public void ClearOutputFiles()
        {
            System.IO.File.WriteAllText("graph_sizes_path.txt", "");
            System.IO.File.WriteAllText("zero_delay_percent_path.txt", "");
            System.IO.File.WriteAllText("all_nodes_live_percent_path.txt", "");
            System.IO.File.WriteAllText("redundancy_gain_path.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_path.txt", "");
            System.IO.File.WriteAllText("avg_up_time_path.txt", "");
        }
                

        /// <summary>
        /// Writes summary statistics for one path graph size.
        /// </summary>
        /// <param name="numNodes">Number of nodes in the path graph.</param>
        /// <param name="summary">Aggregated trial statistics for this graph size.</param>
        /// <remarks>
        /// Appends graph size, zero-delay percentage, all-nodes-live percentage,
        /// redundancy gain, average message delay, and average live-node percentage
        /// to the path output files.
        /// </remarks>
        public void WriteSummary(int numNodes, ResultSummary summary)
        {
            Console.WriteLine("Path graph family with {0} nodes has zero message delay {1:N2}% of the time.",
                numNodes, summary.ZeroDelayPercent);

            Console.WriteLine("Path graph family with {0} nodes has all nodes live {1:N2}% of the time.",
                numNodes, summary.AllNodesLivePercent);

            Console.WriteLine("Redundancy gain from topology is {0:N2} percentage points.",
                summary.RedundancyGainPercent);

            Console.WriteLine("The average message delay between two end nodes is {0:N4}.",
                summary.AverageMessageDelay);

            Console.WriteLine("On average, {0:N2}% nodes are live at any given time",
                summary.AverageLivePercent);

            Console.WriteLine();

            System.IO.File.AppendAllText("graph_sizes_path.txt",
                numNodes.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("zero_delay_percent_path.txt",
                summary.ZeroDelayPercent.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("all_nodes_live_percent_path.txt", 
                summary.AllNodesLivePercent.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("redundancy_gain_path.txt",
                summary.RedundancyGainPercent.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_msg_delays_path.txt", 
                summary.AverageMessageDelay.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_up_time_path.txt", 
                summary.AverageLivePercent.ToString() + Environment.NewLine);
        }
    }
}
