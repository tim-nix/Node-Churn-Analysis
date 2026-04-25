using System;

namespace NetworkSimulation
{
    public class PathExperimentReporter
    {
        public void ClearOutputFiles()
        {
            System.IO.File.WriteAllText("graph_sizes_path.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_path.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_path.txt", "");
            System.IO.File.WriteAllText("avg_up_time_path.txt", "");
        }

        public void WriteSummary(int numNodes, ResultSummary summary)
        {
            Console.WriteLine("Path graph family with {0} nodes is connected {1:N2}% of the time.",
                numNodes, summary.ConnectivityPercent);

            Console.WriteLine("The average message delay between two end nodes is {0:N4}.",
                summary.AverageMessageDelay);

            Console.WriteLine("On average, {0:N2}% nodes are live at any given time",
                summary.AverageLivePercent);

            Console.WriteLine();

            System.IO.File.AppendAllText("graph_sizes_path.txt", numNodes.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_connectivity_path.txt", summary.ConnectivityPercent.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_msg_delays_path.txt", summary.AverageMessageDelay.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText("avg_up_time_path.txt", summary.AverageLivePercent.ToString() + Environment.NewLine);
        }
    }
}
