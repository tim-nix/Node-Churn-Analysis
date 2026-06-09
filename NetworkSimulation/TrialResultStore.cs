using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NetworkSimulation
{
    /// <summary>
    /// Persists complete trial checkpoints while retaining raw delay files.
    /// </summary>
    public class TrialResultStore
    {
        private const string Header =
            "Delay,NumLive,TotalNodes,PercentLive,StartTime,Success,ZeroDelay,AllNodesLive";

        private readonly string delayFile;
        private readonly string checkpointFile;
        private readonly string metadataFile;
        private readonly string metadata;

        public TrialResultStore(string delayFile, string metadata)
        {
            if (string.IsNullOrWhiteSpace(delayFile))
                throw new ArgumentException("Delay file path is required.", "delayFile");
            if (string.IsNullOrWhiteSpace(metadata))
                throw new ArgumentException("Trial metadata is required.", "metadata");

            this.delayFile = delayFile;
            this.metadata = metadata;
            checkpointFile = Path.Combine(
                Path.GetDirectoryName(delayFile) ?? string.Empty,
                Path.GetFileNameWithoutExtension(delayFile) + ".trials.csv");
            metadataFile = Path.Combine(
                Path.GetDirectoryName(delayFile) ?? string.Empty,
                Path.GetFileNameWithoutExtension(delayFile) + ".metadata.txt");
        }

        public string CheckpointFile
        {
            get { return checkpointFile; }
        }

        public List<TrialResult> PrepareAndLoad(ExperimentRunMode runMode)
        {
            if (runMode == ExperimentRunMode.Restart)
            {
                File.WriteAllText(delayFile, string.Empty);
                File.WriteAllText(checkpointFile, Header + Environment.NewLine);
                File.WriteAllText(metadataFile, metadata + Environment.NewLine);
                return new List<TrialResult>();
            }

            bool hasDelays = File.Exists(delayFile)
                && File.ReadLines(delayFile).Any(line => !string.IsNullOrWhiteSpace(line));

            if (!File.Exists(checkpointFile))
            {
                if (hasDelays)
                {
                    throw new InvalidOperationException(
                        "Cannot safely resume from a legacy delay-only file. "
                        + "Restart the experiment to create complete trial checkpoints: "
                        + delayFile);
                }

                File.WriteAllText(checkpointFile, Header + Environment.NewLine);
                File.WriteAllText(metadataFile, metadata + Environment.NewLine);
                return new List<TrialResult>();
            }

            ValidateMetadata();
            List<TrialResult> results = LoadCheckpoint();
            ValidateRawDelays(results);
            return results;
        }

        private void ValidateMetadata()
        {
            if (!File.Exists(metadataFile))
            {
                throw new InvalidOperationException(
                    "Cannot safely resume without experiment metadata: "
                    + metadataFile);
            }

            string persistedMetadata = File.ReadAllText(metadataFile).Trim();
            if (!string.Equals(persistedMetadata, metadata, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Experiment metadata does not match the checkpoint for "
                    + delayFile);
            }
        }

        public void Append(TrialResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");
            if (!result.Success)
                throw new ArgumentException(
                    "Only successful trials may be checkpointed.",
                    "result");

            File.AppendAllText(
                checkpointFile,
                Serialize(result) + Environment.NewLine);

            File.AppendAllText(
                delayFile,
                result.Delay.ToString("R", CultureInfo.InvariantCulture)
                + Environment.NewLine);
        }

        private List<TrialResult> LoadCheckpoint()
        {
            string[] lines = File.ReadAllLines(checkpointFile);

            if (lines.Length == 0 || lines[0] != Header)
            {
                throw new InvalidDataException(
                    "Trial checkpoint has an invalid or missing header: "
                    + checkpointFile);
            }

            List<TrialResult> results = new List<TrialResult>();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                results.Add(Parse(lines[i], i + 1));
            }

            return results;
        }

        private void ValidateRawDelays(IList<TrialResult> results)
        {
            List<double> delays = new List<double>();

            if (File.Exists(delayFile))
            {
                int lineNumber = 0;
                foreach (string line in File.ReadLines(delayFile))
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    double delay;
                    if (!double.TryParse(
                        line,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out delay))
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "Invalid delay at {0}, line {1}.",
                                delayFile,
                                lineNumber));
                    }

                    delays.Add(delay);
                }
            }

            if (delays.Count != results.Count)
            {
                throw new InvalidDataException(
                    "Raw delay and trial checkpoint counts do not match for "
                    + delayFile);
            }

            for (int i = 0; i < delays.Count; i++)
            {
                if (delays[i] != results[i].Delay)
                {
                    throw new InvalidDataException(
                        "Raw delay and trial checkpoint values do not match for "
                        + delayFile);
                }
            }
        }

        private static string Serialize(TrialResult result)
        {
            return string.Join(
                ",",
                result.Delay.ToString("R", CultureInfo.InvariantCulture),
                result.NumLive.ToString(CultureInfo.InvariantCulture),
                result.TotalNodes.ToString(CultureInfo.InvariantCulture),
                result.PercentLive.ToString("R", CultureInfo.InvariantCulture),
                result.StartTime.ToString("R", CultureInfo.InvariantCulture),
                result.Success.ToString(CultureInfo.InvariantCulture),
                result.ZeroDelay.ToString(CultureInfo.InvariantCulture),
                result.AllNodesLive.ToString(CultureInfo.InvariantCulture));
        }

        private TrialResult Parse(string line, int lineNumber)
        {
            string[] fields = line.Split(',');
            if (fields.Length != 8)
            {
                throw new InvalidDataException(
                    string.Format(
                        "Invalid trial checkpoint at {0}, line {1}.",
                        checkpointFile,
                        lineNumber));
            }

            try
            {
                return new TrialResult
                {
                    Delay = double.Parse(fields[0], CultureInfo.InvariantCulture),
                    NumLive = int.Parse(fields[1], CultureInfo.InvariantCulture),
                    TotalNodes = int.Parse(fields[2], CultureInfo.InvariantCulture),
                    PercentLive = double.Parse(fields[3], CultureInfo.InvariantCulture),
                    StartTime = double.Parse(fields[4], CultureInfo.InvariantCulture),
                    Success = bool.Parse(fields[5]),
                    ZeroDelay = bool.Parse(fields[6]),
                    AllNodesLive = bool.Parse(fields[7])
                };
            }
            catch (FormatException exception)
            {
                throw new InvalidDataException(
                    string.Format(
                        "Invalid trial checkpoint at {0}, line {1}.",
                        checkpointFile,
                        lineNumber),
                    exception);
            }
        }
    }
}
