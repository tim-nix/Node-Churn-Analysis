using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NetworkSimulation
{
    public class SurvivalAnalysis
    {
        private double GetSurvivalAtTime(
            List<Tuple<double, double>> survivalData,
            double t)
        {
            foreach (var pair in survivalData)
            {
                if (pair.Item1 >= t)
                {
                    return pair.Item2;
                }
            }

            return 0.0;
        }

        public List<Tuple<double, double>> ComputeSurvival(string fileName)
        {
            List<double> samples = File.ReadAllLines(fileName)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => double.Parse(line, CultureInfo.InvariantCulture))
                .OrderBy(value => value)
                .ToList();

            int n = samples.Count;

            List<Tuple<double, double>> survivalValues =
                new List<Tuple<double, double>>();

            for (int i = 0; i < n; i++)
            {
                double t = samples[i];

                int numberStillDelayed = n - i;

                double survival = numberStillDelayed / Convert.ToDouble(n);

                survivalValues.Add(
                    new Tuple<double, double>(t, survival));
            }

            return survivalValues;
        }

        public void ComparePathAndCycleSurvival(
            string pathFileName,
            string cycleFileName,
            string outputFileName)
        {
            List<Tuple<double, double>> pathSurvival =
                ComputeSurvival(pathFileName);

            List<Tuple<double, double>> cycleSurvival =
                ComputeSurvival(cycleFileName);

            File.WriteAllText(
                outputFileName,
                "t,S_path,S_cycle,S_path_squared,alpha" + Environment.NewLine);

            int count = Math.Min(pathSurvival.Count, cycleSurvival.Count);

            int step = Math.Max(1, count / 20);

            double alphaSum = 0.0;
            int alphaCount = 0;

            Console.WriteLine("t\tS_path\tS_cycle\tS_path^2\talpha");

            List<double> alphaValues = new List<double>();

            for (int i = 0; i < count; i += step)
            {
                double t = pathSurvival[i].Item1;

                double sPath = pathSurvival[i].Item2;

                double sCycle = GetSurvivalAtTime(cycleSurvival, t);

                double sPathSquared = sPath * sPath;

                double alpha = 0.0;

                if (sPath >= 0.10 && sPath <= 0.90 &&
                    sCycle > 0.0 && sCycle < 1.0 &&
                    sCycle < sPath)
                {
                    alpha = Math.Log(sCycle) / Math.Log(sPath);

                    alphaSum += alpha;
                    alphaCount++;

                    alphaValues.Add(alpha);
                }

                if (alpha > 0.0)
                {
                    Console.WriteLine(
                        "{0:N4}\t{1:N4}\t{2:N4}\t{3:N4}\t{4:N4}",
                        t,
                        sPath,
                        sCycle,
                        sPathSquared,
                        alpha);
                }

                string line =
                    t.ToString(CultureInfo.InvariantCulture) + "," +
                    sPath.ToString(CultureInfo.InvariantCulture) + "," +
                    sCycle.ToString(CultureInfo.InvariantCulture) + "," +
                    sPathSquared.ToString(CultureInfo.InvariantCulture) + "," +
                    alpha.ToString(CultureInfo.InvariantCulture);

                File.AppendAllText(
                    outputFileName,
                    line + Environment.NewLine);
            }

            if (alphaValues.Count > 0)
            {
                alphaValues.Sort();

                double medianAlpha;

                int m = alphaValues.Count;

                if (m % 2 == 1)
                    medianAlpha = alphaValues[m / 2];
                else
                    medianAlpha = (alphaValues[m / 2 - 1] + alphaValues[m / 2]) / 2.0;

                double averageAlpha = alphaSum / alphaCount;

                Console.WriteLine(
                    "Redundancy strength (alpha): mean = {0:N4}, median = {1:N4}",
                    averageAlpha,
                    medianAlpha);
            }
            else
            {
                Console.WriteLine("No valid alpha values were computed.");
            }
        }
    }
}