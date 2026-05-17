using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkSimulation
{
    /// <summary>
    /// Runs a family of path-topology message-delay experiments over a range
    /// of graph sizes.
    /// 
    /// For each path size, this class executes repeated Monte Carlo trials,
    /// records raw delay samples, aggregates summary statistics, and delegates
    /// output formatting to PathExperimentReporter.
    /// </summary>
    public class PathExperiment
    {
        private readonly TrialRunner runner;
        private readonly PathExperimentReporter reporter;

        /// <summary>
        /// Initializes the path experiment with a trial runner and reporter.
        /// </summary>
        public PathExperiment()
        {
            runner = new TrialRunner();
            reporter = new PathExperimentReporter();
        }
                
        /// <summary>
        /// Executes path-topology simulations for graph sizes in the configured range.
        /// </summary>
        /// <param name="minOrder">Smallest path graph order to simulate.</param>
        /// <param name="maxOrder">Exclusive upper bound on path graph order.</param>
        /// <param name="nodeDelta">Increment between graph sizes.</param>
        /// <param name="numberSimulations">Number of successful trials per graph size.</param>
        /// <param name="numSessions">Number of ON sessions generated per node.</param>
        /// <param name="baseTime">Base time used for churn generation.</param>
        /// <param name="upDistro">Distribution for live/ON durations.</param>
        /// <param name="downDistro">Distribution for failed/OFF durations.</param>
        /// <remarks>
        /// Raw delay samples are written to msg_delays_path_N.txt, where N is
        /// the number of nodes in the path graph.
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
            // Delete raw delay files and derived statistic files.
            reporter.ClearOutputFiles();
            
            int simulationCount = numberSimulations;

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes += nodeDelta)
            {
                Console.WriteLine("Number of nodes: " + numNodes);

                string delayOutputFile = "msg_delays_path_" + numNodes.ToString() + ".txt";

                if (runMode == ExperimentRunMode.Restart)
                {
                    System.IO.File.WriteAllText(delayOutputFile, "");

                    Console.WriteLine("Clearing delay file: {0}", delayOutputFile);
                }

                List<TrialResult> results = LoadResultsFromDelayFile(delayOutputFile, numNodes);

                int completedSimulations = results.Count;

                Console.WriteLine(
                    "Running simulations for n={0}: {1} completed, {2} remaining.",
                    numNodes,
                    completedSimulations,
                    simulationCount - completedSimulations);

                for (int sim = completedSimulations; sim < simulationCount; sim++)
                {
                    Network network = TopologyFactory.CreatePath(numNodes);

                    TrialResult result =
                        runner.RunTrial(
                            network,
                            numNodes,
                            numSessions,
                            baseTime,
                            upDistro,
                            downDistro,
                            delayOutputFile,
                            MessageDelayMode.PathEndpoint);

                    if (!result.Success)
                    {
                        sim--;

                        Console.WriteLine("Simulation failed for n={0}; retrying simulation {1}.",
                            numNodes,
                            sim + 1);

                        continue;
                    }

                    results.Add(result);
                                  
                }

                ResultSummary summary = ResultAggregator.Summarize(results);

                Console.WriteLine("Summary statistics written for n={0}.", numNodes);
                reporter.WriteSummary(numNodes, summary);               
            }
        }

        private List<TrialResult> LoadResultsFromDelayFile(string delayOutputFile, int numNodes)
        {
            List<TrialResult> results = new List<TrialResult>();

            if (!System.IO.File.Exists(delayOutputFile))
            {
                return results;
            }

            foreach (string line in System.IO.File.ReadLines(delayOutputFile))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                double delay = Convert.ToDouble(line);

                TrialResult result = new TrialResult
                {
                    Success = true,
                    Delay = delay,
                    ZeroDelay = (delay == 0),
                    TotalNodes = numNodes,
                    PercentLive = 0.0,
                    AllNodesLive = false
                };

                results.Add(result);
            }

            return results;
        }
    }
}
