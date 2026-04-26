using System;
using System.Collections.Generic;


namespace NetworkSimulation
{
    /// <summary>
    /// The purpose of this class is to represent a set of
    /// peers within a peer-to-peer network that will join
    /// the network for a duration (a session) and then 
    /// depart the network for a duration (downtime) and
    /// then rejoin the network.  Thus, the lifecycle of
    /// each peer is represented as a NodeTimeline.
    /// </summary>
    public class NetworkChurn
    {
        private List<NodeTimeline> nodeSessions;    // the timelines of all peers
        private int numNodes;                       // the number of peers in the network


        /// <summary>
        /// The purpose of this 'getter' is to return the timeline
        /// for a specific node
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public NodeTimeline this[int i]
        {
            get
            {
                if ((i >= 0) && (i < nodeSessions.Count))
                    return nodeSessions[i];
                else
                    throw new IndexOutOfRangeException("Error: Requested element does not exist!");
            }
        }


        /// <summary>
        /// A constructor for the NetworkChurn class.  Creates 
        /// the array that will be used to store the tracked 
        /// session on the number of nodes specified by the
        /// parameter.
        /// </summary>
        /// <param name="numNodes">The number of nodes in the network.</param>
        public NetworkChurn(int numNodes)
        {
            if (numNodes > 0)
            {
                this.numNodes = numNodes;
                nodeSessions = new List<NodeTimeline>(numNodes);
            }
            else
            {
                throw new ArgumentException("Error: Faulty arguments for NetworkChurn!");
            }
        }


