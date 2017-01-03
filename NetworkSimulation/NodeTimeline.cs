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
    class NodeTimeline
    {
        private double baseTime = 0.0;  // the earliest possible start time  
        private Session[] timeline;     // the collection of tracked sessions


        /// <summary>
        /// The purpose of this 'getter' is to return a the endtime
        /// of the last tracked session.
        /// </summary>
        public double finalTime
        {
            get
            {
                return timeline[timeline.Length - 1].EndTime;
            }
        }


        /// <summary>
        /// A constructor for the NodeTimeline class.  Sets up the
        /// baseTime, the numSessions and creates the array that 
        /// will be used to store the tracked sessions.
        /// </summary>
        /// <param name="numToTrack">The number of sessions to track</param>
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
                timeline[i] = new Session(startTime, endTime);

                startTime = endTime + randomNum.genrand_real1();
                endTime = startTime + randomNum.genrand_real1();
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
        public void generatePETimeline()
        {
            MersenneTwister randomNum = new MersenneTwister();

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
            }

            for(int i = 0; i < timeline.Length; i++)
            {
                timeline[i] = new Session(startTime, endTime);

                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
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
        public void generatePPTimeline()
        {
            MersenneTwister randomNum = new MersenneTwister();

            double alpha = 3.0;
            double beta = 1.0;

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genparet_real(alpha, beta);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
            }

            for (int i = 0; i < timeline.Length; i++)
            {
                timeline[i] = new Session(startTime, endTime);

                startTime = endTime + randomNum.genparet_real(alpha, beta);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
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
        public void generateEETimeline()
        {
            MersenneTwister randomNum = new MersenneTwister();

            double lambda = 2.0;

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genexp_real(lambda);
            }

            for (int i = 0; i < timeline.Length; i++)
            {
                timeline[i] = new Session(startTime, endTime);

                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genexp_real(lambda);
            }
        }


        /// <summary>
        /// The purpose of this method is to determine if there
        /// exists a live session at time_t within the timeline.
        /// </summary>
        /// <param name="time_t">The time to test.</param>
        /// <returns>Whether any session is live.</returns>
        public bool timeIsLive(double time_t)
        {
            if (time_t < 0)
                throw new ArgumentException("Error: Argument error in timeIsLive!");

            if (finalTime < time_t)
                return false;
            else
            {
                bool live = false;
                int index = 0;

                while (timeline[index].EndTime < time_t)
                {
                    live = timeline[index].isLive(time_t);
                    index++;
                }

                return live;
            }
        }


        /// <summary>
        /// Calculate the 
        /// </summary>
        /// <returns></returns>
        public double averageUpTime()
        {
            double sum = 0.0;
            for (int i = 0; i < timeline.Length; i++)
                sum += timeline[i].getDurationLive();

            return sum / Convert.ToDouble(timeline.Length);
        }

        public double averageDownTime()
        {
            double sum = 0.0;
            for (int i = 1; i < timeline.Length; i++)
                sum += timeline[i].StartTime - timeline[i - 1].EndTime;

            return sum / Convert.ToDouble(timeline.Length);
        }


        public void displayTimeline()
        {
            for (int i = 0; i < timeline.Length; i++)
                Console.WriteLine(timeline[i].ToString());
        }
    }
}
