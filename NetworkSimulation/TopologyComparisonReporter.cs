using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NetworkSimulation
{
    /// <summary>
    /// Compares path and cycle mean-delay output files and computes absolute
    /// and percentage delay reduction.
    /// </summary>
    public class TopologyComparisonReporter
    {
        /// <summary>
        /// Reads path and cycle summary files, matches path size N against cycle
        /// size 2(N-1), and writes delay-reduction outputs.
        /// </summary>
        /// <remarks>
        /// Outputs absolute delay reductions to delay_reduction_path_vs_cycle.txt
        /// and percentage reductions to delay_reduction_percent_path_vs_cycle.txt.
        /// Rows whose cycle diameter route does not match the path length are
        /// skipped.
        /// </remarks>
        public void ComparePathAndCycle()
        {
            string[] pathSizes = File.ReadAllLines("graph_sizes_path.txt");
            string[] cycleSizes = File.ReadAllLines("graph_sizes_cycle.txt");

            string[] pathDelays = File.ReadAllLines("avg_msg_delays_path.txt");
            string[] cycleDelays = File.ReadAllLines("avg_msg_delays_cycle.txt");

            File.WriteAllText("delay_reduction_path_vs_cycle.txt", "");
            File.WriteAllText("delay_reduction_percent_path_vs_cycle.txt", "");

            int count = Math.Min(pathDelays.Length, cycleDelays.Length);

            for (int i = 0; i < count; i++)
            {
                int pathN = int.Parse(pathSizes[i]);
                int cycleN = int.Parse(cycleSizes[i]);

                if (cycleN !=
                    TopologyFactory.GetEquivalentCycleOrder(pathN))
                {
                    Console.WriteLine(
                        "Skipping comparison at row {0}: path n={1}, cycle n={2}",
                        i, pathN, cycleN);
                    continue;
                }

                double pathDelay = double.Parse(pathDelays[i], CultureInfo.InvariantCulture);
                double cycleDelay = double.Parse(cycleDelays[i], CultureInfo.InvariantCulture);

                double reduction = pathDelay - cycleDelay;

                double reductionPercent = 0.0;
                if (pathDelay > 0.0)
                {
                    reductionPercent = (reduction / pathDelay) * 100.0;
                }

                Console.WriteLine(
                    "path n={0} vs cycle n={1}: path delay={2:N4}, cycle delay={3:N4}, reduction={4:N4}, reduction={5:N2}%",
                    pathN, cycleN, pathDelay, cycleDelay, reduction, reductionPercent);

                File.AppendAllText(
                    "delay_reduction_path_vs_cycle.txt",
                    reduction.ToString(CultureInfo.InvariantCulture) + Environment.NewLine);

                File.AppendAllText(
                    "delay_reduction_percent_path_vs_cycle.txt",
                    reductionPercent.ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
            }
        }
    }
}
