namespace NetworkSimulation
{
    public class TrialResult
    {
        public double Delay { get; set; }
        public int NumLive { get; set; }
        public double PercentLive { get; set; }
        public double StartTime { get; set; }
        public bool Success { get; set; }
        public bool Connected { get; set; }
        public bool ZeroDelay { get; set; }
    }
}
