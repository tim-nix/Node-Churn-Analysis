using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NetworkSimulation;

namespace NetworkSimulation.Tests
{
    internal static class Program
    {
        private static int failures;

        private static int Main()
        {
            Run("Checkpoint round trip is complete and invariant", CheckpointRoundTrip);
            Run("Legacy delay-only resume is rejected", LegacyResumeIsRejected);
            Run("Changed resume metadata is rejected", ChangedMetadataIsRejected);
            Run("Retry attempts are bounded", RetryAttemptsAreBounded);
            Run("Unexpected trial exceptions propagate", UnexpectedTrialExceptionsPropagate);
            Run("Seeded distributions are reproducible", SeededDistributionsAreReproducible);
            Run("Resumed path run matches uninterrupted run", ResumeMatchesUninterrupted);
            Run("Multi-path length includes shared endpoints", MultiPathLengthIncludesEndpoints);
            Run("Multi-path requires an internal node", MultiPathRequiresInternalNode);
            Run("Equivalent cycle preserves path length", EquivalentCyclePreservesPathLength);

            Console.WriteLine(
                failures == 0
                    ? "All tests passed."
                    : failures + " test(s) failed.");

            return failures == 0 ? 0 : 1;
        }

        private static void CheckpointRoundTrip()
        {
            WithTemporaryDirectory(directory =>
            {
                CultureInfo originalCulture = CultureInfo.CurrentCulture;
                try
                {
                    CultureInfo.CurrentCulture = new CultureInfo("fr-FR");

                    string delayFile = Path.Combine(directory, "delays.txt");
                    TrialResultStore store =
                        new TrialResultStore(delayFile, "seed=42");
                    store.PrepareAndLoad(ExperimentRunMode.Restart);

                    TrialResult expected = new TrialResult
                    {
                        Delay = 1.25,
                        NumLive = 3,
                        TotalNodes = 4,
                        PercentLive = 75.5,
                        StartTime = 225.125,
                        Success = true,
                        ZeroDelay = false,
                        AllNodesLive = false
                    };

                    store.Append(expected);
                    TrialResult actual =
                        store.PrepareAndLoad(ExperimentRunMode.Resume).Single();

                    Equal(expected.Delay, actual.Delay, "Delay");
                    Equal(expected.NumLive, actual.NumLive, "NumLive");
                    Equal(expected.TotalNodes, actual.TotalNodes, "TotalNodes");
                    Equal(expected.PercentLive, actual.PercentLive, "PercentLive");
                    Equal(expected.StartTime, actual.StartTime, "StartTime");
                    Equal(expected.AllNodesLive, actual.AllNodesLive, "AllNodesLive");
                    Equal("1.25", File.ReadAllText(delayFile).Trim(), "Invariant delay");
                }
                finally
                {
                    CultureInfo.CurrentCulture = originalCulture;
                }
            });
        }

        private static void LegacyResumeIsRejected()
        {
            WithTemporaryDirectory(directory =>
            {
                string delayFile = Path.Combine(directory, "legacy.txt");
                File.WriteAllText(delayFile, "1.5" + Environment.NewLine);
                TrialResultStore store =
                    new TrialResultStore(delayFile, "seed=42");

                Throws<InvalidOperationException>(
                    () => store.PrepareAndLoad(ExperimentRunMode.Resume));
            });
        }

        private static void RetryAttemptsAreBounded()
        {
            int attempts = 0;

            Throws<InvalidOperationException>(
                () => TrialAttempts.Run(
                    attempt =>
                    {
                        attempts++;
                        return new TrialResult
                        {
                            Success = false,
                            FailureReason = "expected test failure"
                        };
                    },
                    3));

            Equal(3, attempts, "Attempt count");
        }

        private static void ChangedMetadataIsRejected()
        {
            WithTemporaryDirectory(directory =>
            {
                string delayFile = Path.Combine(directory, "delays.txt");
                new TrialResultStore(delayFile, "seed=42")
                    .PrepareAndLoad(ExperimentRunMode.Restart);

                Throws<InvalidOperationException>(
                    () => new TrialResultStore(delayFile, "seed=43")
                        .PrepareAndLoad(ExperimentRunMode.Resume));
            });
        }

        private static void UnexpectedTrialExceptionsPropagate()
        {
            TrialRunner runner = new TrialRunner();

            Throws<ArgumentException>(
                () => runner.RunTrial(
                    TopologyFactory.CreatePath(3),
                    3,
                    2,
                    0.0,
                    new ThrowingDistribution(),
                    new ConstantDistribution(1.0),
                    MessageDelayMode.PathEndpoint));
        }

