using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkSimulation
{
    /// <summary>
    /// Runs cycle-topology message-delay experiments for either an arithmetic
    /// range or an explicit collection of cycle orders.
    /// 
    /// For each cycle size, this class executes repeated Monte Carlo trials
    /// from node 0 to node floor(N / 2), aggregates summary statistics, and
    /// delegates output formatting to CycleExperimentReporter. The endpoints
    /// are exactly opposite when N is even.
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
        /// Executes cycle-topology simulations for an arithmetic range of
        /// cycle orders.
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
        /// Delivery is measured from node 0 to node floor(N / 2), which is
        /// exactly opposite node 0 when N is even.
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
            Run(
                Enumerable.Range(minOrder, maxOrder - minOrder)
                    .Where(order => (order - minOrder) % nodeDelta == 0),
                numberSimulations,
                numSessions,
                baseTime,
                upDistro,
                downDistro,
                runMode,
                randomSeed,
                maxAttempts);
        }

        /// <summary>
        /// Executes cycle-topology simulations for explicit graph orders.
        /// </summary>
        /// <param name="graphOrders">Numbers of nodes in the cycle graphs.</param>
        /// <param name="numberSimulations">Number of successful trials per cycle order.</param>
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
        /// Raw delay samples are written to msg_delays_cycle_N.txt, where N is
        /// the supplied cycle order. Delivery is measured from node 0 to node
        /// floor(N / 2), which is exactly opposite node 0 when N is even.
        /// </remarks>
        public void Run(
            IEnumerable<int> graphOrders,
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

            foreach (int numNodes in graphOrders)
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
