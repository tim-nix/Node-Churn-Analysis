using System;
using System.Collections.Generic;

namespace NetworkSimulation
{
    /// <summary>
    /// Represents the alternating ON/OFF lifecycle of one node.
    /// Sessions are generated lazily, so the lifecycle can be queried for
    /// arbitrarily late times without choosing a fixed array size up front.
    /// </summary>
    public class NodeTimeline
    {
        private readonly double baseTime;
        private readonly int initialSessionCount;
        private readonly List<Session> timeline;
        private Distribution upDistro;
        private Distribution downDistro;

        public int numSessions
        {
            get { return timeline.Count; }
        }

        public double BaseTime
        {
            get { return baseTime; }
        }

        public Session[] TimeLine
        {
            get
            {
                EnsureGenerated();

                Session[] copy = new Session[timeline.Count];
                for (int i = 0; i < copy.Length; i++)
                {
                    copy[i] = new Session(
                        timeline[i].StartTime,
                        timeline[i].EndTime);
                }

                return copy;
            }
        }

        /// <summary>
        /// Initializes a node timeline with an initial number of stored ON
        /// sessions. Additional sessions are generated lazily when later times
        /// are queried.
        /// </summary>
        /// <param name="numToTrack">Initial number of ON sessions to store.</param>
        /// <param name="earliestTime">
        /// Earliest time at which the first stored session may be active.
        /// </param>
        public NodeTimeline(int numToTrack, double earliestTime)
        {
            if (earliestTime < 0.0 || numToTrack <= 0)
                throw new ArgumentException("Error: Faulty arguments for NodeTimeline!");

            baseTime = earliestTime;
            initialSessionCount = numToTrack;
            timeline = new List<Session>(numToTrack);
        }

        /// <summary>
        /// Evolves the alternating OFF/ON process to the base time and stores
        /// the initial session window using the supplied duration distributions.
        /// </summary>
        /// <param name="upDistribution">Distribution of ON durations.</param>
        /// <param name="downDistribution">Distribution of OFF durations.</param>
        public void generateTimeline(
            Distribution upDistribution,
            Distribution downDistribution)
        {
            if (upDistribution == null)
                throw new ArgumentNullException("upDistribution");
            if (downDistribution == null)
                throw new ArgumentNullException("downDistribution");

            upDistro = upDistribution;
            downDistro = downDistribution;
            timeline.Clear();

            double previousEnd = 0.0;
            Session firstTrackedSession;
            do
            {
                firstTrackedSession = GenerateSession(previousEnd);
                previousEnd = firstTrackedSession.EndTime;
            }
            while (previousEnd < baseTime);

            timeline.Add(firstTrackedSession);

            while (timeline.Count < initialSessionCount)
                AppendSession(timeline[timeline.Count - 1].EndTime);
        }

        /// <summary>
        /// Returns the start time of the first stored ON session.
        /// </summary>
        public double getFirstTime()
        {
            EnsureGenerated();
            return timeline[0].StartTime;
        }

        /// <summary>
        /// Returns the end time of the first stored ON session.
        /// </summary>
        public double getFirstEnd()
        {
            EnsureGenerated();
            return timeline[0].EndTime;
        }

        /// <summary>
        /// Returns the end of the sessions generated so far. This is not a
        /// lifecycle limit; later queries extend the timeline automatically.
        /// </summary>
        public double getFinalTime()
        {
            EnsureGenerated();
            return timeline[timeline.Count - 1].EndTime;
        }

        /// <summary>
        /// Determines whether the node is ON at a given time, extending the
        /// timeline if necessary.
        /// </summary>
        /// <param name="time">Finite, non-negative time to query.</param>
        /// <returns>True when time falls in a stored ON interval.</returns>
        public bool timeIsLive(double time)
        {
            ValidateTime(time);
            EnsureThrough(time);

            int index = FindSessionAtOrBefore(time);
            return index >= 0 && timeline[index].isLive(time);
        }

