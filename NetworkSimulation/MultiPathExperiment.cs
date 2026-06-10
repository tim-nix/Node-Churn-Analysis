using System;
using System.Collections.Generic;

namespace NetworkSimulation
{
    /// <summary>
    /// Runs multi-path topology message-delay experiments for explicit
    /// collections of equal path lengths and path counts.
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
        /// Path lengths to simulate, where each value is the number of nodes
        /// in each internally disjoint path, including the shared endpoints.
        /// </param>
        /// <param name="pathCounts">
        /// Path counts to simulate, where each value is the number of
        /// internally disjoint equal-length paths between the shared endpoints.
        /// The current topology constructor requires each value to be at least
        /// two.
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
        /// <param name="randomSeed">
        /// Experiment seed used to derive deterministic per-trial streams.
        /// </param>
        /// <param name="maxAttempts">
        /// Maximum attempts allowed per successful trial.
        /// </param>
        public void Run(
            int[] pathLengths,
            int[] pathCounts,
            int numberSimulations,
            int numSessions,
            double baseTime,
            Distribution upDistro,
            Distribution downDistro,
            ExperimentRunMode runMode,
            int randomSeed = 12345,
            int maxAttempts = 3)
        {
            reporter.ClearOutputFiles();

            int simulationCount = numberSimulations;

            foreach (int pathLength in pathLengths)
            {
                foreach (int pathCount in pathCounts)
                {
                    int numNodes =
                        2 + pathCount * (pathLength - 2);

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

                    string metadata = string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "topology=multipath;pathNodes={0};count={1};seed={2}",
                        pathLength,
                        pathCount,
                        randomSeed);
                    TrialResultStore store =
                        new TrialResultStore(delayOutputFile, metadata);
                    List<TrialResult> results =
                        store.PrepareAndLoad(runMode);

                    int completedSimulations = results.Count;
                    if (completedSimulations > simulationCount)
                        throw new InvalidOperationException(
                            "Checkpoint contains more trials than requested.");

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
                        TrialResult result =
                            TrialAttempts.Run(
                                attempt => runner.RunTrial(
                                    TopologyFactory.CreateMultiPath(
                                        pathLength,
                                        pathCount),
                                    numNodes,
                                    numSessions,
                                    baseTime,
                                    upDistro.WithRandomSource(
                                        new MersenneTwister(RandomSeed.Derive(
                                            randomSeed,
                                            pathLength,
                                            pathCount,
                                            sim,
                                            attempt,
                                            0))),
                                    downDistro.WithRandomSource(
                                        new MersenneTwister(RandomSeed.Derive(
                                            randomSeed,
                                            pathLength,
                                            pathCount,
                                            sim,
                                            attempt,
                                            1))),
                                    MessageDelayMode.MultiPathEndpoint),
                                maxAttempts);

                        results.Add(result);
                        store.Append(result);
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

    }
}
