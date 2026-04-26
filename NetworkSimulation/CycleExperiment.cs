using System;
using System.Collections.Generic;

namespace NetworkSimulation
{
    public class CycleExperiment
    {
        private readonly TrialRunner runner;
        private readonly PathExperimentReporter reporter;

        public CycleExperiment()
        {
            runner = new TrialRunner();
            reporter = new PathExperimentReporter();
        }

        public void Run(int minOrder, int maxOrder, int numberSimulations, int numSessions, double baseTime, Distribution upDistro, Distribution downDistro)
        {
            reporter.ClearOutputFiles();

            int simulationCount = numberSimulations;

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes++)
            {
                List<TrialResult> results = new List<TrialResult>();

                Console.WriteLine("Number of nodes: " + numNodes);

                for (int sim = 0; sim < simulationCount; sim++)
                {
                    Network network = TopologyFactory.CreateCycle(numNodes);

                    TrialResult result = runner.RunTrial(
                        network,
                        numNodes,
                        numSessions,
                        baseTime,
                        upDistro,
                        downDistro,
                        "msg_delays_cycle_" + numNodes.ToString() + ".txt");

                    if (!result.Success)
                    {
                        sim--;
                        continue;
                    }

                    results.Add(result);
                }

                ResultSummary summary = ResultAggregator.Summarize(results);

                reporter.WriteSummary(numNodes, summary);
            }
        }
    }
}