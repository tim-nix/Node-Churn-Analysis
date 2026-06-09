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
        /// <param name="numSessions">
        /// Initial ON sessions generated per node; timelines extend as needed.
        /// </param>
        /// <param name="baseTime">Base time used for churn generation.</param>
        /// <param name="upDistro">Distribution for live/ON durations.</param>
        /// <param name="downDistro">Distribution for failed/OFF durations.</param>
        /// <param name="runMode">Controls checkpoint restart or resume behavior.</param>
        /// <param name="randomSeed">Experiment seed used for deterministic trials.</param>
        /// <param name="maxAttempts">Maximum attempts allowed per successful trial.</param>
        /// <remarks>
        /// Raw delay samples are written to msg_delays_path_N.txt. Complete
        /// trial checkpoints and seed metadata are written beside that file.
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
            ExperimentRunMode runMode,
            int randomSeed = 12345,
            int maxAttempts = 3)
        {
            // Delete raw delay files and derived statistic files.
            reporter.ClearOutputFiles();
            
            int simulationCount = numberSimulations;

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes += nodeDelta)
            {
                Console.WriteLine("Number of nodes: " + numNodes);

                string delayOutputFile = "msg_delays_path_" + numNodes.ToString() + ".txt";

                string metadata = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "topology=path;nodes={0};seed={1}",
                    numNodes,
                    randomSeed);
                TrialResultStore store =
                    new TrialResultStore(delayOutputFile, metadata);
                List<TrialResult> results = store.PrepareAndLoad(runMode);

                int completedSimulations = results.Count;
                if (completedSimulations > simulationCount)
                    throw new InvalidOperationException(
                        "Checkpoint contains more trials than requested.");

                Console.WriteLine(
                    "Running simulations for n={0}: {1} completed, {2} remaining.",
                    numNodes,
                    completedSimulations,
                    simulationCount - completedSimulations);

                for (int sim = completedSimulations; sim < simulationCount; sim++)
                {
                    TrialResult result =
                        TrialAttempts.Run(
                            attempt => runner.RunTrial(
                                TopologyFactory.CreatePath(numNodes),
                                numNodes,
                                numSessions,
                                baseTime,
                                upDistro.WithRandomSource(
                                    new MersenneTwister(RandomSeed.Derive(
                                        randomSeed, numNodes, 0, sim, attempt, 0))),
                                downDistro.WithRandomSource(
                                    new MersenneTwister(RandomSeed.Derive(
                                        randomSeed, numNodes, 0, sim, attempt, 1))),
                                MessageDelayMode.PathEndpoint),
                            maxAttempts);

                    results.Add(result);
                    store.Append(result);
                                  
                }

                ResultSummary summary = ResultAggregator.Summarize(results);

                Console.WriteLine("Summary statistics written for n={0}.", numNodes);
                reporter.WriteSummary(numNodes, summary);               
            }
        }

    }
}
