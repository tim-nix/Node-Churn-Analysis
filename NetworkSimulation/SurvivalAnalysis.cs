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
                "t,S_path,S_cycle,S_path_squared" + Environment.NewLine);

            int count = Math.Min(pathSurvival.Count, cycleSurvival.Count);

            int step = Math.Max(1, count / 20);

            Console.WriteLine("t\tS_path\tS_cycle\tS_path^2");

            for (int i = 0; i < count; i += step)
            {
                double t = pathSurvival[i].Item1;

                double sPath = pathSurvival[i].Item2;

                double sCycle = GetSurvivalAtTime(cycleSurvival, t);

                double sPathSquared = sPath * sPath;

                Console.WriteLine(
                    "{0:N4}\t{1:N4}\t{2:N4}\t{3:N4}",
                    t,
                    sPath,
                    sCycle,
                    sPathSquared);
            }
        }
    }
}