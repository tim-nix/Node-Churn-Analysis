using System;
using System.Collections.Generic;

namespace NetworkSimulation
{
    /// <summary>
    /// Runs a family of multi-path topology message-delay experiments over
    /// a range of equal path lengths and path counts.
    ///
    /// For each path length and path count, this class executes repeated
    /// Monte Carlo trials using shared endpoint nodes, records raw delay
    /// samples, aggregates summary statistics, and delegates output
    /// formatting to MultiPathExperimentReporter.
    /// </summary>
    public class MultiPathExperiment
    {
        private readonly TrialRunner runner;
        private readonly MultiPathExperimentReporter reporter;

        /// <summary>
        /// Initializes the multi-path experiment with a trial runner and reporter.
        /// </summary>
        public MultiPathExperiment()
        {
            runner = new TrialRunner();
            reporter = new MultiPathExperimentReporter();
        }

        /// <summary>
        /// Executes multi-path topology simulations for each configured path
        /// length and path count.
        /// </summary>
        /// <param name="pathLengths">
        /// Path lengths to simulate, where each value is the number of edges
        /// in each internally disjoint path.
        /// </param>
        /// <param name="pathCounts">
        /// Path counts to simulate, where each value is the number of
        /// internally disjoint equal-length paths between the shared endpoints.
        /// </param>
        /// <param name="numberSimulations">
        /// Number of successful trials per topology configuration.
        /// </param>
        /// <param name="numSessions">
        /// Initial ON sessions generated per node; timelines extend as needed.
        /// </param>
        /// <param name="baseTime">
        /// Base time used for churn generation.
        /// </param>
        /// <param name="upDistro">
        /// Distribution for live/ON durations.
        /// </param>
        /// <param name="downDistro">
        /// Distribution for failed/OFF durations.
        /// </param>
        /// <param name="runMode">
        /// Indicates whether raw delay files should be restarted or resumed.
        /// </param>
        public void Run(
            int[] pathLengths,
            int[] pathCounts,
            int numberSimulations,
            int numSessions,
            double baseTime,
            Distribution upDistro,
            Distribution downDistro,
            ExperimentRunMode runMode)
        {
            reporter.ClearOutputFiles();

            int simulationCount = numberSimulations;

            foreach (int pathLength in pathLengths)
            {
                foreach (int pathCount in pathCounts)
                {
                    int numNodes =
                        2 + pathCount * (pathLength - 1);

                    Console.WriteLine(
                        "Multi-path topology: path length = {0}, path count = {1}, nodes = {2}",
                        pathLength,
                        pathCount,
                        numNodes);

                    string delayOutputFile =
                        "msg_delays_multipath_k"
                        + pathCount.ToString()
                        + "_len"
                        + pathLength.ToString()
                        + ".txt";

                    if (runMode == ExperimentRunMode.Restart)
                    {
                        System.IO.File.WriteAllText(delayOutputFile, "");
                        Console.WriteLine(
                            "Clearing delay file: {0}",
                            delayOutputFile);
                    }

                    List<TrialResult> results =
                        LoadResultsFromDelayFile(delayOutputFile, numNodes);

                    int completedSimulations = results.Count;

                    Console.WriteLine(
                        "Running simulations for k={0}, length={1}: {2} completed, {3} remaining.",
                        pathCount,
                        pathLength,
                        completedSimulations,
                        simulationCount - completedSimulations);

                    for (int sim = completedSimulations;
                         sim < simulationCount;
                         sim++)
                    {
                        Network network =
                            TopologyFactory.CreateMultiPath(
                                pathLength,
                                pathCount);

                        TrialResult result =
                            runner.RunTrial(
                                network,
                                numNodes,
                                numSessions,
                                baseTime,
                                upDistro,
                                downDistro,
                                delayOutputFile,
                                MessageDelayMode.MultiPathEndpoint);

                        if (!result.Success)
                        {
                            sim--;

                            Console.WriteLine(
                                "Simulation failed for k={0}, length={1}; retrying simulation {2}.",
                                pathCount,
                                pathLength,
                                sim + 1);

                            continue;
                        }

                        results.Add(result);
                    }

                    ResultSummary summary =
                        ResultAggregator.Summarize(results);

                    Console.WriteLine(
                        "Summary statistics written for k={0}, length={1}.",
                        pathCount,
                        pathLength);

                    reporter.WriteSummary(
                        pathLength,
                        pathCount,
                        numNodes,
                        summary);
                }
            }
        }

        private List<TrialResult> LoadResultsFromDelayFile(
            string delayOutputFile,
            int numNodes)
        {
            List<TrialResult> results =
                new List<TrialResult>();

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

                double delay =
                    Convert.ToDouble(line);

                TrialResult result =
                    new TrialResult
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