        /// <summary>
        /// The purpose of this method is to generate a timeline
        /// for each node using the NodeTimeline class with each
        /// up time as well as each down time generated using the 
        /// uniform distribution.  The parameters specify the 
        /// number of live sessions to track for each node and the 
        /// base time to start tracking each timeline.  Thus, the 
        /// start time of the first session for each node will be 
        /// at or later than the base time.
        /// </summary>
        /// <param name="numSessions">The number of live sessions to track</param>
        /// <param name="baseTime">The earliest time to track sessions</param>
        public void generateChurn(int numSessions, double baseTime, Distribution upDistro, Distribution downDistro)
        {
            nodeSessions.Clear();

            for (int i = 0; i < numNodes; i++)
            {
                NodeTimeline timeline = new NodeTimeline(numSessions, baseTime);
                timeline.generateTimeline(upDistro, downDistro);
                nodeSessions.Add(timeline);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public double getNodeResidualAtTime(int node, double time)
        {
            EnsureChurnGenerated();

            if ((node < 0) || (node >= nodeSessions.Count))
                throw new ArgumentException("Error: Argument error in getNodeResidualAtTime!");

            return nodeSessions[node].getResidual(time);
        }


        public double getNextStartTimeForNode(int node, double time)
        {
            EnsureChurnGenerated();

            if ((node < 0) || (node >= nodeSessions.Count))
                throw new ArgumentException("Error: Argument error in getNodeResidualAtTime!");

            return nodeSessions[node].getNextStart(time);
        }

        /// <summary>
        /// The purpose of this method is to generate a boolean
        /// array that specifies the status (alive or not) of
        /// each node at the time specified by the argument.
        /// </summary>
        /// <param name="time">The time to check all nodes</param>
        /// <returns>True for each live node; false otherwise</returns>
        public bool[] getStatusAtTime(double time)
        {
            EnsureChurnGenerated();

            bool[] status = new bool[nodeSessions.Count];
            for (int i = 0; i < status.Length; i++)
                status[i] = nodeSessions[i].timeIsLive(time);

            return status;
        }


        /// <summary>
        /// The purpose of this method is to iterate through all 
        /// of the node timelines and return the earliest start 
        /// time of the first occuring session.
        /// </summary>
        /// <returns>The earliest first start time</returns>
        public double getEarliestFirstTime()
        {
            EnsureChurnGenerated();

            double firstTime = nodeSessions[0].getFirstTime();
            for (int i = 1; i < nodeSessions.Count; i++)
            {
                if (nodeSessions[i].getFirstTime() < firstTime)
                    firstTime = nodeSessions[i].getFirstTime();
            }

            return firstTime;
        }


        /// <summary>
        /// The purpose of this method is to iterate through all 
        /// of the node timelines and return the earliest end 
        /// time of the first occuring session.
        /// </summary>
        /// <returns>The earliest first start time</returns>
        public double getEarliestFirstEnd()
        {
            EnsureChurnGenerated();

            double firstEnd = nodeSessions[0].getFirstEnd();
            for (int i = 1; i < nodeSessions.Count; i++)
            {
                if (nodeSessions[i].getFirstEnd() < firstEnd)
                    firstEnd = nodeSessions[i].getFirstEnd();
            }

            return firstEnd;
        }


        /// <summary>
        /// The purpose of this method is to iterate through all 
        /// of the node timelines and return the earliest stop 
        /// time of the last occuring session.
        /// </summary>
        /// <returns>The earliest final stop time</returns>
        public double getEarliestFinalTime()
        {
            EnsureChurnGenerated();

            double shortestTime = nodeSessions[0].getFinalTime();
            for (int i = 1; i < nodeSessions.Count; i++)
            {
                if (nodeSessions[i].getFinalTime() < shortestTime)
                    shortestTime = nodeSessions[i].getFinalTime();
            }

            return shortestTime;
        }


        /// <summary>
        /// The purpose of this method is to iterate through all 
        /// of the node timelines and return the last stop 
        /// time of the last occuring session.
        /// </summary>
        /// <returns>The earliest final stop time</returns>
        public double getLastFinalTime()
        {
            EnsureChurnGenerated();

            double lastTime = nodeSessions[0].getFinalTime();
            for (int i = 1; i < nodeSessions.Count; i++)
            {
                if (nodeSessions[i].getFinalTime() > lastTime)
                   lastTime = nodeSessions[i].getFinalTime();
            }

            return lastTime;
        }


        /// <summary>
        /// The purpose of this method is to store the start times for all sessions
        /// across all nodes.  The method sorts the start times before returning 
        /// the results.
        /// </summary>
        /// <returns>The sorted start times of all sessions.</returns>
        public double[] getStartTimes()
        {
            int numSessions = 0;
            for (int i = 0; i < nodeSessions.Count; i++)
                numSessions += nodeSessions[i].numSessions;

            double[] startTimes = new double[numSessions];

            int baseValue = 0;
            for (int i = 0; i < nodeSessions.Count; i++)
            {
                double[] sessionStarts = nodeSessions[i].getStartTimes();
                for (int j = 0; j < sessionStarts.Length; j++)
                    startTimes[baseValue + j] = sessionStarts[j];
                    
                baseValue += sessionStarts.Length;
            }

            Array.Sort(startTimes);
            return startTimes;
        }

        /// <summary>
        /// The purpose of this method is to calculate the 
        /// average live (up) session time across all sessions
        /// and all nodes.
        /// </summary>
        /// <returns>The average up time</returns>
        public double getAverageUpTime()
        {
            EnsureChurnGenerated();

            double sum = 0.0;

            for (int i = 0; i < nodeSessions.Count; i++)
                sum += nodeSessions[i].averageUpTime();

            return sum / Convert.ToDouble(nodeSessions.Count);
        }


        /// <summary>
        /// The purpose of this method is to calculate the 
        /// average time between live sessions across all 
        /// sessions and all nodes.
        /// </summary>
        /// <returns>The average down time</returns>
        public double getAverageDownTime()
        {
            EnsureChurnGenerated();

            double sum = 0.0;

            for (int i = 0; i < nodeSessions.Count; i++)
                sum += nodeSessions[i].averageDownTime();

            return sum / Convert.ToDouble(nodeSessions.Count);
        }


        /// <summary>
        /// This helper method is used to ensure that the churn has been generated before
        /// any of the methods that rely on the churn data are called.  If the churn has 
        /// not been generated, then a NullReferenceException is thrown with an appropriate 
        /// error message.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        private void EnsureChurnGenerated()
        {
            if (nodeSessions.Count == 0)
                throw new NullReferenceException("Error: Must first generate churn!");
        }
    }
}
