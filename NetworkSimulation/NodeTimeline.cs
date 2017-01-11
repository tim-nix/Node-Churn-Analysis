using System;


namespace NetworkSimulation
{
    /// <summary>
    /// The purpose of this class is to represent a series
    /// of sessions of a given node.  The baseTime attribute
    /// corresponds to the earliest time to start tracking
    /// sessions.  This provides the lifecycle time to evolve.
    /// The timeline attribute stores all sessions.
    /// </summary>
    public class NodeTimeline
    {
        private double baseTime = 0.0;  // the earliest possible start time  
        private Session[] timeline;     // the collection of tracked sessions


        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member baseTime.
        /// </summary>
        public double BaseTime
        {
            get
            {
                return baseTime;
            }
        }


        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member timeline.
        /// </summary>
        public Session[] TimeLine
        {
            get
            {
                Session[] tl = new Session[timeline.Length];

                for (int i = 0; i < tl.Length; i++)
                    tl[i] = new Session(timeline[i].StartTime, timeline[i].EndTime);

                return tl;
            }
        }


        /// <summary>
        /// A constructor for the NodeTimeline class.  Sets up the
        /// baseTime, the numSessions and creates the array that 
        /// will be used to store the tracked sessions.
        /// </summary>
        /// <param name="numToTrack">The number of live sessions to track</param>
        /// <param name="earliestTime">The earliest time to track sessions</param>
        public NodeTimeline(int numToTrack, double earliestTime)
        {
            if ((earliestTime >= 0.0) && (numToTrack > 0))
            {
                baseTime = earliestTime;    // the earliest start time
                timeline = new Session[numToTrack];
            }
            else
                throw new ArgumentException("Error: Faulty arguments for NodeTimeline!");
        }


        /// <summary>
        /// The purpose of this method is to randomly generate the 
        /// sessions.  First, the method iterates through a series
        /// of random sessions without storing them to allow the
        /// session timeline to evolve.  Once the timeline reaches
        /// the baseTime, sessions will be tracked.
        /// 
        /// This particular method uses a uniform distribution,
        /// [0, 1] for each uptime as well as each downtime.
        /// </summary>
        public void generateUUTimeline()
        {
            MersenneTwister randomNum = new MersenneTwister();

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genrand_real1();
                endTime = startTime + randomNum.genrand_real1();
            }

            for (int i = 0; i < timeline.Length; i++)
            {
                startTime = endTime + randomNum.genrand_real1();
                endTime = startTime + randomNum.genrand_real1();

                timeline[i] = new Session(startTime, endTime);
            }
        }


        /// <summary>
        /// The purpose of this method is to randomly generate the 
        /// sessions.  First, the method iterates through a series
        /// of random sessions without storing them to allow the
        /// session timeline to evolve.  Once the timeline reaches
        /// the baseTime, sessions will be tracked.
        /// 
        /// This particular method uses a Paretto distribution
        /// for each uptime and an exponential distribution for
        /// each downtime.
        /// </summary>
        public void generatePETimeline(double alpha = 3.0, double beta = 1.0, double lambda = 2.0)
        {
            MersenneTwister randomNum = new MersenneTwister();

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
            }

