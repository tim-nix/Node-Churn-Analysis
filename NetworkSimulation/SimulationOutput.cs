using System;
using System.IO;

namespace NetworkSimulation
{
    /// <summary>
    /// Establishes the directory used by simulation output readers and writers.
    /// </summary>
    public static class SimulationOutput
    {
        private static bool initialized;

        /// <summary>
        /// Gets the absolute directory selected for simulation files.
        /// </summary>
        public static string DirectoryPath { get; private set; }

        /// <summary>
        /// Creates a directory beneath the current working directory and makes
        /// it the process working directory for all relative simulation file
        /// paths. Repeated calls return the originally selected directory.
        /// </summary>
        /// <param name="directoryName">
        /// Child directory name to create beneath the startup directory.
        /// </param>
        /// <returns>The absolute output directory path.</returns>
        public static string Initialize(string directoryName = "output")
        {
            if (initialized)
                return DirectoryPath;

            if (string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentException(
                    "Output directory name cannot be empty.",
                    "directoryName");

            string startupDirectory = Environment.CurrentDirectory;
            DirectoryPath = Path.GetFullPath(
                Path.Combine(startupDirectory, directoryName));

            Directory.CreateDirectory(DirectoryPath);
            Directory.SetCurrentDirectory(DirectoryPath);
            initialized = true;

            return DirectoryPath;
        }
    }
}