        private static void SeededDistributionsAreReproducible()
        {
            Exponential first =
                new Exponential(2.0, new MersenneTwister(1234));
            Exponential second =
                new Exponential(2.0, new MersenneTwister(1234));

            for (int i = 0; i < 20; i++)
                Equal(first.generateRandom(), second.generateRandom(), "Random sample");

            uint derivedA = RandomSeed.Derive(99, 5, 2, 7, 0, 1);
            uint derivedB = RandomSeed.Derive(99, 5, 2, 7, 0, 1);
            Equal(derivedA, derivedB, "Derived seed");
        }

        private static void ResumeMatchesUninterrupted()
        {
            WithTemporaryDirectory(root =>
            {
                string resumedDirectory = Path.Combine(root, "resumed");
                string completeDirectory = Path.Combine(root, "complete");
                Directory.CreateDirectory(resumedDirectory);
                Directory.CreateDirectory(completeDirectory);

                RunPathExperiment(resumedDirectory, 2, ExperimentRunMode.Restart);
                RunPathExperiment(resumedDirectory, 4, ExperimentRunMode.Resume);
                RunPathExperiment(completeDirectory, 4, ExperimentRunMode.Restart);

                string resumed = File.ReadAllText(
                    Path.Combine(resumedDirectory, "msg_delays_path_3.trials.csv"));
                string complete = File.ReadAllText(
                    Path.Combine(completeDirectory, "msg_delays_path_3.trials.csv"));

                Equal(complete, resumed, "Checkpoint contents");
            });
        }

        private static void MultiPathLengthIncludesEndpoints()
        {
            const int pathLength = 5;
            const int pathCount = 3;

            int[,] graph = CommonGraphs.MultiPath(pathLength, pathCount);

            Equal(
                2 + pathCount * (pathLength - 2),
                graph.GetLength(0),
                "Graph order");

            Network network = new Network(graph);
            List<int> lengths = network
                .getShortestIndependentPathLengths(0, 1)
                .Cast<int>()
                .ToList();

            Equal(pathCount, lengths.Count, "Independent path count");
            foreach (int edgeLength in lengths)
                Equal(pathLength - 1, edgeLength, "Path edge count");
        }

        private static void MultiPathRequiresInternalNode()
        {
            Throws<ArgumentException>(() => CommonGraphs.MultiPath(2, 2));
        }

        private static void EquivalentCyclePreservesPathLength()
        {
            int[] pathLengths = { 5, 10, 15, 20 };
            int[] expectedCycleOrders = { 8, 18, 28, 38 };

            for (int i = 0; i < pathLengths.Length; i++)
            {
                int cycleOrder =
                    TopologyFactory.GetEquivalentCycleOrder(pathLengths[i]);

                Equal(expectedCycleOrders[i], cycleOrder, "Cycle order");
                Equal(
                    pathLengths[i],
                    (cycleOrder / 2) + 1,
                    "Cycle diameter path nodes");
            }

            Throws<ArgumentException>(
                () => TopologyFactory.GetEquivalentCycleOrder(1));
        }

        private static void RunPathExperiment(
            string directory,
            int count,
            ExperimentRunMode runMode)
        {
            string originalDirectory = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = directory;
                new PathExperiment().Run(
                    3,
                    4,
                    1,
                    count,
                    4,
                    0.0,
                    new Exponential(2.0),
                    new Exponential(3.0),
                    runMode,
                    8675309,
                    3);
            }
            finally
            {
                Environment.CurrentDirectory = originalDirectory;
            }
        }

        private static void WithTemporaryDirectory(Action<string> action)
        {
            string directory = Path.Combine(
                Path.GetTempPath(),
                "NetworkSimulation.Tests",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            try
            {
                action(directory);
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        private static void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("PASS " + name);
            }
            catch (Exception exception)
            {
                failures++;
                Console.WriteLine("FAIL " + name);
                Console.WriteLine(exception);
            }
        }

        private static void Equal<T>(T expected, T actual, string label)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new Exception(
                    string.Format(
                        "{0}: expected <{1}>, actual <{2}>.",
                        label,
                        expected,
                        actual));
            }
        }

        private static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }

            throw new Exception(
                "Expected exception " + typeof(TException).Name + ".");
        }

        private sealed class ThrowingDistribution : Distribution
        {
            public override double generateRandom()
            {
                throw new ArgumentException("Unexpected configuration failure.");
            }

            public override Distribution WithRandomSource(IRandomSource randomSource)
            {
                return this;
            }

            public override double getExpectedValue()
            {
                return 0.0;
            }

            public override double[] getPMF(int hSize = 50)
            {
                return new double[hSize];
            }
        }

        private sealed class ConstantDistribution : Distribution
        {
            private readonly double value;

            public ConstantDistribution(double value)
            {
                this.value = value;
            }

            public override double generateRandom()
            {
                return value;
            }

            public override Distribution WithRandomSource(IRandomSource randomSource)
            {
                return this;
            }

            public override double getExpectedValue()
            {
                return value;
            }

            public override double[] getPMF(int hSize = 50)
            {
                return new double[hSize];
            }
        }
    }
}
