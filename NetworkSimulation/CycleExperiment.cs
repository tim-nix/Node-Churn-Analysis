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
        /// Raw delay samples are written to msg_delays_cycle_N.txt. Complete
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

                string delayOutputFile = "msg_delays_cycle_" + numNodes.ToString() + ".txt";

                string metadata = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "topology=cycle;nodes={0};seed={1}",
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
                                TopologyFactory.CreateCycle(numNodes),
                                numNodes,
                                numSessions,
                                baseTime,
                                upDistro.WithRandomSource(
                                    new MersenneTwister(RandomSeed.Derive(
                                        randomSeed, numNodes, 1, sim, attempt, 0))),
                                downDistro.WithRandomSource(
                                    new MersenneTwister(RandomSeed.Derive(
                                        randomSeed, numNodes, 1, sim, attempt, 1))),
                                MessageDelayMode.CycleDiameter),
                            maxAttempts);

                    results.Add(result);
                    store.Append(result);
                }

                Console.WriteLine("Finished simulations for n={0}. Recomputing summary statistics.", numNodes);

                ResultSummary summary = ResultAggregator.Summarize(results);

                Console.WriteLine("Summary statistics written for n={0}.", numNodes);
                reporter.WriteSummary(numNodes, summary);                                
            }
        }

    }
}
