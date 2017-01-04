using System;


namespace NetworkSimulation
{
    /// <summary>
    /// The purpose of this class is to keep track of a single live
    /// session of a node.  The session goes live at the "time"
    /// specified by the start attribute.  The session is live
    /// until the "time" specified by the end attribute occurs.
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
        /// The purpose of this method is to return whether or
        /// not the session is live at time_t.
        /// </summary>
        /// <param name="time_t">The time to test.</param>
        /// <returns>Whether the session is live.</returns>
        public bool isLive(double time_t)
        {
            if ((time_t >= start) && (time_t <= end))
                return true;
            else
                return false;
        }


        /// <summary>
        /// The purpose of this method is to calculate the
        /// duration of time of the session.
        /// </summary>
        /// <returns></returns>
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
