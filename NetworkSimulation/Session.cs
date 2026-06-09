using System;


namespace NetworkSimulation
{
    /// <summary>
    /// Represents one node ON session using the half-open interval
    /// [start, end), so the node is OFF exactly at the end time.
    /// </summary>
    public class Session
    {
        private double start;       // start time of the session
        private double end;         // end time of the session

        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member start.
        /// </summary>
        public double StartTime
        {
            get
            {
                return start;
            }
        }

        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member end.
        /// </summary>
        public double EndTime
        {
            get
            {
                return end;
            }
        }


        /// <summary>
        /// A constructor for the Session class.
        /// </summary>
        /// <param name="startTime">The start time of the session.</param>
        /// <param name="endTime">The end time of the session.</param>
        public Session(double startTime, double endTime)
        {
            if (startTime < endTime)
            {
                start = startTime;
                end = endTime;
            }
            else
                throw new ArgumentException("Error: Start time must occur before end time!");
        }


        /// <summary>
        /// Determines whether a time lies in this session's half-open interval.
        /// </summary>
        /// <param name="time_t">The time to test.</param>
        /// <returns>Whether the session is live.</returns>
        public bool isLive(double time_t)
        {
            if ((time_t >= start) && (time_t < end))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the remaining ON time in this session.
        /// </summary>
        /// <param name="time_t">The time to test.</param>
        /// <returns>The time remaining.</returns>
        public double getResidual(double time_t)
        {
            if (isLive(time_t))
                return end - time_t;
            else
                return 0.0;
        }


        /// <summary>
        /// Returns the total ON duration of the session.
        /// </summary>
        /// <returns>End time minus start time.</returns>
        public double getDurationLive()
        {
            return end - start;
        }


        /// <summary>
        /// The purpose of this method is to convert the values contained in 
        /// class attributes start and end to a string.
        /// </summary>
        /// <returns>The class attributes as a string.</returns>
        public override String ToString()
        {
            return ("Start Time = " + start + " and End Time = " + end);
        }
            
    }
}
