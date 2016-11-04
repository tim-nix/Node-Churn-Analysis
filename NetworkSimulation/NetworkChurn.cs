using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class NetworkChurn
    {
        NodeTimeline[] nodeSessions;

        public NetworkChurn(int numNodes)
        {
            nodeSessions = new NodeTimeline[numNodes];
        }


        public void generateChurn(int numSessions, double baseTime)
        {
            for (int i = 0; i < nodeSessions.Length; i++)
            {
                nodeSessions[i] = new NodeTimeline(numSessions, baseTime);
                nodeSessions[i].generateTimeline();
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
    }
}