        /// <summary>
        /// Returns the first ON-session start strictly after a given time.
        /// </summary>
        /// <param name="time">Finite, non-negative reference time.</param>
        public double getNextStart(double time)
        {
            ValidateTime(time);
            EnsureGenerated();

            while (timeline[timeline.Count - 1].StartTime <= time)
                AppendSession(timeline[timeline.Count - 1].EndTime);

            int low = 0;
            int high = timeline.Count - 1;
            while (low < high)
            {
                int middle = low + ((high - low) / 2);
                if (timeline[middle].StartTime <= time)
                    low = middle + 1;
                else
                    high = middle;
            }

            return timeline[low].StartTime;
        }

        /// <summary>
        /// Returns the remaining ON duration at a given time.
        /// </summary>
        /// <param name="time">Finite, non-negative time to query.</param>
        /// <returns>Remaining ON time, or zero when the node is OFF.</returns>
        public double getResidual(double time)
        {
            ValidateTime(time);
            EnsureThrough(time);

            int index = FindSessionAtOrBefore(time);
            return index >= 0 ? timeline[index].getResidual(time) : 0.0;
        }

        /// <summary>
        /// Returns the first ON/OFF boundary strictly after the supplied time.
        /// </summary>
        /// <param name="time">Finite, non-negative reference time.</param>
        public double getNextTransition(double time)
        {
            ValidateTime(time);
            EnsureThrough(time);

            int index = FindSessionAtOrBefore(time);
            if (index >= 0 && timeline[index].isLive(time)
                && timeline[index].EndTime > time)
            {
                return timeline[index].EndTime;
            }

            return getNextStart(time);
        }

        /// <summary>
        /// Computes this node's ON residual at every stored session start in
        /// another timeline.
        /// </summary>
        /// <param name="other">Timeline supplying the query times.</param>
        public double[] getResiduals(NodeTimeline other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            Session[] otherTimeline = other.TimeLine;
            double[] residuals = new double[otherTimeline.Length];
            for (int i = 0; i < residuals.Length; i++)
                residuals[i] = getResidual(otherTimeline[i].StartTime);

            return residuals;
        }

        /// <summary>
        /// Returns the start times of all sessions generated so far.
        /// </summary>
        public double[] getStartTimes()
        {
            EnsureGenerated();

            double[] startTimes = new double[timeline.Count];
            for (int i = 0; i < timeline.Count; i++)
                startTimes[i] = timeline[i].StartTime;

            return startTimes;
        }

        /// <summary>
        /// Calculates the mean ON duration across sessions generated so far.
        /// </summary>
        public double averageUpTime()
        {
            EnsureGenerated();

            double sum = 0.0;
            for (int i = 0; i < timeline.Count; i++)
                sum += timeline[i].getDurationLive();

            return sum / timeline.Count;
        }

        /// <summary>
        /// Calculates the mean OFF duration between stored ON sessions.
        /// </summary>
        /// <returns>Zero when fewer than two sessions are stored.</returns>
        public double averageDownTime()
        {
            EnsureGenerated();
            if (timeline.Count < 2)
                return 0.0;

            double sum = 0.0;
            for (int i = 1; i < timeline.Count; i++)
                sum += timeline[i].StartTime - timeline[i - 1].EndTime;

            return sum / (timeline.Count - 1);
        }

        /// <summary>
        /// Estimates the empirical CDF of generated ON durations.
        /// </summary>
        /// <param name="hSize">Number of histogram thresholds.</param>
        /// <param name="rate">Distance between successive thresholds.</param>
        public double[] getUpTimeCDF(int hSize = 50, double rate = 0.5)
        {
            EnsureGenerated();
            ValidateHistogramArguments(hSize, rate);

            double[] cdf = new double[hSize];
            double cap = 0.0;
            for (int i = 0; i < hSize; i++)
            {
                for (int j = 0; j < timeline.Count; j++)
                {
                    if (timeline[j].getDurationLive() < cap)
                        cdf[i] += 1.0;
                }

                cdf[i] /= timeline.Count;
                cap += rate;
            }

            return cdf;
        }

