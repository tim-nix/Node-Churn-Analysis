namespace NetworkSimulation
{
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
