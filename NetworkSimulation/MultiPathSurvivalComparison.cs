using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetworkSimulation
{
    /// <summary>
    /// Computes empirical survival curves and redundancy exponent estimates
    /// for shared-endpoint multi-path topologies.
    ///
    /// This class reads previously generated raw delay files and compares
    /// each multi-path delay distribution against a corresponding single-path
    /// baseline.  It does not run new simulations.
    /// </summary>
    public class MultiPathSurvivalComparison
    {
        private const double LowerSurvivalCutoff = 0.15;
        private const double UpperSurvivalCutoff = 0.85;

        public void Run(
            int[] pathLengths,
            int[] pathCounts)
        {
            File.WriteAllText(
                "alpha_summary_multipath.txt",
                "pathLength,pathCount,meanAlpha,medianAlpha,pointsUsed" +
                Environment.NewLine);

            foreach (int pathLength in pathLengths)
            {
                string pathDelayFile = $"msg_delays_path_{pathLength}.txt";

                if (!File.Exists(pathDelayFile))
                {
                    Console.WriteLine($"Missing baseline path file: {pathDelayFile}");

                    continue;
                }

                List<double> pathDelays =
                    LoadDelays(pathDelayFile);

                foreach (int pathCount in pathCounts)
                {
                    string multiPathDelayFile = $"msg_delays_multipath_k{pathCount}_len{pathLength}.txt";

                    if (!File.Exists(multiPathDelayFile))
                    {
                        Console.WriteLine(
                            $"Missing multi-path file: {multiPathDelayFile}");

                        continue;
                    }

                    List<double> multiPathDelays = LoadDelays(multiPathDelayFile);

                    AnalyzeConfiguration(
                        pathLength,
                        pathCount,
                        pathDelays,
                        multiPathDelays);
                }
            }
        }


        private class SurvivalRow
        {
            public double Time { get; set; }
            public double SPath { get; set; }
            public double SMultiPath { get; set; }
            public double SPathToK { get; set; }
            public double Alpha { get; set; }
        }


        private void AnalyzeConfiguration(
    int pathLength,
    int pathCount,
    List<double> pathDelays,
    List<double> multiPathDelays)
        {
            string survivalOutputFile = $"survival_multipath_k{pathCount}_len{pathLength}.csv";

            List<double> timePoints =
                pathDelays
                    .Concat(multiPathDelays)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();

            List<double> alphaValues = new List<double>();

            List<SurvivalRow> rows = new List<SurvivalRow>();

            foreach (double t in timePoints)
            {
                double sPath = Survival(pathDelays, t);

                double sMultiPath = Survival(multiPathDelays, t);

                if (sPath <= 0.0 || sPath >= 1.0 || sMultiPath <= 0.0 || sMultiPath >= 1.0)
                {
                    continue;
                }

                double idealIndependent = Math.Pow(sPath, pathCount);

                double alpha = Math.Log(sMultiPath) / Math.Log(sPath);

                rows.Add(
                    new SurvivalRow
                    {
                        Time = t,
                        SPath = sPath,
                        SMultiPath = sMultiPath,
                        SPathToK = idealIndependent,
                        Alpha = alpha
                    });

                if (sPath >= LowerSurvivalCutoff &&
                    sPath <= UpperSurvivalCutoff)
                {
                    alphaValues.Add(alpha);
                }
            }

            if (alphaValues.Count == 0)
            {
                Console.WriteLine($"No alpha values available for pathLength={pathLength}, k={pathCount}.");

                return;
            }

            double meanAlpha = alphaValues.Average();

            double medianAlpha = Median(alphaValues);

            File.WriteAllText(survivalOutputFile, "t,S_path,S_multipath,S_path_to_k,alpha,S_path_to_mean_alpha" + Environment.NewLine);

            foreach (SurvivalRow row in rows)
            {
                double fittedSurvival =
                    Math.Pow(row.SPath, meanAlpha);

                File.AppendAllText(
                    survivalOutputFile,
                    $"{row.Time},{row.SPath},{row.SMultiPath},{row.SPathToK},{row.Alpha},{fittedSurvival}" +
                    Environment.NewLine);
            }

            File.AppendAllText(
                "alpha_summary_multipath.txt",
                $"{pathLength},{pathCount},{meanAlpha},{medianAlpha},{alphaValues.Count}" +
                Environment.NewLine);

            Console.WriteLine(
                $"Multi-path alpha summary: length={pathLength}, k={pathCount}, " +
                $"mean alpha={meanAlpha:N4}, median alpha={medianAlpha:N4}, " +
                $"points={alphaValues.Count}");
        }

        private static List<double> LoadDelays(string filename)
        {
            return File.ReadLines(filename)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => double.Parse(
                    line,
                    System.Globalization.CultureInfo.InvariantCulture))
                .ToList();
        }

        private static double Survival(
            List<double> delays,
            double t)
        {
            int count =
                delays.Count(delay => delay > t);

            return (double)count / delays.Count;
        }

        private static double Median(List<double> values)
        {
            List<double> sorted =
                values.OrderBy(value => value).ToList();

            int count =
                sorted.Count;

            if (count % 2 == 1)
            {
                return sorted[count / 2];
            }

            return (sorted[(count / 2) - 1] + sorted[count / 2]) / 2.0;
        }
    }
}
