namespace NetworkSimulation
{
    /// <summary>
    /// Stores aggregate statistics computed from a collection of successful
    /// Monte Carlo trials.
    /// </summary>
    public class ResultSummary
    {
        public double ZeroDelayPercent { get; set; }
        public double AllNodesLivePercent { get; set; }
        public double RedundancyGainPercent { get; set; }
        public double AverageMessageDelay { get; set; }
        public double AverageLivePercent { get; set; }
        public int TrialCount { get; set; }
    }
}
