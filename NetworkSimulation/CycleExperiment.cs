using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkSimulation
{
    /// <summary>
    /// Runs a family of cycle-topology message-delay experiments over a range
    /// of graph sizes.
    /// 
    /// For each cycle size, this class executes repeated Monte Carlo trials
    /// using the diameter target node, aggregates summary statistics, and
    /// delegates output formatting to CycleExperimentReporter.
    /// </summary>
    public class CycleExperiment
    {
        private readonly TrialRunner runner;
        private readonly CycleExperimentReporter reporter;

        /// <summary>
        /// Initializes the cycle experiment with a trial runner and reporter.
        /// </summary>
        public CycleExperiment()
        {
            runner = new TrialRunner();
            reporter = new CycleExperimentReporter();
        }

        /// <summary>
        /// Executes cycle-topology simulations for graph sizes in the configured range.
        /// </summary>
        /// <param name="minOrder">Smallest cycle graph order to simulate.</param>
        /// <param name="maxOrder">Exclusive upper bound on cycle graph order.</param>
        /// <param name="nodeDelta">Increment between graph sizes.</param>
        /// <param name="numberSimulations">Number of successful trials per graph size.</param>
        /// <param name="numSessions">Number of ON sessions generated per node.</param>
        /// <param name="baseTime">Base time used for churn generation.</param>
        /// <param name="upDistro">Distribution for live/ON durations.</param>
        /// <param name="downDistro">Distribution for failed/OFF durations.</param>
        /// <remarks>
        /// Raw delay samples are written to msg_delays_cycle_N.txt, where N is
        /// the number of nodes in the cycle graph.
        /// </remarks>
        public void Run(
            int minOrder,
            int maxOrder,
            int nodeDelta,
            int numberSimulations,
            int numSessions,
            double baseTime,
            Distribution upDistro,
            Distribution downDistro,
            ExperimentRunMode runMode)
        {
            bool restart = runMode == ExperimentRunMode.Restart;

            if (restart)
            {
                reporter.ClearOutputFiles();
            }

            int simulationCount = numberSimulations;

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes += nodeDelta)
            {
                List<TrialResult> results = new List<TrialResult>();

                Console.WriteLine("Number of nodes: " + numNodes);

                string delayOutputFile = "msg_delays_cycle_" + numNodes.ToString() + ".txt";
                int completedSimulations = 0;

                if (runMode == ExperimentRunMode.Restart)
                {
                    System.IO.File.WriteAllText(delayOutputFile, "");

                    Console.WriteLine("Clearing delay file: {0}", delayOutputFile);
                }
                else if (System.IO.File.Exists(delayOutputFile))
                {
                    completedSimulations = System.IO.File.ReadLines(delayOutputFile).Count();

                    Console.WriteLine(
                        "Resuming delay file: {0}; completed simulations: {1}",
                        delayOutputFile,
                        completedSimulations);
                }

                if (completedSimulations >= simulationCount)
                {
                    Console.WriteLine(
                        "Skipping number of nodes {0}; already completed {1} simulations.",
                        numNodes,
                        completedSimulations);

                    continue;
                }


                for (int sim = completedSimulations; sim < simulationCount; sim++)
                {
                    Network network = TopologyFactory.CreateCycle(numNodes);

                    TrialResult result = runner.RunTrial(
                        network,
                        numNodes,
                        numSessions,
                        baseTime,
                        upDistro,
                        downDistro,
                        delayOutputFile,
                        MessageDelayMode.CycleDiameter);

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