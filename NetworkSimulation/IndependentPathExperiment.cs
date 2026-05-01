using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NetworkSimulation
{
    public class IndependentPathExperiment
    {
        public void CompareIndependentMinimum(
            string pathDelayFileName,
            string outputFileName)
        {
            List<double> pathDelays = File.ReadAllLines(pathDelayFileName)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => double.Parse(line, CultureInfo.InvariantCulture))
                .ToList();

            if (pathDelays.Count < 2)
            {
                throw new ArgumentException("At least two path-delay samples are required.");
            }

            List<double> minDelays = new List<double>();

            for (int i = 0; i + 1 < pathDelays.Count; i += 2)
            {
                double t1 = pathDelays[i];
                double t2 = pathDelays[i + 1];

                minDelays.Add(Math.Min(t1, t2));
            }

            string minDelayFileName = "msg_delays_independent_min.txt";

            File.WriteAllLines(
                minDelayFileName,
                minDelays.Select(value => value.ToString(CultureInfo.InvariantCulture)));

            SurvivalAnalysis survival = new SurvivalAnalysis();

            List<Tuple<double, double>> pathSurvival =
                survival.ComputeSurvival(pathDelayFileName);

            List<Tuple<double, double>> minSurvival =
                survival.ComputeSurvival(minDelayFileName);

            File.WriteAllText(
                outputFileName,
                "t,S_path,S_min,S_path_squared,error" + Environment.NewLine);

            int count = pathSurvival.Count;
            int step = Math.Max(1, count / 20);

            Console.WriteLine("t\tS_path\tS_min\tS_path^2\terror");

            for (int i = 0; i < count; i += step)
            {
                double t = pathSurvival[i].Item1;
                double sPath = pathSurvival[i].Item2;
                double sMin = GetSurvivalAtTime(minSurvival, t);
                double sPathSquared = sPath * sPath;
                double error = sMin - sPathSquared;

                Console.WriteLine(
                    "{0:N4}\t{1:N4}\t{2:N4}\t{3:N4}\t{4:N4}",
                    t,
                    sPath,
                    sMin,
                    sPathSquared,
                    error);

                string line =
                    t.ToString(CultureInfo.InvariantCulture) + "," +
                    sPath.ToString(CultureInfo.InvariantCulture) + "," +
                    sMin.ToString(CultureInfo.InvariantCulture) + "," +
                    sPathSquared.ToString(CultureInfo.InvariantCulture) + "," +
                    error.ToString(CultureInfo.InvariantCulture);

                File.AppendAllText(outputFileName, line + Environment.NewLine);
            }
        }

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
    }
}