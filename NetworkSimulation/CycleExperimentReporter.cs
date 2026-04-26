using System;

namespace NetworkSimulation
{
    public class CycleExperimentReporter
    {
        public void ClearOutputFiles()
        {
            System.IO.File.WriteAllText("graph_sizes_cycle.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_cycle.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_cycle.txt", "");
            System.IO.File.WriteAllText("avg_up_time_cycle.txt", "");
        }

        public void WriteSummary(int numNodes, ResultSummary summary)
        {
            Console.WriteLine("Cycle graph family with {0} nodes is connected {1:N2}% of the time.",
                numNodes, summary.ZeroDelayPercent);

            Console.WriteLine("The average message delay between two end nodes is {0:N4}.",
                summary.AverageMessageDelay);

            Console.WriteLine("On average, {0:N2}% nodes are live at any given time",
                summary.AverageLivePercent);

            Console.WriteLine();

            System.IO.File.AppendAllText("graph_sizes_cycle.txt", numNodes.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_connectivity_cycle.txt", summary.ZeroDelayPercent.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_msg_delays_cycle.txt", summary.AverageMessageDelay.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_up_time_cycle.txt", summary.AverageLivePercent.ToString() + Environment.NewLine);
        }
    }
}