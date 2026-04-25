using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class PathExperiment
    {
        private readonly TrialRunner runner;

        public PathExperiment()
        {
            runner = new TrialRunner();
        }

        public void Run(
    int minOrder,
    int maxOrder,
    int numSessions,
    double baseTime,
    Distribution upDistro,
    Distribution downDistro)
        {
            int numSims = 100;

            int[] nValues = new int[maxOrder - minOrder];

            double[] connectivity = new double[maxOrder - minOrder];
            double[] liveTime = new double[maxOrder - minOrder];
            double[] avgMsgDelays = new double[maxOrder - minOrder];

            int index = 0;
            int totalSims = 0;

            System.IO.File.WriteAllText("graph_sizes_path.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_path.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_path.txt", "");
            System.IO.File.WriteAllText("avg_up_time_path.txt", "");

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes++)
            {
                nValues[index] = numNodes;

                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;
                liveTime[index] = 0.0;

                Console.WriteLine("Number of nodes: " + numNodes);
                totalSims = 0;

                for (int sim = 0; sim < numSims; sim++)
                {
                    TrialResult result = runner.RunPathTrial(
                        numNodes,
                        numSessions,
                        baseTime,
                        upDistro,
                        downDistro);

                    if (!result.Success)
                    {
                        sim--;
                        continue;
                    }

                    if (result.Connected)
                        connectivity[index]++;

                    avgMsgDelays[index] += result.Delay;
                    liveTime[index] += result.PercentLive;

                    totalSims++;
                    Console.WriteLine("Simulation " + (sim + 1));
                }

                connectivity[index] = (connectivity[index] / Convert.ToDouble(totalSims)) * 100.0;
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(totalSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(totalSims);

                Console.WriteLine(
                    "Path graph family with {0} nodes is connected {1:N2}% of the time.",
                    nValues[index],
                    connectivity[index]);

                Console.WriteLine(
                    "The average message delay between two end nodes is {0:N4}.",
                    avgMsgDelays[index]);

                Console.WriteLine(
                    "On average, {0:N2}% nodes are live at any given time",
                    liveTime[index]);

                Console.WriteLine();

                System.IO.File.AppendAllText(
                    "graph_sizes_path.txt",
                    nValues[index].ToString() + Environment.NewLine);

                System.IO.File.AppendAllText(
                    "avg_connectivity_path.txt",
                    connectivity[index].ToString() + Environment.NewLine);

                System.IO.File.AppendAllText(
                    "avg_msg_delays_path.txt",
                    avgMsgDelays[index].ToString() + Environment.NewLine);

                System.IO.File.AppendAllText(
                    "avg_up_time_path.txt",
                    liveTime[index].ToString() + Environment.NewLine);

                index++;
            }
        }
    }
}
