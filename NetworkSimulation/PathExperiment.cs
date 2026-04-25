using System;
using System.Collections.Generic;

namespace NetworkSimulation
{
    public class PathExperiment
    {
        private readonly TrialRunner runner;

        public PathExperiment()
        {
            runner = new TrialRunner();
        }

        public void Run(int minOrder, int maxOrder, int numSessions, double baseTime, Distribution upDistro, Distribution downDistro)
        {
            int numSims = 100;

            int[] nValues = new int[maxOrder - minOrder];

            double[] connectivity = new double[maxOrder - minOrder];
            double[] liveTime = new double[maxOrder - minOrder];
            double[] avgMsgDelays = new double[maxOrder - minOrder];

            int index = 0;
            int totalSims = 0;

            PathExperimentReporter reporter = new PathExperimentReporter();
            reporter.ClearOutputFiles();

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes++)
            {
                List<TrialResult> results = new List<TrialResult>();
                nValues[index] = numNodes;

                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;
                liveTime[index] = 0.0;

                Console.WriteLine("Number of nodes: " + numNodes);
                totalSims = 0;

                for (int sim = 0; sim < numSims; sim++)
                {
                    Network network = TopologyFactory.CreatePath(numNodes);

                    TrialResult result = runner.RunTrial(
                        network,
                        numNodes,
                        numSessions,
                        baseTime,
                        upDistro,
                        downDistro,
                        "msg_delays_path_" + numNodes.ToString() + ".txt");

                    if (!result.Success)
                    {
                        sim--;
                        continue;
                    }

                    results.Add(result);

                    totalSims++;
                    Console.WriteLine("Simulation " + (sim + 1));
                }

                ResultSummary summary = ResultAggregator.Summarize(results);

                connectivity[index] = summary.ConnectivityPercent;
                avgMsgDelays[index] = summary.AverageMessageDelay;
                liveTime[index] = summary.AverageLivePercent;

                reporter.WriteSummary(numNodes, summary);

                index++;
            }
        }
    }
}
