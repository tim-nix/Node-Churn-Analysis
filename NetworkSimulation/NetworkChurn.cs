using System;


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
    class NetworkChurn
    {
        private NodeTimeline[] nodeSessions;    // the timelines of all peers

        public NetworkChurn(int numNodes)
        {
            nodeSessions = new NodeTimeline[numNodes];
        }


        public void generateChurn(int numSessions, double baseTime)
        {
            for (int i = 0; i < nodeSessions.Length; i++)
            {
                nodeSessions[i] = new NodeTimeline(numSessions, baseTime);
                nodeSessions[i].generatePETimeline();
            }
        }

        public bool[] getStatusAtTime(double time)
        {
            bool[] status = new bool[nodeSessions.Length];
            for (int i = 0; i < status.Length; i++)
                status[i] = nodeSessions[i].timeIsLive(time);

            return status;
        }

        public double getQuickestFinalTime()
        {
            double shortestTime = nodeSessions[0].getFinalTime();
            for (int i = 1; i < nodeSessions.Length; i++)
            {
                if (nodeSessions[i].getFinalTime() < shortestTime)
                    shortestTime = nodeSessions[i].getFinalTime();
            }

            return shortestTime;
        }

        public double getAverageUpTime()
        {
            double sum = 0.0;

            for (int i = 0; i < nodeSessions.Length; i++)
                sum += nodeSessions[i].averageUpTime();

            return sum / Convert.ToDouble(nodeSessions.Length);
        }

        public double getAverageDownTime()
        {
            double sum = 0.0;

            for (int i = 0; i < nodeSessions.Length; i++)
                sum += nodeSessions[i].averageDownTime();

            return sum / Convert.ToDouble(nodeSessions.Length);
        }
    }
}
