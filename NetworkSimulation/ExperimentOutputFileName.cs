using System;
using System.IO;

namespace NetworkSimulation
{
    internal static class ExperimentOutputFileName
    {
        public static string GetRegimeSuffix(string regimeName)
        {
            if (string.IsNullOrWhiteSpace(regimeName))
            {
                throw new ArgumentException(
                    "Regime name cannot be empty.",
                    "regimeName");
            }

            if (regimeName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException(
                    "Regime name contains invalid file-name characters.",
                    "regimeName");
            }

            return "_" + regimeName;
        }
    }
}
