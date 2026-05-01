using System;
using System.IO;
using System.Linq;

namespace NetworkSimulation
{
    class Program
    {
        public static void pathExponential()
        {
            Simulations sim = new Simulations(minN: 5, maxN: 20, nDelta: 5, numSims: 10000);
            Distribution upD = new Exponential(2.0);
            Distribution downD = new Exponential(3.0);
            sim.setUpDistro(upD, downD);
            sim.simPath();
        }

        public static void cycleExponential()
        {
            Simulations sim = new Simulations(minN: 10, maxN: 40, nDelta: 10, numSims: 10000);
            Distribution upD = new Exponential(2.0);
            Distribution downD = new Exponential(3.0);
            sim.setUpDistro(upD, downD);
            sim.simCycle();
        }

        private static int ExtractN(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);

            string[] parts = name.Split('_');

            return int.Parse(parts.Last());
        }

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

        public static void independentPathMinimumExperiment()
        {
            IndependentPathExperiment experiment = new IndependentPathExperiment();

            experiment.CompareIndependentMinimum(
                "msg_delays_path_5.txt",
                "survival_independent_min_path_5.csv");
        }



        static void Main(string[] args)
        {
            pathExponential();

            Console.WriteLine("Starting independent-path minimum experiment...");
            independentPathMinimumExperiment();
            Console.WriteLine("Finished independent-path minimum experiment.");
        }
    }
}
