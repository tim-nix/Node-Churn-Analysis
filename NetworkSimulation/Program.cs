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
        /// Current parameters use path sizes 5, 10, and 15; exponential ON rate
        /// 2.0; exponential OFF/recovery rate 3.0; and 1000 simulations per size.
        /// </remarks>
        public static void pathExponential()
        {
            Simulations sim = new Simulations(minN: 5, maxN: 16, nDelta: 1, numSims: 100000);
            Distribution upD = new Exponential(1.0);
            Distribution downD = new Exponential(1.0);
            sim.setUpDistro(upD, downD);
            sim.simPath(ExperimentRunMode.Resume);
        }

        /// <summary>
        /// Runs the configured cycle-topology experiment using exponential ON/OFF
        /// churn distributions.
        /// </summary>
        /// <remarks>
        /// Current parameters use cycle sizes 10, 20, and 30 so that each cycle
        /// comparison preserves the source-target distance of the corresponding
        /// path experiment.
        /// </remarks>
        public static void cycleExponential()
        {
            Simulations sim = new Simulations(minN: 10, maxN: 32, nDelta: 2, numSims: 100000);
            Distribution upD = new Exponential(1.0);
            Distribution downD = new Exponential(1.0);
            sim.setUpDistro(upD, downD);
            sim.simCycle(ExperimentRunMode.Resume);
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
        public static void comparePathAndCycleExperiment()
        {
            bool runPath = true;
            bool runCycle = true;
            bool runCompare = true;
            bool runSurvival = true;

            if (runPath)
            {
                Console.WriteLine("Starting path experiment...");
                pathExponential();
                Console.WriteLine("Finished path experiment.");
            }

            if (runCycle)
            {
                Console.WriteLine("Starting cycle experiment...");
                cycleExponential();
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
        /// <remarks>
        /// For each n, reads msg_delays_path_n.txt and writes
        /// survival_independent_min_path_n.csv.
        /// </remarks>
        public static void independentPathMinimumExperiment(IEnumerable<int> nValues)
        {
            pathExponential();

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
        public static void multiPathSanityTest(
            int pathLength,
            int pathCount,
            int trials)
        {
            int numNodes = 2 + pathCount * (pathLength - 1);

            int numSessions = 100;
            double baseTime = 200.0;

            Distribution upDistro = new Exponential(2.0);
            Distribution downDistro = new Exponential(3.0);

            string delayOutputFile = $"msg_delays_multipath_k{pathCount}_len{pathLength}_sanity.txt";

            File.WriteAllText(delayOutputFile, "");

            TrialRunner runner = new TrialRunner();
            List<TrialResult> results = new List<TrialResult>();

            Console.WriteLine("Starting multi-path sanity test...");
            Console.WriteLine($"pathLength = {pathLength}");
            Console.WriteLine($"pathCount = {pathCount}");
            Console.WriteLine($"numNodes = {numNodes}");

            for (int trial = 0; trial < trials; trial++)
            {
                Network network =
                    TopologyFactory.CreateMultiPath(pathLength, pathCount);

                TrialResult result = runner.RunTrial(
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
                    trial--;
                    continue;
                }

                results.Add(result);
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
        /// Entry point for the simulation program.
        /// </summary>
        /// <param name="args">Command-line arguments; currently unused.</param>
        /// <remarks>
        /// Boolean flags control whether path experiments, cycle experiments,
        /// mean-delay comparison, and survival comparison are executed.
        /// </remarks>
        static void Main(string[] args)
        {
            MultiPathExperiment experiment = new MultiPathExperiment();

            experiment.Run(
                new int[] { 5 },
                new int[] { 2, 3, 4 },
                1000,
                100,
                200.0,
                new Exponential(2.0),
                new Exponential(3.0),
                ExperimentRunMode.Restart);
        }
    }
}
