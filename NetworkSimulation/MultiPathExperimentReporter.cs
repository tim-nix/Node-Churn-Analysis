using System;

namespace NetworkSimulation
{
    /// <summary>
    /// Writes multi-path experiment summary statistics to the console and
    /// to multi-path-specific output files.
    /// </summary>
    public class MultiPathExperimentReporter
    {
        /// <summary>
        /// Clears multi-path summary output files before a new experiment run.
        /// </summary>
        public void ClearOutputFiles()
        {
            System.IO.File.WriteAllText("path_lengths_multipath.txt", "");
            System.IO.File.WriteAllText("path_counts_multipath.txt", "");
            System.IO.File.WriteAllText("graph_sizes_multipath.txt", "");
            System.IO.File.WriteAllText("zero_delay_percent_multipath.txt", "");
            System.IO.File.WriteAllText("all_nodes_live_percent_multipath.txt", "");
            System.IO.File.WriteAllText("redundancy_gain_multipath.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_multipath.txt", "");
            System.IO.File.WriteAllText("avg_up_time_multipath.txt", "");
        }

        /// <summary>
        /// Writes summary statistics for one multi-path topology configuration.
        /// </summary>
        /// <param name="pathLength">
        /// Number of nodes in each internally disjoint path, including the
        /// shared source and destination nodes.
        /// </param>
        /// <param name="pathCount">
        /// Number of internally disjoint equal-length paths between the
        /// shared source and destination nodes.
        /// </param>
        /// <param name="numNodes">
        /// Number of nodes in the generated multi-path graph.
        /// </param>
        /// <param name="summary">
        /// Aggregated trial statistics for this topology configuration.
        /// </param>
        public void WriteSummary(
            int pathLength,
            int pathCount,
            int numNodes,
            ResultSummary summary)
        {
            Console.WriteLine(
                "Multi-path graph family with path length {0}, path count {1}, and {2} nodes has zero message delay {3:N2}% of the time.",
                pathLength,
                pathCount,
                numNodes,
                summary.ZeroDelayPercent);

            Console.WriteLine(
                "Multi-path graph family with path length {0}, path count {1}, and {2} nodes has all nodes live {3:N2}% of the time.",
                pathLength,
                pathCount,
                numNodes,
                summary.AllNodesLivePercent);

            Console.WriteLine(
                "Redundancy gain from topology is {0:N2} percentage points.",
                summary.RedundancyGainPercent);

            Console.WriteLine(
                "The average message delay between the shared endpoint nodes is {0:N4}.",
                summary.AverageMessageDelay);

            Console.WriteLine(
                "On average, {0:N2}% nodes are live at any given time",
                summary.AverageLivePercent);

            Console.WriteLine();

            System.IO.File.AppendAllText(
                "path_lengths_multipath.txt",
                pathLength.ToString(System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);

            System.IO.File.AppendAllText(
                "path_counts_multipath.txt",
                pathCount.ToString(System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);

            System.IO.File.AppendAllText(
                "graph_sizes_multipath.txt",
                numNodes.ToString(System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);

            System.IO.File.AppendAllText(
                "zero_delay_percent_multipath.txt",
                summary.ZeroDelayPercent.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);

            System.IO.File.AppendAllText(
                "all_nodes_live_percent_multipath.txt",
                summary.AllNodesLivePercent.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);

            System.IO.File.AppendAllText(
                "redundancy_gain_multipath.txt",
                summary.RedundancyGainPercent.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);

            System.IO.File.AppendAllText(
                "avg_msg_delays_multipath.txt",
                summary.AverageMessageDelay.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);

            System.IO.File.AppendAllText(
                "avg_up_time_multipath.txt",
                summary.AverageLivePercent.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine);
        }
    }
}
