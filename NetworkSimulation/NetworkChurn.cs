using System;


namespace NetworkSimulation
{
    class NetworkChurn
    {
        private NodeTimeline[] nodeSessions;

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
            double shortestTime = nodeSessions[0].finalTime;
            for (int i = 1; i < nodeSessions.Length; i++)
            {
                if (nodeSessions[i].finalTime < shortestTime)
                    shortestTime = nodeSessions[i].finalTime;
            }

            return shortestTime;
        }

        public double getAverageLiveTime()
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
