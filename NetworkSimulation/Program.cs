using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetworkSimulation
{
    class Program
    {
        /// <summary>
        /// Runs the configured path-topology experiment using exponential ON/OFF
        /// churn distributions.
        /// </summary>
        /// <remarks>
        /// Current parameters use path sizes 5, 10, 15, and 20; exponential ON
        /// rate 2.0; and exponential OFF/recovery rate 3.0. The number of
        /// simulations per size is supplied by the caller.
        /// </remarks>
        /// <param name="runMode">
        /// Controls whether existing raw samples are cleared or resumed.
        /// </param>
        /// <param name="numberSimulations">
        /// Number of successful simulations to run per path size.
        /// </param>
        /// <param name="randomSeed">
        /// Seed used to derive deterministic random streams for each trial.
        /// </param>
        public static void pathExponential(
            ExperimentRunMode runMode,
            int numberSimulations,
            int randomSeed)
        {
            Simulations sim = new Simulations(
                minN: 5,
                maxN: 21,
                nDelta: 5,
                numSims: numberSimulations);

            Distribution upDistro = new Exponential(2.0);
            Distribution downDistro = new Exponential(3.0);

            sim.setUpDistro(upDistro, downDistro);

            sim.simPath(runMode, randomSeed);
        }

        /// <summary>
        /// Runs the configured cycle-topology experiment using exponential ON/OFF
        /// churn distributions.
        /// </summary>
        /// <remarks>
        /// Current parameters use even cycle sizes from 10 through 30,
        /// exponential ON/OFF rates of 1.0, and caller-supplied simulation
        /// count and random seed values.
        /// </remarks>
        /// <param name="runMode">
        /// Controls whether existing raw samples are cleared or resumed.
        /// </param>
        /// <param name="numberSimulations">
        /// Number of successful simulations to run per cycle size.
        /// </param>
        /// <param name="randomSeed">
        /// Seed used to derive deterministic random streams for each trial.
        /// </param>
        public static void cycleExponential(
            ExperimentRunMode runMode,
            int numberSimulations,
            int randomSeed)
        {
            Simulations sim = new Simulations(
                minN: 10,
                maxN: 32,
                nDelta: 2,
                numSims: numberSimulations);
            Distribution upD = new Exponential(1.0);
            Distribution downD = new Exponential(1.0);
            sim.setUpDistro(upD, downD);
            sim.simCycle(runMode, randomSeed);
        }

        /// <summary>
        /// Extracts the trailing graph-size value from an experiment output file name.
        /// </summary>
        /// <param name="fileName">
        /// File name following the convention prefix_N.txt.
        /// </param>
        /// <returns>
        /// The integer graph size N parsed from the file name.
        /// </returns>
        private static int ExtractN(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);

            string[] parts = name.Split('_');

            return int.Parse(parts.Last());
        }

        /// <summary>
        /// Compares available path and cycle raw-delay files using empirical
        /// survival-function analysis.
        /// </summary>
        /// <remarks>
        /// Each path file msg_delays_path_N.txt is matched with
        /// msg_delays_cycle_2N.txt. Matching pairs are analyzed by SurvivalAnalysis
        /// and written to survival_comparison_pathN_cycle2N.csv.
        /// </remarks>
        public static void survivalComparison()
        {
            SurvivalAnalysis analysis = new SurvivalAnalysis();

            string[] pathFiles = Directory.GetFiles(
                Directory.GetCurrentDirectory(),
                "msg_delays_path_*.txt");

            string[] cycleFiles = Directory.GetFiles(
                Directory.GetCurrentDirectory(),
                "msg_delays_cycle_*.txt");

            foreach (string pathFile in pathFiles)
            {
                int pathN = ExtractN(pathFile);

                int expectedCycleN = 2 * pathN;

                string matchingCycleFile =
                    cycleFiles.FirstOrDefault(f => ExtractN(f) == expectedCycleN);

                if (matchingCycleFile == null)
                {
                    Console.WriteLine(
                        "No matching cycle file for path n={0}", pathN);
                    continue;
                }

                string outputFile =
                    $"survival_comparison_path{pathN}_cycle{expectedCycleN}.csv";

                Console.WriteLine(
                    "Comparing path n={0} with cycle n={1}",
                    pathN,
                    expectedCycleN);

                analysis.ComparePathAndCycleSurvival(
                    pathFile,
                    matchingCycleFile,
                    outputFile);
            }
        }

        /// <summary>
        /// Compares previously generated path and cycle summary output files
        /// and computes mean-delay reduction metrics.
        /// </summary>
        /// <remarks>
        /// This method does not run new simulations. It reads the existing
        /// path and cycle average-delay files, matches each path graph of size
        /// N with the corresponding cycle graph of size 2N, and writes both
        /// absolute and percentage delay-reduction results.
        /// </remarks>
        /// <param name="runMode">
        /// Restart/resume mode forwarded to both simulation families.
        /// </param>
        /// <param name="numberSimulations">
        /// Number of successful simulations to run per path size.
        /// </param>
        /// <param name="randomSeed">
        /// Seed forwarded to both simulation families.
        /// </param>
        public static void comparePathAndCycleExperiment(
            ExperimentRunMode runMode,
            int numberSimulations,
            int randomSeed)
        {
            bool runPath = true;
            bool runCycle = true;
            bool runCompare = true;
            bool runSurvival = true;

            if (runPath)
            {
                Console.WriteLine("Starting path experiment...");
                pathExponential(runMode, numberSimulations, randomSeed);
                Console.WriteLine("Finished path experiment.");
            }

            if (runCycle)
            {
                Console.WriteLine("Starting cycle experiment...");
                cycleExponential(runMode, numberSimulations, randomSeed);
                Console.WriteLine("Finished cycle experiment.");
            }

            if (runCompare)
            {
                Console.WriteLine("Starting comparison of path and cycle...");
                new TopologyComparisonReporter().ComparePathAndCycle();
                Console.WriteLine("Finished comparison of path and cycle.");
            }

            if (runSurvival)
            {
                Console.WriteLine("Starting survival comparison...");
                survivalComparison();
                Console.WriteLine("Finished survival comparison.");
            }
        }

        /// <summary>
        /// Runs the independent-path minimum-delay validation experiment
        /// for multiple path graph sizes.
        /// </summary>
        /// <param name="nValues">
        /// Collection of path graph sizes to evaluate.
        /// </param>
        /// <param name="runMode">
        /// Restart/resume mode forwarded to the baseline path simulations.
        /// </param>
        /// <param name="numberSimulations">
        /// Number of successful simulations to run per path size.
        /// </param>
        /// <param name="randomSeed">
        /// Seed forwarded to the baseline path simulations.
        /// </param>
        /// <remarks>
        /// For each n, reads msg_delays_path_n.txt and writes
        /// survival_independent_min_path_n.csv.
        /// </remarks>
        public static void independentPathMinimumExperiment(
            IEnumerable<int> nValues,
            ExperimentRunMode runMode,
            int numberSimulations,
            int randomSeed)
        {
            pathExponential(runMode, numberSimulations, randomSeed);

            Console.WriteLine("Starting independent-path minimum experiment...");

            IndependentPathExperiment experiment = new IndependentPathExperiment();

            foreach (int n in nValues)
            {
                string inputFile = $"msg_delays_path_{n}.txt";
                string outputFile = $"survival_independent_min_path_{n}.csv";

                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"Skipping n = {n}, file not found: {inputFile}");
                    continue;
                }

                Console.WriteLine($"Running independent-path experiment for n = {n}");

                experiment.CompareIndependentMinimum(inputFile, outputFile);
            }
            Console.WriteLine("Finished independent-path minimum experiment.");
        }


        /// <summary>
        /// Executes a small-scale validation experiment for the multi-path
        /// topology model.  The experiment generates a topology containing
        /// multiple internally disjoint equal-length paths between a shared
        /// source node and destination node and measures message delivery
        /// delay under node churn.
        ///
        /// The method is intended for debugging and validation of topology
        /// construction, routing behavior, and message-delay calculations
        /// prior to running large-scale experiments.
        /// </summary>
        /// <param name="pathLength">
        /// The number of edges in each path between the shared source and
        /// destination nodes.
        /// </param>
        /// <param name="pathCount">
        /// The number of internally disjoint equal-length paths between the
        /// shared source and destination nodes.
        /// </param>
        /// <param name="trials">
        /// The number of simulation trials to execute.
        /// </param>
        /// <param name="runMode">
        /// Controls whether the sanity-test delay file is cleared first.
        /// </param>
        /// <param name="randomSeed">
        /// Seed used to derive deterministic random streams for each trial.
        /// </param>
        /// <param name="maxAttempts">
        /// Maximum attempts allowed per successful trial.
        /// </param>
        public static void multiPathSanityTest(
            int pathLength,
            int pathCount,
            int trials,
            ExperimentRunMode runMode,
            int randomSeed,
            int maxAttempts = 3)
        {
            int numNodes = 2 + pathCount * (pathLength - 1);

            int numSessions = 100;
            double baseTime = 200.0;

            Distribution upDistro = new Exponential(2.0);
            Distribution downDistro = new Exponential(3.0);

            string delayOutputFile = $"msg_delays_multipath_k{pathCount}_len{pathLength}_sanity.txt";

            TrialRunner runner = new TrialRunner();
            TrialResultStore store = new TrialResultStore(
                delayOutputFile,
                string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "topology=multipath-sanity;length={0};count={1};seed={2}",
                    pathLength,
                    pathCount,
                    randomSeed));
            List<TrialResult> results = store.PrepareAndLoad(runMode);

            Console.WriteLine("Starting multi-path sanity test...");
            Console.WriteLine($"pathLength = {pathLength}");
            Console.WriteLine($"pathCount = {pathCount}");
            Console.WriteLine($"numNodes = {numNodes}");

            for (int trial = results.Count; trial < trials; trial++)
            {
                TrialResult result = TrialAttempts.Run(
                    attempt => runner.RunTrial(
                        TopologyFactory.CreateMultiPath(pathLength, pathCount),
                        numNodes,
                        numSessions,
                        baseTime,
                        upDistro.WithRandomSource(
                            new MersenneTwister(RandomSeed.Derive(
                                randomSeed,
                                pathLength,
                                pathCount,
                                trial,
                                attempt,
                                0))),
                        downDistro.WithRandomSource(
                            new MersenneTwister(RandomSeed.Derive(
                                randomSeed,
                                pathLength,
                                pathCount,
                                trial,
                                attempt,
                                1))),
                        MessageDelayMode.MultiPathEndpoint),
                    maxAttempts);

                results.Add(result);
                store.Append(result);
            }

            ResultSummary summary = ResultAggregator.Summarize(results);

            Console.WriteLine("Multi-path sanity test complete.");
            Console.WriteLine($"Successful trials: {results.Count}");
            Console.WriteLine($"Average delay: {summary.AverageMessageDelay:N4}");
            Console.WriteLine($"Zero delay percent: {summary.ZeroDelayPercent:N2}%");
            Console.WriteLine($"All nodes live percent: {summary.AllNodesLivePercent:N2}%");
            Console.WriteLine();
        }


        /// <summary>
        /// Runs baseline path simulations, multipath simulations, and the
        /// resulting survival comparison using exponential ON/OFF churn.
        /// </summary>
        /// <param name="runMode">
        /// Restart/resume mode forwarded through the complete workflow.
        /// </param>
        /// <param name="numberSimulations">
        /// Number of successful simulations to run for each baseline path size
        /// and multi-path topology configuration.
        /// </param>
        /// <param name="randomSeed">
        /// Seed used to derive deterministic random streams for each trial.
        /// </param>
        public static void multiPathExponential(
            ExperimentRunMode runMode,
            int numberSimulations,
            int randomSeed)
        {
            int[] pathLengths = { 5, 10, 15, 20 };
            int[] pathCounts = { 2, 3, 4, 5 };

            int numSessions = 1000;
            double baseTime = 200.0;

            Distribution upDistro = new Exponential(2.0);
            Distribution downDistro = new Exponential(3.0);

            Console.WriteLine("Starting baseline path simulations...");
            pathExponential(runMode, numberSimulations, randomSeed);
            Console.WriteLine("Finished baseline path simulations.");

            Console.WriteLine("Starting multi-path simulations...");
            MultiPathExperiment experiment = new MultiPathExperiment();
            experiment.Run(
                pathLengths,
                pathCounts,
                numberSimulations,
                numSessions,
                baseTime,
                upDistro,
                downDistro,
                runMode,
                randomSeed);
            Console.WriteLine("Finished multi-path simulations.");

            Console.WriteLine("Starting multi-path survival comparison...");
            MultiPathSurvivalComparison comparison = new MultiPathSurvivalComparison();
            comparison.Run(pathLengths, pathCounts);
            Console.WriteLine("Finished multi-path survival comparison.");
        }



        /// <summary>
        /// Entry point for the simulation program.
        /// </summary>
        /// <param name="args">Command-line arguments; currently unused.</param>
        /// <remarks>
        /// Change runMode here to control restart/resume behavior and
        /// numberSimulations to control the trial count, and randomSeed to
        /// reproduce every experiment invoked by this entry point.
        /// </remarks>
        static void Main(string[] args)
        {
            string outputDirectory = SimulationOutput.Initialize();
            ExperimentRunMode runMode = ExperimentRunMode.Resume;
            int numberSimulations = 100000;
            int randomSeed = 12345;

            Console.WriteLine("Simulation output directory: {0}", outputDirectory);
            Console.WriteLine("Experiment run mode: {0}", runMode);
            Console.WriteLine("Simulations per configuration: {0}", numberSimulations);
            Console.WriteLine("Experiment random seed: {0}", randomSeed);
            multiPathExponential(runMode, numberSimulations, randomSeed);
        }
    }
}