        /// <summary>
        /// Estimates the empirical CDF of generated OFF durations.
        /// </summary>
        /// <param name="hSize">Number of histogram thresholds.</param>
        /// <param name="rate">Distance between successive thresholds.</param>
        public double[] getDownTimeCDF(int hSize = 50, double rate = 0.5)
        {
            EnsureGenerated();
            ValidateHistogramArguments(hSize, rate);

            double[] cdf = new double[hSize];
            if (timeline.Count < 2)
                return cdf;

            double cap = 0.0;
            for (int i = 0; i < hSize; i++)
            {
                for (int j = 1; j < timeline.Count; j++)
                {
                    double downTime =
                        timeline[j].StartTime - timeline[j - 1].EndTime;
                    if (downTime < cap)
                        cdf[i] += 1.0;
                }

                cdf[i] /= timeline.Count - 1;
                cap += rate;
            }

            return cdf;
        }

        /// <summary>
        /// Formats the base time and all sessions generated so far.
        /// </summary>
        public override string ToString()
        {
            EnsureGenerated();

            System.Text.StringBuilder text =
                new System.Text.StringBuilder("Base Time: " + baseTime + "\n");
            for (int i = 0; i < timeline.Count; i++)
                text.Append(timeline[i]).Append('\n');

            return text.ToString();
        }

        /// <summary>
        /// Generates and appends the next OFF/ON cycle.
        /// </summary>
        private void AppendSession(double previousEnd)
        {
            timeline.Add(GenerateSession(previousEnd));
        }

        /// <summary>
        /// Generates the next ON session following an OFF interval.
        /// </summary>
        private Session GenerateSession(double previousEnd)
        {
            double downTime = GenerateDuration(downDistro, "OFF");
            double upTime = GenerateDuration(upDistro, "ON");
            double startTime = previousEnd + downTime;
            double endTime = startTime + upTime;

            if (double.IsInfinity(startTime) || double.IsInfinity(endTime))
                throw new InvalidOperationException(
                    "Timeline duration overflowed the supported time range.");

            return new Session(startTime, endTime);
        }

        /// <summary>
        /// Draws a finite positive duration, retrying zero-valued samples to
        /// guarantee that simulated time advances.
        /// </summary>
        private static double GenerateDuration(
            Distribution distribution,
            string stateName)
        {
            const int maxAttempts = 1000;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                double duration = distribution.generateRandom();
                if (double.IsNaN(duration)
                    || double.IsInfinity(duration)
                    || duration < 0.0)
                {
                    throw new InvalidOperationException(
                        stateName + " distribution must generate finite, non-negative durations.");
                }

                if (duration > 0.0)
                    return duration;
            }

            throw new InvalidOperationException(
                stateName + " distribution repeatedly generated zero-duration states.");
        }

        /// <summary>
        /// Extends the timeline until it covers the supplied time.
        /// </summary>
        private void EnsureThrough(double time)
        {
            EnsureGenerated();
            while (timeline[timeline.Count - 1].EndTime <= time)
                AppendSession(timeline[timeline.Count - 1].EndTime);
        }

        /// <summary>
        /// Finds the last session whose start is not later than time.
        /// </summary>
        private int FindSessionAtOrBefore(double time)
        {
            int low = 0;
            int high = timeline.Count - 1;
            int result = -1;

            while (low <= high)
            {
                int middle = low + ((high - low) / 2);
                if (timeline[middle].StartTime <= time)
                {
                    result = middle;
                    low = middle + 1;
                }
                else
                {
                    high = middle - 1;
                }
            }

            return result;
        }

        /// <summary>
        /// Verifies that distributions and the initial timeline were generated.
        /// </summary>
        private void EnsureGenerated()
        {
            if (timeline.Count == 0 || upDistro == null || downDistro == null)
                throw new NullReferenceException("Error: Must first generate a timeline!");
        }

        /// <summary>
        /// Validates a timeline query time.
        /// </summary>
        private static void ValidateTime(double time)
        {
            if (double.IsNaN(time) || double.IsInfinity(time) || time < 0.0)
                throw new ArgumentException("Time must be finite and non-negative.");
        }

        /// <summary>
        /// Validates empirical-distribution histogram settings.
        /// </summary>
        private static void ValidateHistogramArguments(int hSize, double rate)
        {
            if (hSize <= 0)
                throw new ArgumentException("Histogram size must be positive.");
            if (rate <= 0.0)
                throw new ArgumentException("Histogram rate must be positive.");
        }
    }
}
