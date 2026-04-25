using System;
using System.Collections.Generic;

namespace NetworkSimulation
{
    public class PathExperiment
    {
        private const int DefaultSimulationCount = 100;

        private readonly TrialRunner runner;
        private readonly PathExperimentReporter reporter;

        public PathExperiment()
        {
            runner = new TrialRunner();
            reporter = new PathExperimentReporter();
        }

        public void Run(int minOrder, int maxOrder, int numSessions, double baseTime, Distribution upDistro, Distribution downDistro)
        {
            reporter.ClearOutputFiles();

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes++)
            {
                List<TrialResult> results = new List<TrialResult>();

                Console.WriteLine("Number of nodes: " + numNodes);

                for (int sim = 0; sim < DefaultSimulationCount; sim++)
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

                    Console.WriteLine("Simulation " + (sim + 1));
                }

                ResultSummary summary = ResultAggregator.Summarize(results);

                reporter.WriteSummary(numNodes, summary);
            }
        }
    }
}