            for(int i = 0; i < timeline.Length; i++)
            {
                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genparet_real(alpha, beta);

                timeline[i] = new Session(startTime, endTime);
            }
        }


        /// <summary>
        /// The purpose of this method is to randomly generate the 
        /// sessions.  First, the method iterates through a series
        /// of random sessions without storing them to allow the
        /// session timeline to evolve.  Once the timeline reaches
        /// the baseTime, sessions will be tracked.
        /// 
        /// This particular method uses a Paretto distribution
        /// for each uptime as well as each downtime.
        /// </summary>
        public void generatePPTimeline(double alpha = 3.0, double beta = 1.0)
        {
            MersenneTwister randomNum = new MersenneTwister();

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genparet_real(alpha, beta);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
            }

            for (int i = 0; i < timeline.Length; i++)
            {
                startTime = endTime + randomNum.genparet_real(alpha, beta);
                endTime = startTime + randomNum.genparet_real(alpha, beta);

                timeline[i] = new Session(startTime, endTime);
            }
        }


        /// <summary>
        /// The purpose of this method is to randomly generate the 
        /// sessions.  First, the method iterates through a series
        /// of random sessions without storing them to allow the
        /// session timeline to evolve.  Once the timeline reaches
        /// the baseTime, sessions will be tracked.
        /// 
        /// This particular method uses an exponential distribution
        /// for each uptime as well as each downtime.
        /// </summary>
        public void generateEETimeline(double lambda = 2.0)
        {
            MersenneTwister randomNum = new MersenneTwister();

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genexp_real(lambda);
            }

            for (int i = 0; i < timeline.Length; i++)
            {
                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genexp_real(lambda);

                timeline[i] = new Session(startTime, endTime);
            }
        }


        /// <summary>
        /// The purpose of this method is to return a the start
        /// time of the first tracked session.
        /// </summary>
        public double getFirstTime()
        {
            if (timeline[0] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");
            else
                return timeline[0].StartTime;
        }


        /// <summary>
        /// The purpose of this method is to return a the endtime
        /// of the last tracked session.
        /// </summary>
        public double getFinalTime()
        {
            if (timeline[timeline.Length - 1] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");
            else
                return timeline[timeline.Length - 1].EndTime;
        }


        /// <summary>
        /// The purpose of this method is to determine if there
        /// exists a live session at time_t within the timeline.
        /// </summary>
        /// <param name="time_t">The time to test.</param>
        /// <returns>Whether any session is live.</returns>
        public bool timeIsLive(double time_t)
        {
            if (timeline[0] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");

            if (time_t < 0)
                throw new ArgumentException("Error: Argument error in timeIsLive!");

            for (int i = 0; i < timeline.Length; i++)
            {
                if (timeline[i].isLive(time_t))
                    return true;
                if (time_t < timeline[i].EndTime)
                    return false;
            }

            return false;
        }


        /// <summary>
        /// The purpose of this method is to calculate the average
        /// up time across all tracked sessions.
        /// </summary>
        /// <returns>The average up time.</returns>
        public double averageUpTime()
        {
            if (timeline[0] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");

            double sum = 0.0;
            for (int i = 0; i < timeline.Length; i++)
                sum += timeline[i].getDurationLive();

            return sum / Convert.ToDouble(timeline.Length);
        }


        /// <summary>
        /// The purpose of this method is to calculate the average
        /// downtime across all tracked sessions.  The first down 
        /// time follows the end of the first tracked session.
        /// </summary>
        /// <returns>The average downtime.</returns>
        public double averageDownTime()
        {
            if (timeline[0] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");

            double sum = 0.0;
            for (int i = 1; i < timeline.Length; i++)
                sum += (timeline[i].StartTime - timeline[i - 1].EndTime);

            return sum / Convert.ToDouble(timeline.Length - 1);
        }


        /// <summary>
        /// The purpose of this method is to calculate the cumulative 
        /// distribution function of the session durations from the 
        /// generated timeline.
        /// </summary>
        /// <param name="hSize">The number of discrete steps.</param>
        /// <param name="rate">The step rate for each step.</param>
        /// <returns>The ccdf distribution.</returns>
        public double[] getUpTimeCDF(int hSize = 50, double rate = 0.5)
        {
            if (timeline[0] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");

            if (hSize <= 0)
                throw new ArgumentException("Error: Argument error in timeIsLive!");

            if (rate <= 0)
                throw new ArgumentException("Error: Argument error in timeIsLive!");

            double[] cdf = new double[hSize];

            double cap = 0.0;
            for (int i = 0; i < hSize; i++)
            {
                for (int j = 0; j < timeline.Length; j++)
                {
                    if (timeline[j].getDurationLive() < cap)
                        cdf[i] += 1.0;
                }

                cap += rate;
            }

            for (int i = 0; i < hSize; i++)
                cdf[i] = cdf[i] / timeline.Length;

            return cdf;
        }


        /// <summary>
        /// The purpose of this method is to calculate the complementary
        /// cumulative distribution function of the down times between 
        /// sessions from the generated timeline.  The first down time
        /// follows the end of the first tracked session.
        /// </summary>
        /// <param name="hSize">The number of discrete steps.</param>
        /// <param name="rate">The step rate for each step.</param>
        /// <returns>The ccdf distribution.</returns>
        public double[] getDownTimeCDF(int hSize = 50, double rate = 0.5)
        {
            if (timeline[0] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");

            if (hSize <= 0)
                throw new ArgumentException("Error: Argument error in timeIsLive!");

            if (rate <= 0)
                throw new ArgumentException("Error: Argument error in timeIsLive!");

            double[] cdf = new double[hSize];

            double cap = 0.0;
            for (int i = 0; i < hSize; i++)
            {
                for (int j = 1; j < timeline.Length; j++)
                {
                    if ((timeline[j].EndTime - timeline[j-1].StartTime) < cap)
                        cdf[i] += 1.0;
                }

                cap += rate;
            }

            for (int i = 0; i < hSize; i++)
                cdf[i] = cdf[i] / timeline.Length;

            return cdf;
        }


        /// <summary>
        /// The purpose of this method is to convert the values contained in 
        /// class attributes baseTime and timeline to a string.
        /// </summary>
        /// <returns>The class attributes as a string.</returns>
        public override String ToString()
        {
            if (timeline[0] == null)
                throw new System.NullReferenceException("Error: Must first generate a timeline!");

            String s = "Base Time: " + baseTime + "\n";
            for (int i = 0; i < timeline.Length; i++)
            {
                s += timeline[i].ToString();
                s += "\n";
            }

            return s;
        }
    }
}
