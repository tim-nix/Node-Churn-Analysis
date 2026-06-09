namespace NetworkSimulation
{
    /// <summary>
    /// Stores the result of a single Monte Carlo message-delay trial.
    /// </summary>
    public class TrialResult
    {
        public double Delay { get; set; }
        public int NumLive { get; set; }
        public int TotalNodes { get; set; }
        public double PercentLive { get; set; }
        public double StartTime { get; set; }
        public bool Success { get; set; }
        public bool ZeroDelay { get; set; }
        public bool AllNodesLive { get; set; }
        public string FailureReason { get; set; }
    }
}
